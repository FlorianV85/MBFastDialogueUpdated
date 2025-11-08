using System.Collections.Generic;

namespace MBFastDialogue
{
    public class Settings
    {
        public Whitelist whitelist { get; set; } = new Whitelist();
        public string toggle_key { get; set; } = "X";    
            
        public static Settings FromMCM()
        {
            var mcm = MCMSettings.Instance;
            return new Settings
            {
                toggle_key = mcm?.ToggleKey.SelectedValue,
                whitelist = new Whitelist
                {
                    whitelistPatterns = mcm?.GetWhitelistPatternsList()
                }
            };
        }
    }
    
    public class Whitelist
    {
        public List<string> whitelistPatterns { get; set; } = new List<string>();
    }
}
