using System.Collections.Generic;

namespace MBFastDialogue.Constants
{
    internal static class ExcludeMenuOptions
    {
        public static readonly HashSet<string> Ids = new HashSet<string>
        {
            "continue_preparations", "village_raid_action", "village_force_volunteer_action",
            "village_force_supplies_action", "attack", "capture_the_enemy", "str_order_attack",
            "leave_soldiers_behind", "surrender", "leave", "go_back_to_settlement"
        };
    }
}