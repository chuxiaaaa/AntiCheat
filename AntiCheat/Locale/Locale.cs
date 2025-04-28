using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiCheat.Locale
{
    public class Locale
    {
        public Dictionary<string,string> Config { get; set; }
        public Dictionary<string, string> Item { get; set; }
        public Dictionary<string, string> Log { get; set; }
        public Dictionary<string,string> Message { get; set; }
        public Dictionary<string,string> OperationLog { get; set; }
        public string Prefix { get; set; }
        public string MessageFormat { get; set; }
    }

    public class LocaleCfg
    {
        public string current_language { get; set; }

        public Dictionary<string,string> support_language { get; set; }
    }
}
