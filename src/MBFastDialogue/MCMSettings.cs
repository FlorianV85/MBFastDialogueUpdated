using System.Collections.Generic;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace MBFastDialogue
{
    internal sealed class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        private const string GeneralGroup = "{=MBFastDialogue_General}General"; 
        private const string WhitelistGroup = "{=MBFastDialogue_Whitelist}Whitelist";
        
        public override string Id => "MBFastDialogue";
        public override string DisplayName => "Fast Dialogue";
        public override string FolderName => "MBFastDialogue";
        public override string FormatType => "json";

        [SettingPropertyBool(
            "{=MBFastDialogue_EnableMod}Enable Fast Dialogue",
            Order = 0,
            RequireRestart = false,
            HintText = "{=MBFastDialogue_EnableMod_Hint}Enable or disable Fast Dialogue mod. Can also be toggled with CTRL + configured key."
            )]
        [SettingPropertyGroup(GeneralGroup, GroupOrder = 0)]
        public bool EnableMod { get; set; } = true;

        [SettingPropertyDropdown(
            "{=MBFastDialogue_ToggleKey}Toggle Hotkey",
            Order = 1,
            RequireRestart = false,
            HintText = "{=MBFastDialogue_ToggleKey_Hint}Press CTRL + this key to toggle Fast Dialogue on/off in game.")]
        [SettingPropertyGroup(GeneralGroup, GroupOrder = 0)]
        public Dropdown<string> ToggleKey { get; set; } = new Dropdown<string>(new string[]
        {
            "X", "Z", "C", "V", "B", "N", "M",
            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12"
        }, selectedIndex: 0);

        [SettingPropertyText(
            "{=MBFastDialogue_WhitelistPatterns}Whitelist Patterns (comma separated)",
            Order = 0,
            RequireRestart = false,
            HintText = "{=MBFastDialogue_WhitelistPatterns_Hint}Party IDs containing these patterns will use Fast Dialogue. Leave empty to allow all.")]
        [SettingPropertyGroup(WhitelistGroup, GroupOrder = 2)]
        public string WhitelistPatterns { get; set; } = "";

        public List<string> GetWhitelistPatternsList()
        {
            if (string.IsNullOrWhiteSpace(WhitelistPatterns))
                return new List<string>();

            var patterns = new List<string>();
            foreach (var pattern in WhitelistPatterns.Split(','))
            {
                var trimmed = pattern.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    patterns.Add(trimmed);
            }
            return patterns;
        }
    }
}