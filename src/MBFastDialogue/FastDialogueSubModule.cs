using HarmonyLib;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MCM.Internal.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace MBFastDialogue
{
    /// <summary>
    /// The entry point for the submodule
    /// </summary>
    public class FastDialogueSubModule : MBSubModuleBase
    {
        public static string FastEncounterMenu = "fast_combat_menu";

        public static string ModuleName = "MBFastDialogue";

        public static FastDialogueSubModule? Instance { get; private set; }

        private Settings settings { get; set; }

        private InputKey _toggleKey = InputKey.X;

        public bool Running { get; private set; } = true;

        private Harmony _harmony;

        public FastDialogueSubModule()
        {
            base.OnSubModuleLoad();
            Instance = this;
            try
            {
                _harmony = new Harmony("io.dallen.bannerlord.fastdialogue");
                _harmony.PatchAll(typeof(FastDialogueSubModule).Assembly);

                LoadSettingsFromMCM();
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Loaded {ModuleName} with MCM support",
                    Color.FromUint(4282569842U)
                ));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Fast Dialogue error during load: {ex.Message}", 
                    Colors.Red
                ));
            }
        }
        
        private void LoadSettingsFromMCM()
        {
            try
            {
                var newSettings = Settings.FromMCM();
                if(newSettings == null) return;
                settings = newSettings;
                Enum.TryParse(settings.ToggleKey, out _toggleKey);
                if (MCMSettings.Instance != null) Running = MCMSettings.Instance.EnableMod;
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Fast Dialogue: MCM settings error - {ex.Message}", 
                    Colors.Red
                ));
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("io.dallen.bannerlord.fastdialogue");
            
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage($"Loaded {ModuleName}. Toggle Hotkey: CTRL + {settings.ToggleKey}", Color.FromUint(4282569842U)));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign campaign && gameStarter is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new FastDialogueCampaignBehaviorBase());
            }
        }

        public bool IsPatternWhitelisted(string name)
        {
            if (settings.Whitelist.WhitelistPatterns.Count == 0)
            {
                return true;
            }

            foreach (var pattern in settings.Whitelist.WhitelistPatterns)
            {
                if (name.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnApplicationTick(float dt)
        {            
            if (MCMSettings.Instance == null) return;
            Running = MCMSettings.Instance.EnableMod;
            Enum.TryParse(MCMSettings.Instance.ToggleKey.SelectedValue, out _toggleKey);

            var topScreen = ScreenManager.TopScreen;

            if (topScreen == null || (!Input.IsKeyDown(InputKey.LeftControl) && !Input.IsKeyDown(InputKey.RightControl)) || !Input.IsKeyPressed(_toggleKey))
            {
                Running = !Running;
                MCMSettings.Instance.EnableMod = Running;
                InformationManager.DisplayMessage(new InformationMessage(ModuleName + " is now " + (Running ? "active" : "inactive"), Color.FromUint(4282569842U)));
            }
        }

        private static T? LoadSettingsFor<T>(string moduleName) where T : class
        {
            var settingsPath = Path.Combine(BasePath.Name, "Modules", moduleName, "settings.xml");
            try
            {
                using (var reader = XmlReader.Create(settingsPath))
                {
                    var root = new XmlRootAttribute();
                    root.ElementName = moduleName + ".Settings";
                    root.IsNullable = true;

                    if (reader.MoveToContent() != XmlNodeType.Element)
                    {
                        return null;
                    }

                    if (reader.Name != root.ElementName)
                    {
                        return null;
                    }

                    var serialiser = new XmlSerializer(typeof(T), root);
                    var loaded = (T)serialiser.Deserialize(reader);
                    return loaded;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}