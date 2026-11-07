using System;
using Silk;
using Logger = Silk.Logger;

namespace Seasons
{
    public enum Season
    {
        Auto,
        Halloween,
        Christmas,
        Off,
    }

    public static class SeasonConfig
    {
        private const string Key = "Season";

        // Cached: the game asks every time a seasonal object starts and every time the lobby or
        // victory music starts.
        public static Season Current { get; private set; } = Season.Auto;

        public static void Load()
        {
            string raw = Config.GetModConfigValue(SeasonsMod.ModId, Key, "Auto");
            Current = Parse(raw);
            if (!string.Equals(raw?.Trim(), Current.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogWarning($"[Seasons] Unknown Season '{raw}' in the settings file; using Auto. Valid: Auto, Halloween, Christmas, Off.");
            }
        }

        public static void Set(Season season)
        {
            Current = season;
            Config.SetModConfigValue(SeasonsMod.ModId, Key, season.ToString());
            Logger.LogInfo($"[Seasons] Seasonal theme set to {Describe(season)}.");
        }

        public static Season Parse(string raw)
        {
            if (!string.IsNullOrWhiteSpace(raw))
            {
                foreach (Season season in Enum.GetValues(typeof(Season)))
                {
                    if (string.Equals(raw.Trim(), season.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return season;
                    }
                }
            }
            return Season.Auto;
        }

        public static string Describe(Season season)
        {
            switch (season)
            {
                case Season.Halloween: return "Halloween";
                case Season.Christmas: return "Christmas";
                case Season.Off: return "Off (no seasonal theme)";
                default: return "Auto (the game's dates)";
            }
        }
    }
}
