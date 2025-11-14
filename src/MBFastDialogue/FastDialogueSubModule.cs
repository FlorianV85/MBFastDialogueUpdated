using HarmonyLib;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
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
        public const string FastEncounterMenu = "fast_combat_menu";
        private const string ModuleName = "MBFastDialogue";
        private const string HarmonyId = "io.dallen.bannerlord.fastdialogue";

        public static FastDialogueSubModule Instance { get; private set; }

        private Settings _settings { get; set; } = new Settings();
        private InputKey _toggleKey = InputKey.X;
        private Harmony _harmony; 
        public bool Running { get; private set; } = true;


        public FastDialogueSubModule()
        {
            Instance = this;
        }

        /*public FastDialogueSubModule()
        {
            base.OnSubModuleLoad();
            Instance = this;
            try
            {
                _harmony = new Harmony("io.dallen.bannerlord.fastdialogue");
                _harmony.PatchAll(typeof(FastDialogueSubModule).Assembly);

                var newSettings = LoadSettingsFor<Settings>(ModuleName);
                if (newSettings == null) return;
                settings = newSettings;
                Enum.TryParse(settings.ToggleKey, out _toggleKey);
            }
            catch (Exception)
            {
                // TODO: Find a logger
            }
        }*/
        
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                InitializeSettings();
                InitializeHarmony();
            }
            catch (Exception)
            {
                Running = false;
            }
        }
        
        private void InitializeSettings()
        {
            _settings = Settings.FromMCM() ?? new Settings();
            _toggleKey = Enum.TryParse(_settings.ToggleKey, out InputKey key) ? key : InputKey.X;
            if (MCMSettings.Instance != null) Running = MCMSettings.Instance.EnableMod;
        }

        private void InitializeHarmony()
        {
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll(typeof(FastDialogueSubModule).Assembly);
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll(HarmonyId);
            ReflectionUtils.ClearCache();
            Instance = null;

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            var message = $"Loaded {ModuleName}. Toggle: CTRL + {_settings?.ToggleKey ?? "X"}";
            InformationManager.DisplayMessage(new InformationMessage(message, Color.FromUint(4282569842U)));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign && gameStarter is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new FastDialogueCampaignBehaviorBase());
            }
        }

        public bool IsPatternWhitelisted(string name)
        {
            if (_settings?.Whitelist?.WhitelistPatterns == null || _settings.Whitelist.WhitelistPatterns.Count == 0)
            {
                return true;
            }

            var patterns = _settings.Whitelist.WhitelistPatterns;
            foreach (var t in patterns)
            {
                if (name.Contains(t))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnApplicationTick(float dt)
        {            
            if (ScreenManager.TopScreen == null) return;
            if (MCMSettings.Instance == null) return;
            if (!IsToggleKeyPressed()) return;
            
            Running = !Running;
            MCMSettings.Instance.EnableMod = Running;
            var status = Running ? "active" : "inactive";
            InformationManager.DisplayMessage(new InformationMessage($"{ModuleName} is now {status}", Color.FromUint(4282569842U)));
        }

        private bool IsToggleKeyPressed()
        {
            Enum.TryParse(MCMSettings.Instance.ToggleKey.SelectedValue, out _toggleKey);
            return (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl)) && 
                   Input.IsKeyPressed(_toggleKey);
        }
        
        private static T? LoadSettingsFor<T>(string moduleName) where T : class
        {
            var settingsPath = Path.Combine(BasePath.Name, "Modules", moduleName, "settings.xml");

            if (!File.Exists(settingsPath))
            {
                return null;
            }
            
            try
            {
                using (var reader = XmlReader.Create(settingsPath))
                {
                    if (reader.MoveToContent() != XmlNodeType.Element)
                    {
                        return null;
                    }

                    var root = new XmlRootAttribute()
                    {
                        ElementName = moduleName + ".Settings",
                        IsNullable = true
                    };

                    if (reader.Name != root.ElementName)
                    {
                        return null;
                    }

                    var serialiser = new XmlSerializer(typeof(T), root);
                    return serialiser.Deserialize(reader) as T;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}