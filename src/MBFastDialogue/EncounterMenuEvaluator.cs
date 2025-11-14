using TaleWorlds.CampaignSystem.Party;

namespace MBFastDialogue
{
    internal static class EncounterMenuEvaluator
    {
        public static string GetEncounterMenu(PartyBase encountered)
        {
            // Debug
            /*InformationManager.DisplayMessage(new InformationMessage($"Party ID : {encounteredPartyBase.Id}", Color.FromUint(4282569842U)));
            if(encounteredPartyBase.MobileParty != null)
            {
                InformationManager.DisplayMessage(new InformationMessage($"MobileParty StringId : {encounteredPartyBase.MobileParty.StringId}", Color.FromUint(4282569842U)));
            }*/

            var instance = FastDialogueSubModule.Instance;
            if (instance?.Running != true) return null;

            if (encountered.IsSettlement || encountered.MapEvent != null) return null;

            var partyId = encountered.Id;

            if (partyId.Contains("locate_and_rescue_traveller_quest_raider_party")) return null;

            if (!instance.IsPatternWhitelisted(partyId)) return null;

            var main = PartyBase.MainParty;
            var encounteredFaction = encountered.MapFaction;
            var mainFaction = main.MapFaction;
            var isAtWar = mainFaction.IsAtWarWith(encounteredFaction);
            
            if (!isAtWar)
            {
                var mobile = encountered.MobileParty;
                if (encounteredFaction.IsClan || 
                    encounteredFaction.IsMinorFaction ||
                    (mobile?.IsLordParty == true))
                {
                    return null;
                }
            }
            
            if (encounteredFaction == mainFaction && 
                mainFaction.Leader?.CharacterObject == main.LeaderHero?.CharacterObject)
            {
                return null;
            }
            
            var mobileParty = encountered.MobileParty;
            if (mobileParty?.IsCurrentlyUsedByAQuest == true && partyId.Contains("villager")) return null;
            
            if (!encountered.IsMobile) return FastDialogueSubModule.FastEncounterMenu;
            
            if (mobileParty != null)
            {
                var mainMobile = MobileParty.MainParty;
                var isGarrisonWithSiege = mobileParty.IsGarrison && mainMobile.BesiegedSettlement != null;
                var isOwnBesiegedSettlement = mainMobile.CurrentSettlement != null && 
                                              mobileParty.BesiegedSettlement == mainMobile.CurrentSettlement;
                
                if (!isGarrisonWithSiege && !isOwnBesiegedSettlement)
                {
                    return FastDialogueSubModule.FastEncounterMenu;
                }
            }
            
            /*if (FastDialogueSubModule.Instance != null && !FastDialogueSubModule.Instance.Running)
            {
                return null;
            }

            if(encounteredPartyBase.Id.Contains("locate_and_rescue_traveller_quest_raider_party"))
            {
                return null;
            }

            if (encounteredPartyBase.IsSettlement || encounteredPartyBase.MapEvent != null)
            {
                return null;
            }*/

            /*if ((encounteredPartyBase.MapFaction.IsClan || (encounteredPartyBase.MobileParty != null && encounteredPartyBase.MobileParty.IsLordParty) || encounteredPartyBase.MapFaction.IsMinorFaction) && !PartyBase.MainParty.MapFaction.IsAtWarWith(encounteredPartyBase.MapFaction))
            {
                return null;
            }

            if (FastDialogueSubModule.Instance != null && !FastDialogueSubModule.Instance.IsPatternWhitelisted(encounteredPartyBase.Id))
            {
                return null;
            }

            var inOwnedKingdom = encounteredPartyBase.MapFaction == PartyBase.MainParty.MapFaction && PartyBase.MainParty.MapFaction.Leader.CharacterObject == PartyBase.MainParty.LeaderHero.CharacterObject;
            if (inOwnedKingdom)
            {
                return null;
            }

            if (encounteredPartyBase.MobileParty?.IsCurrentlyUsedByAQuest == true && encounteredPartyBase.Id.Contains("villager"))
            {
                return null;
            }

            if (!encounteredPartyBase.IsMobile)
            {
                return FastDialogueSubModule.FastEncounterMenu;
            }

            var notGarrisonOrSiege = !encounteredPartyBase.MobileParty.IsGarrison || MobileParty.MainParty.BesiegedSettlement == null;
            var notOwnSettlementOrNotOwnBesiegedSettlement = MobileParty.MainParty.CurrentSettlement == null || encounteredPartyBase.MobileParty.BesiegedSettlement != MobileParty.MainParty.CurrentSettlement;
            if (notGarrisonOrSiege && notOwnSettlementOrNotOwnBesiegedSettlement)
            {
                return FastDialogueSubModule.FastEncounterMenu;
            }*/
            return null;
        }
    }
}