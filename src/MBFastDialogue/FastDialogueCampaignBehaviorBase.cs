using System;
using HarmonyLib.BUTR.Extensions;
using Helpers;
using MBFastDialogue.Constants;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace MBFastDialogue
{
    public class FastDialogueCampaignBehaviorBase : EncounterGameMenuBehavior
    {
        private delegate void GameMenuEncounterOnInitDelegate(MenuCallbackArgs args);
        private delegate bool EncounterLeaveSoldiersBehindConditionDelegate(MenuCallbackArgs args);
        private delegate void EncounterLeaveSoldiersBehindConsequenceDelegate(
            MenuCallbackArgs args
        );
        private delegate bool EncounterSurrenderConditionDelegate(MenuCallbackArgs args);
        private delegate bool EncounterLeaveConditionDelegate(MenuCallbackArgs args);

        private GameMenuEncounterOnInitDelegate? _gameEncounterOnInit;
        private EncounterSurrenderConditionDelegate? _encounterSurrenderCondition;
        private EncounterLeaveConditionDelegate? _encounterLeaveCondition;
        private EncounterLeaveSoldiersBehindConditionDelegate? _encounterLeaveSoldiersBehindCondition;
        private EncounterLeaveSoldiersBehindConsequenceDelegate? _encounterLeaveSoldiersBehindConsequence;

        private void Init(MenuCallbackArgs args)
        {
            _gameEncounterOnInit?.Invoke(args);

            var current = PlayerEncounter.Current;
            var encountered = PlayerEncounter.EncounteredParty;

            if (current == null && encountered != null)
            {
                PlayerEncounter.RestartPlayerEncounter(encountered, PartyBase.MainParty);
            }
        }

        private bool ShouldShowWarOptions()
        {
            try
            {
                var encountered = PlayerEncounter.EncounteredParty;
                if (encountered == null)
                    return false;
                var partyId = encountered.Id;

                if (partyId.Contains(PartyIds.QuestPartyTemplate))
                    return true;

                var mobile = PlayerEncounter.EncounteredMobileParty;
                if (mobile != null)
                {
                    var stringId = mobile.StringId;

                    if (
                        stringId.Contains(PartyIds.Conspiracy)
                        || stringId.Contains(PartyIds.Conspirator)
                    )
                        return true;

                    if (
                        (mobile.IsCaravan || mobile.IsVillager)
                        && PartyBase.MainParty.MapFaction != encountered.MapFaction
                    )
                        return true;
                }
                return PartyBase.MainParty.MapFaction.IsAtWarWith(encountered.MapFaction);
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"MBFastDialogue Exception: {ex.Message}", Color.Black)
                );
                return false;
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            var encounterGameMenuBehavior =
                Campaign.Current.GetCampaignBehavior<EncounterGameMenuBehavior>();

            _gameEncounterOnInit = AccessTools2.GetDelegate<GameMenuEncounterOnInitDelegate>(
                encounterGameMenuBehavior,
                encounterGameMenuBehavior.GetType(),
                "game_menu_encounter_on_init"
            );
            _encounterSurrenderCondition =
                AccessTools2.GetDelegate<EncounterSurrenderConditionDelegate>(
                    encounterGameMenuBehavior,
                    encounterGameMenuBehavior.GetType(),
                    "game_menu_encounter_surrender_on_condition"
                );
            _encounterLeaveCondition = AccessTools2.GetDelegate<EncounterLeaveConditionDelegate>(
                encounterGameMenuBehavior,
                encounterGameMenuBehavior.GetType(),
                "game_menu_encounter_leave_on_condition"
            );
            _encounterLeaveSoldiersBehindCondition =
                AccessTools2.GetDelegate<EncounterLeaveSoldiersBehindConditionDelegate>(
                    encounterGameMenuBehavior,
                    encounterGameMenuBehavior.GetType(),
                    "game_menu_encounter_leave_your_soldiers_behind_on_condition"
                );
            _encounterLeaveSoldiersBehindConsequence =
                AccessTools2.GetDelegate<EncounterLeaveSoldiersBehindConsequenceDelegate>(
                    encounterGameMenuBehavior,
                    encounterGameMenuBehavior.GetType(),
                    "game_menu_encounter_leave_your_soldiers_behind_accept_on_consequence"
                );

            const string menuId = ModuleConstants.FastEncounterMenu;
            starter.AddGameMenu(
                menuId,
                "{=!}{ENCOUNTER_TEXT}",
                Init,
                GameMenu.MenuOverlayType.Encounter,
                relatedObject: null
            );
            AddMenuOptions(starter, menuId);
        }

        private void AddMenuOptions(CampaignGameStarter starter, string menuId)
        {
            starter.AddGameMenuOption(
                menuId,
                $"{menuId}_attack",
                "{=o1pZHZOF}Attack!",
                args => ShouldShowWarOptions() && MenuHelper.EncounterAttackCondition(args),
                MenuHelper.EncounterAttackConsequence,
                isLeave: false,
                index: -1,
                isRepeatable: false
            );

            starter.AddGameMenuOption(
                menuId,
                $"{menuId}_troops",
                "{=QfMeoKOm}Send troops.",
                args => ShouldShowWarOptions() && MenuHelper.EncounterOrderAttackCondition(args),
                MenuHelper.EncounterOrderAttackConsequence,
                isLeave: false,
                index: -1,
                isRepeatable: false
            );

            starter.AddGameMenuOption(
                menuId,
                $"{menuId}_getaway",
                "{=qNgGoqmI}Try to get away.",
                args => _encounterLeaveSoldiersBehindCondition?.Invoke(args) ?? false,
                args => _encounterLeaveSoldiersBehindConsequence?.Invoke(args),
                isLeave: false,
                index: -1,
                isRepeatable: false
            );

            starter.AddGameMenuOption(
                menuId,
                $"{menuId}_talk",
                "{=OPhlqUVl}Talk",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
                    return PlayerEncounter.Current != null
                        || PlayerEncounter.EncounteredParty != null;
                },
                OnTalkConsequence,
                isLeave: false,
                index: -1,
                isRepeatable: false
            );

            starter.AddGameMenuOption(
                menuId,
                $"{menuId}_surrend",
                "{=3nT5wWzb}Surrender.",
                args => _encounterSurrenderCondition?.Invoke(args) ?? false,
                _ =>
                {
                    PlayerEncounter.PlayerSurrender = true;
                    PlayerEncounter.Update();
                },
                isLeave: false,
                index: -1,
                isRepeatable: false
            );

            starter.AddGameMenuOption(
                menuId,
                $"{menuId}_leave",
                "{=2YYRyrOO}Leave...",
                args => _encounterLeaveCondition?.Invoke(args) ?? false,
                OnLeaveConsequence,
                isLeave: true,
                index: -1,
                isRepeatable: false
            );
        }

        private void OnTalkConsequence(MenuCallbackArgs args)
        {
            try
            {
                var current = PlayerEncounter.Current;
                var encountered = PlayerEncounter.EncounteredParty;

                if (current == null && encountered != null)
                {
                    PlayerEncounter.RestartPlayerEncounter(encountered, PartyBase.MainParty);
                }

                PlayerEncounter.DoMeeting();
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(
                        $"Fast Dialogue: Conversation error - {ex.Message}",
                        Colors.Red
                    )
                );
            }
        }

        private void OnLeaveConsequence(MenuCallbackArgs args)
        {
            MenuHelper.EncounterLeaveConsequence();
            var mobile = PartyBase.MainParty.MobileParty;
            if (mobile != null)
            {
                mobile.SetDisorganized(false);
            }
        }
    }
}
