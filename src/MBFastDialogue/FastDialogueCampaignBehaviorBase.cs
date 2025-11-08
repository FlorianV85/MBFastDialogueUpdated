using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
#if v110 || v111 || v112 || v113 || v114 || v115 || v116 || v120 || v221 || v122 || v123 || v124 || v125 || v126 || v127 || v128 || v129 || v1210 || v1211 || v1212
using TaleWorlds.CampaignSystem.Overlay;
#endif

namespace MBFastDialogue
{
    public class FastDialogueCampaignBehaviorBase : EncounterGameMenuBehavior
    {
        private EncounterGameMenuBehavior GetGlobalCampaignBehaviorManager() => Campaign.Current.GetCampaignBehavior<EncounterGameMenuBehavior>();

        private void Init(MenuCallbackArgs args)
        {
            ReflectionUtils.ForceCall<object>(GetGlobalCampaignBehaviorManager(), "game_menu_encounter_on_init",
                new object[] { args });
            
            if (PlayerEncounter.Current == null && PlayerEncounter.EncounteredParty != null)
            {
                PlayerEncounter.RestartPlayerEncounter(
                    PlayerEncounter.EncounteredParty, 
                    PartyBase.MainParty
                );
            }
        }

        private GameMenuOption.OnConditionDelegate ConditionOf(string name) =>
            (args) => ReflectionUtils.ForceCall<bool>(GetGlobalCampaignBehaviorManager(), name, new object[] { args });
        private GameMenuOption.OnConsequenceDelegate ConsequenceOf(string name) =>
            (args) => ReflectionUtils.ForceCall<object>(GetGlobalCampaignBehaviorManager(), name, new object[] { args });

        private bool ShouldShowWarOptions()
        {
            try
            {
                if(PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.Id.Contains("quest_party_template"))
                {
                    return true;
                }
                
                if(PlayerEncounter.EncounteredParty != null && (PlayerEncounter.EncounteredMobileParty != null && (PlayerEncounter.EncounteredMobileParty.StringId.Contains("conspiracy") || PlayerEncounter.EncounteredMobileParty.StringId.Contains("conspirator"))))
                {
                    return true;
                }
                
                if(PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredMobileParty != null && (PlayerEncounter.EncounteredMobileParty.IsCaravan || PlayerEncounter.EncounteredMobileParty.IsVillager) && (PartyBase.MainParty.MapFaction != PlayerEncounter.EncounteredParty.MapFaction))
                {
                    return true;
                }
                return PlayerEncounter.EncounteredParty != null && PartyBase.MainParty.MapFaction.IsAtWarWith(PlayerEncounter.EncounteredParty.MapFaction);
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage("MBFastDialogue generated an exception " + ex.Message, Color.Black));
            }
            return false;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenu(
                FastDialogueSubModule.FastEncounterMenu,
                "{=!}{ENCOUNTER_TEXT}",
                Init,
#if v110 || v111 || v112 || v113 || v114 || v115 || v116 || v120 || v221 || v122 || v123 || v124 || v125 || v126 || v127 || v128 || v129 || v1210 || v1211 || v1212
GameOverlays.MenuOverlayType.Encounter,                
#else
                GameMenu.MenuOverlayType.Encounter,
#endif
                //GameMenu.MenuFlags.None,
                relatedObject: null);
            campaignGameStarter.AddGameMenuOption(
                FastDialogueSubModule.FastEncounterMenu,
                $"{FastDialogueSubModule.FastEncounterMenu}_attack",
                "{=o1pZHZOF}Attack!",
                args => ShouldShowWarOptions() && MenuHelper.EncounterAttackCondition(args),
                (args) =>
                {
                    MenuHelper.EncounterAttackConsequence(args);
                },
                //ConsequenceOf("game_menu_encounter_attack_on_consequence"),
                isLeave: false,
                index: -1,
                isRepeatable: false);
            campaignGameStarter.AddGameMenuOption(
                FastDialogueSubModule.FastEncounterMenu,
                $"{FastDialogueSubModule.FastEncounterMenu}_troops",
                "{=QfMeoKOm}Send troops.",
                (args) =>
                {
                    return ShouldShowWarOptions() && ReflectionUtils.ForceCall<bool>(GetGlobalCampaignBehaviorManager(), "game_menu_encounter_order_attack_on_condition", new object[] { args });
                },
                (args) =>
                {
                    MenuHelper.EncounterOrderAttackConsequence(args);
                },
                //ConsequenceOf("game_menu_encounter_order_attack_on_consequence"),
                isLeave: false,
                index: -1,
                isRepeatable: false);
            campaignGameStarter.AddGameMenuOption(
                FastDialogueSubModule.FastEncounterMenu,
                $"{FastDialogueSubModule.FastEncounterMenu}_getaway",
                "{=qNgGoqmI}Try to get away.",
                ConditionOf("game_menu_encounter_leave_your_soldiers_behind_on_condition"),
                ConsequenceOf("game_menu_encounter_leave_your_soldiers_behind_accept_on_consequence"),
                isLeave: false,
                index: -1,
                isRepeatable: false);
            campaignGameStarter.AddGameMenuOption(
                FastDialogueSubModule.FastEncounterMenu,
                $"{FastDialogueSubModule.FastEncounterMenu}_talk",
                "{=OPhlqUVl}Talk",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
                    return PlayerEncounter.Current != null || PlayerEncounter.EncounteredParty != null;
                },
                _ =>
                {
                    try
                    {
                        if (PlayerEncounter.Current == null && PlayerEncounter.EncounteredParty != null)
                        {
                            PlayerEncounter.RestartPlayerEncounter(
                                PlayerEncounter.EncounteredParty,
                                PartyBase.MainParty
                            );
                        }

                        PlayerEncounter.DoMeeting();
                    }
                    catch (Exception ex)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"Fast Dialogue: Error starting conversation - {ex.Message}", 
                            Colors.Red
                        ));
                    }
                },
                isLeave: false,
                index: -1,
                isRepeatable: false);
            campaignGameStarter.AddGameMenuOption(
                FastDialogueSubModule.FastEncounterMenu,
                $"{FastDialogueSubModule.FastEncounterMenu}_surrend",
                "{=3nT5wWzb}Surrender.",
                ConditionOf("game_menu_encounter_surrender_on_condition"),
                _ =>
                {
                    PlayerEncounter.PlayerSurrender = true;
                    PlayerEncounter.Update();
                },
                isLeave: false,
                index: -1,
                isRepeatable: false);
            campaignGameStarter.AddGameMenuOption(
                FastDialogueSubModule.FastEncounterMenu,
                $"{FastDialogueSubModule.FastEncounterMenu}_leave",
                "{=2YYRyrOO}Leave...",
                ConditionOf("game_menu_encounter_leave_on_condition"),
#if v110 || v111 || v112 || v113 || v114 || v115 || v116
                (args) =>
#else
            _ =>
#endif
                {
#if v110 || v111 || v112 || v113 || v114 || v115 || v116
                    MenuHelper.EncounterLeaveConsequence(args);
#else
                    MenuHelper.EncounterLeaveConsequence();
#endif
                    if (PartyBase.MainParty.IsMobile && PartyBase.MainParty.MobileParty != null)
                    {
                        PartyBase.MainParty.MobileParty.SetDisorganized(false);
                    }
                },
                true,
                index: -1,
                isRepeatable: false);
        }
    }
}