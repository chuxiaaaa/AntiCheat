using BepInEx.Configuration;

using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCheat
{
    internal class FeatureConfig
    {
        private ConfigEntry<bool> _Enable { get; }
        private ConfigEntry<bool>? _Kick { get; }

        public bool Enable => _Enable.Value;

        public bool Kick => _Kick?.Value ?? false;

        public FeatureConfig(
            ConfigFile config,
            string section,
            string enableDesc,
            bool defaultEnable = true,
            bool hasKick = true)
        {
            _Enable = config.Bind(section,
                "Enable",
                defaultEnable,
                enableDesc);

            if (hasKick)
            {
                _Kick = config.Bind(section,
                    "Kick",
                    false,
                    localizationManager.Cfg_GetString("Kick"));
            }
        }
    }
}
