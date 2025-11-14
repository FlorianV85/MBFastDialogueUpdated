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
#if v110 || v111 || v112 || v113 || v114 || v115 || v116 || v120 || v221 || v122 || v123 || v124 || v125 || v126 || v127 || v128 || v129 || v1210 || v1211 || v1212
        private static readonly MethodInfo GetEncounteredPartyBaseMethod = typeof(DefaultEncounterGameMenuModel).GetMethod("GetEncounteredPartyBase", BindingFlags.Instance | BindingFlags.NonPublic);
#endif

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
#if v110 || v111 || v112 || v113 || v114 || v115 || v116 || v120 || v221 || v122 || v123 || v124 || v125 || v126 || v127 || v128 || v129 || v1210 || v1211 || v1212

                var encountered = (PartyBase)GetEncounteredPartyBaseMethod?.Invoke(__instance, new object[] { attackerParty, defenderParty });
#else
                var encountered = PlayerEncounter.EncounteredParty;
#endif
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