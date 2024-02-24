using AntiCheat.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AntiCheat
{
    public static class LocalizationManager
    {
        private static ResourceManager resourceManager;

        public static Dictionary<string, Type> Languages = new Dictionary<string, Type>
        {
            { "zh_cn", typeof(zh_CN) },
            { "en_us", typeof(en_US) }
        };

        public static void SetLanguage(string language)
        {
            if (Languages.ContainsKey(language))
            {
                resourceManager = new ResourceManager(Languages[language]);
            }
            else
            {
                resourceManager = new ResourceManager(typeof(zh_CN));
            }
        }

        public static string TryGetString(string prefix, string key)
        {
            try
            {
                string value = resourceManager.GetString(prefix + key);
                return value == null ? key : value.TrimEnd();
            }
            catch (Exception)
            {
                return key;
            }
        }

        public static string GetString(string key,Dictionary<string,string> replaces)
        {
            try
            {
                var value = resourceManager.GetString(key);
                foreach (var item in replaces)
                {
                    value = value.Replace(item.Key, item.Value);
                }
                return value;
            }
            catch (Exception)
            {
                return "Missing translation for key: " + key;
            }
        }

        public static string GetString(string key)
        {
            try
            {
                return resourceManager.GetString(key);
            }
            catch (Exception)
            {
                return "Missing translation for key: " + key;
            }
        }
    }
}
