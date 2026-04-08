using BepInEx.Configuration;

namespace AntiCheat
{
    internal class FeatureConfig
    {
        public ConfigEntry<bool> EnableEntry { get; }
        public ConfigEntry<bool> KickEntry { get; }

        public bool Enable => EnableEntry.Value;
        public bool Value => EnableEntry.Value;
        public bool Kick => KickEntry?.Value ?? false;

        public FeatureConfig(
            ConfigFile config,
            string section,
            string enableDesc,
            bool defaultEnable = true,
            bool hasKick = true)
        {
            EnableEntry = config.Bind(
                section,
                "Enable",
                defaultEnable,
                enableDesc);

            if (hasKick)
            {
                KickEntry = config.Bind(
                    section,
                    "Kick",
                    false,
                    AntiCheatPlugin.localizationManager.Cfg_GetString("Kick"));
            }
            else
            {
                KickEntry = null!;
            }
        }
    }
}
