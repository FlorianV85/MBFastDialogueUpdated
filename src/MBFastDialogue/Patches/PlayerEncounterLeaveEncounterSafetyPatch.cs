using HarmonyLib;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Library;

namespace MBFastDialogue.Patches
{
    [HarmonyPatch(typeof(PlayerEncounter), nameof(PlayerEncounter.LeaveEncounter), MethodType.Setter)]
    public static class PlayerEncounterLeaveEncounterSafetyPatch
    {
        private static bool Prefix()
        {
            if (PlayerEncounter.Current != null) return true;
            InformationManager.DisplayMessage(
                new InformationMessage(
                    "Fast Dialogue: PlayerEncounter not initialized, interaction skipped", 
                    Color.FromUint(4282569842U)
                )
            );
            return false;
        }
    }
}