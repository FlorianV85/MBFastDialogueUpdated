using System;
using HarmonyLib;
using MBFastDialogue.Constants;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Library;

namespace MBFastDialogue.Patches
{
    // For other mods compatibility
    // When a non-native menu option is added to menu ID "encounter", it is added as a Fast Dialogue menu option   
    [HarmonyPatch(typeof(CampaignGameStarter), "AddGameMenuOption")]
    public static class CampaignGameStarterPatch
    {
        private static void Postfix(
            CampaignGameStarter __instance,
            string menuId, 
            string optionId, 
            string optionText, 
            GameMenuOption.OnConditionDelegate condition, 
            GameMenuOption.OnConsequenceDelegate consequence, 
            bool isLeave = false, 
            int index = -1, 
            bool isRepeatable = false, 
            object relatedObject = null)
        {
            if (menuId != "encounter" || ExcludeMenuOptions.Ids.Contains(optionId)) return;
            
            try
            {
                __instance.AddGameMenuOption(
                    ModuleConstants.FastEncounterMenu,
                    optionId,
                    optionText, 
                    condition, 
                    consequence, 
                    isLeave, 
                    index, 
                    isRepeatable, 
                    relatedObject);
            }
            catch(Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Fast Dialogue: Mod integration failed - {ex.Message}", Color.FromUint(4282569842U)));
            }
        }
    }
}