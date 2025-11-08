using System.Collections.Generic;

namespace MBFastDialogue
{
    public class Settings
    {
        public Whitelist Whitelist { get; set; } = new Whitelist();
        
        public string ToggleKey { get; set; } = "X";
        
        public static Settings FromMCM()
        {
            var mcm = MCMSettings.Instance;
            return new Settings
            {
                ToggleKey = mcm?.ToggleKey.SelectedValue,
                Whitelist = new Whitelist
                {
                    WhitelistPatterns = mcm?.GetWhitelistPatternsList()
                }
            };
        }
    }

    public class Whitelist
    {
        public List<string> WhitelistPatterns { get; set; } = new List<string>();
    }
}
