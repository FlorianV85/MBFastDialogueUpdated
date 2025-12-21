using System;
using System.Collections.Generic;
using HarmonyLib;
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
        private static readonly HashSet<string> ExcludedOptions = new HashSet<string>
        {
            "continue_preparations", "village_raid_action", "village_force_volunteer_action",
            "village_force_supplies_action", "attack", "capture_the_enemy", "str_order_attack",
            "leave_soldiers_behind", "surrender", "leave", "go_back_to_settlement"
        };
        
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
            if (menuId != "encounter" || ExcludedOptions.Contains(optionId)) return;
            
            try
            {
                __instance.AddGameMenuOption(
                    FastDialogueSubModule.FastEncounterMenu,
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