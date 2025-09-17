
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using static AntiCheat.Core.AntiCheat;

namespace AntiCheat.Locale
{
    public class LocalizationManager
    {
        public LocalizationManager()
        {
            var dirPath = Path.GetDirectoryName(GetType().Assembly.Location);
            var localecfgPath = Path.Combine(dirPath, "locales", "localization_cfg.json");
            var json = File.ReadAllText(localecfgPath);
            var cfg = JsonConvert.DeserializeObject<LocaleCfg>(json);
            var langPath = Path.Combine(dirPath, "locales");
            if (string.IsNullOrWhiteSpace(cfg.current_language))
            {
                var lang = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower();
                if (lang.StartsWith("ko"))
                {
                    cfg.current_language = "ko_KR";
                }
                else if (lang.StartsWith("zh-"))
                {
                    cfg.current_language = "zh_CN";
                }
                else
                {
                    cfg.current_language = "en_US";
                }
                Core.AntiCheat.LogInfo($"no current language set, automatic language selection based on current region");
                Core.AntiCheat.LogInfo($"CurrentCulture:{lang},use language -> {cfg.current_language}");
            }
            current_language = cfg.current_language;
            json = File.ReadAllText($"{Path.Combine(langPath, cfg.current_language)}.json");
            locale = JsonConvert.DeserializeObject<Locale>(json);
        }

        public string current_language { get; set; }
        public Locale locale { get; set; }

        public string Prefix() => locale.Prefix;

        public string MessageFormat(Dictionary<string, string> pairs = null)
            => ReplacePlaceholders(locale.MessageFormat, pairs);

        public string OperationLog_GetString(string key, Dictionary<string, string> pairs = null)
            => GetStringWithFallback(locale.OperationLog, key, pairs, "OperationLog_GetString");

        public string Cfg_GetString(string key)
            => GetStringWithFallback(locale.Config, key, null, "Cfg_GetString");

        public string Item_GetString(string key)
            => GetStringWithFallback(locale.Item, key, null, "Item_GetString");

        public string Log_GetString(string key, Dictionary<string, string> pairs = null)
            => GetStringWithFallback(locale.Log, key, pairs, "Log_GetString");

        public string Msg_GetString(string key, Dictionary<string, string> pairs = null)
            => GetStringWithFallback(locale.Message, key, pairs, "Msg_GetString");

        private string GetStringWithFallback(Dictionary<string, string> dictionary, string key,
    Dictionary<string, string> pairs, string methodName)
        {
            if (dictionary.TryGetValue(key, out string value))
            {
                return ReplacePlaceholders(value, pairs);
            }
            return $"{methodName} fail -> {key}";
        }

        private string ReplacePlaceholders(string input, Dictionary<string, string> pairs)
        {
            if (pairs == null || string.IsNullOrEmpty(input))
                return input;
            return pairs.Aggregate(input, (current, item) => current.Replace(item.Key, item.Value));
        }
    }
}
