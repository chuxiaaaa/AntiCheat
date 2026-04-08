using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    [HarmonyWrapSafe]
    public static class GameNetworkManagerLobbyMetadataPatch
    {
        [HarmonyPatch("StartHost")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void StartHost()
        {
            if (string.IsNullOrWhiteSpace(PluginConfig.Prefix.Value))
            {
                return;
            }

            var setting = GameNetworkManager.Instance.lobbyHostSettings;
            string rawText = setting.lobbyName
                .Replace('【', '[')
                .Replace('】', ']')
                .Replace('［', '[')
                .Replace('］', ']');

            List<string> labels = new List<string>();
            var match = Regex.Match(rawText, "^\\[(.*?)\\]");
            if (match.Success)
            {
                var text = match.Groups[1].Value;
                labels.AddRange(text.Split('/'));
                rawText = rawText.Remove(0, match.Groups[0].Value.Length).TrimStart();
            }

            if (!labels.Any(x => x == PluginConfig.Prefix.Value))
            {
                labels.Add(PluginConfig.Prefix.Value);
            }

            setting.lobbyName = "[" + string.Join("/", labels) + "] " + rawText;
        }
    }
}
