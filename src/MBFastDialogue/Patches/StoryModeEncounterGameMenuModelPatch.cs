using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace MBFastDialogue.Patches
{
    /// <summary>
    /// Catches game trying to setup a new map menu and subs in the fast encounter menu when appropriate
    /// </summary>
    [HarmonyPatch(typeof(DefaultEncounterGameMenuModel), "GetEncounterMenu")]
    public static class StoryModeEncounterGameMenuModelPatch
    {
        private static void Postfix(
            DefaultEncounterGameMenuModel __instance, 
            ref string __result, 
            PartyBase attackerParty, 
            PartyBase defenderParty, 
            bool startBattle, 
            bool joinBattle)
        {
            try
            {
                var encountered = PlayerEncounter.EncounteredParty;
                if (encountered == null) return;
                //InformationManager.DisplayMessage(new InformationMessage($"{encounteredPartyBase.Id}", Color.FromUint(4282569842U)));
                var result = EncounterMenuEvaluator.GetEncounterMenu(encountered);
                if (result != null)
                {
                    __result = result;
                }
            }
            catch (Exception ex)
            { 
                InformationManager.DisplayMessage(new InformationMessage($"Fast Dialogue: Encounter handling failed - {ex.Message}", Color.FromUint(4282569842U)));
            }
        }
    }
}