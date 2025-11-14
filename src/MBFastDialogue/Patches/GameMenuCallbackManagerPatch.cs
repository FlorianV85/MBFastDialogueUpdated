using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Library;

namespace MBFastDialogue.Patches
{
    /// <summary>
    /// Hook the menu setup method to ensure the fast encounter method is hooked correctly
    /// </summary>
    [HarmonyPatch(typeof(GameMenuCallbackManager), "InitializeState")]
    public static class GameMenuCallbackManagerPatch
    {
        private static readonly Type? DefaultEncounterType;
        private static readonly MethodInfo? GameMenuEncounterOnInitMethod;

        static GameMenuCallbackManagerPatch()
        {
            DefaultEncounterType =
                typeof(GameMenu).Assembly.GetType(
                    "TaleWorlds.CampaignSystem.GameMenus.GameMenuInitializationHandlers.DefaultEncounter");
            
            GameMenuEncounterOnInitMethod =
                DefaultEncounterType?.GetMethod("game_menu_encounter_on_init",
                    BindingFlags.Static | BindingFlags.NonPublic);
        }
        
        private static void Postfix(GameMenuCallbackManager __instance, string menuId, MenuContext state)
        {
            if (menuId != FastDialogueSubModule.FastEncounterMenu || GameMenuEncounterOnInitMethod == null) return;
            
            try
            {
                var args = new MenuCallbackArgs(state, null);
                GameMenuEncounterOnInitMethod.Invoke(null, new object[] { args });
                
                /*if (menuId != FastDialogueSubModule.FastEncounterMenu) return;
                var args = new MenuCallbackArgs(state, null);
                GameMenuEncounterOnInitMethod.Invoke(null, new object[] { args });*/
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Fast Dialogue failed to init menu - {ex.Message}", Color.FromUint(4282569842U)));
            }
        }
    }
}