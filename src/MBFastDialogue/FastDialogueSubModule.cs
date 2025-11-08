using HarmonyLib;
using MBFastDialogue.CampaignBehaviors;
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

        private TaleWorlds.InputSystem.InputKey toggleKey = InputKey.X;

        public bool running { get; private set; } = true;

        private Harmony harmony;

        public FastDialogueSubModule()
        {
            base.OnSubModuleLoad();
            Instance = this;
            try
            {
                harmony = new Harmony("io.dallen.bannerlord.fastdialogue");
                harmony.PatchAll(typeof(FastDialogueSubModule).Assembly);
                
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
                settings = Settings.FromMCM();
                Enum.TryParse(settings.toggle_key, out toggleKey);
                if (MCMSettings.Instance != null) running = MCMSettings.Instance.EnableMod;
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
            harmony?.UnpatchAll("io.dallen.bannerlord.fastdialogue");
            
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
            InformationManager.DisplayMessage(new InformationMessage($"Loaded {ModuleName}. Toggle Hotkey: CTRL + {settings.toggle_key}", Color.FromUint(4282569842U)));
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
            LoadSettingsFromMCM(); 
            if (settings.whitelist.whitelistPatterns.Count == 0)
            {
                return true;
            }

            foreach (var pattern in settings.whitelist.whitelistPatterns)
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
            running = MCMSettings.Instance.EnableMod;
            Enum.TryParse(MCMSettings.Instance.ToggleKey.SelectedValue, out toggleKey);

            var topScreen = ScreenManager.TopScreen;

            if (topScreen == null ||
                (!Input.IsKeyDown(InputKey.LeftControl) && !Input.IsKeyDown(InputKey.RightControl)) ||
                !Input.IsKeyPressed(toggleKey)) return;
            running = !running;
            MCMSettings.Instance.EnableMod = running;
            InformationManager.DisplayMessage(new InformationMessage(
                ModuleName + " is now " + (running ? "active" : "inactive"), Color.FromUint(4282569842U)));
        }
    }
}