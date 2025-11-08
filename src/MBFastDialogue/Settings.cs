using System.Collections.Generic;
using System.Xml.Serialization;

namespace MBFastDialogue
{
    public class Settings
    {
        [XmlElement("pattern_whitelist")]
        public Whitelist Whitelist { get; set; } = new Whitelist();

        [XmlElement]
        public string ToggleKey { get; set; } = "X";
    }

    public class Whitelist
    {
        [XmlElement("pattern")]
        public List<string> WhitelistPatterns { get; set; } = new List<string>();
    }
}
