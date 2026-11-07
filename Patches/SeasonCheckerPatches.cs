using HarmonyLib;

namespace Seasons.Patches
{
    // The game decides its seasonal theme only through these two date checks
    // (Halloween: Oct 24 - Nov 2, Christmas: Dec 1 - Jan 7). Their only callers are
    // AudioController.PlayLobbyTheme, AudioController.PlayVictoryTheme and SeasonalSwitch.Start.
    // Halloween is checked first everywhere, so forcing one season means answering "no" to the other.

    [HarmonyPatch(typeof(SeasonChecker), nameof(SeasonChecker.IsItHalloween))]
    public static class IsItHalloweenPatch
    {
        public static bool Prefix(ref bool __result)
        {
            var season = SeasonConfig.Current;
            if (season == Season.Auto)
            {
                return true; // the game's own date check
            }

            __result = season == Season.Halloween;
            return false;
        }
    }

    [HarmonyPatch(typeof(SeasonChecker), nameof(SeasonChecker.IsItChristmas))]
    public static class IsItChristmasPatch
    {
        public static bool Prefix(ref bool __result)
        {
            var season = SeasonConfig.Current;
            if (season == Season.Auto)
            {
                return true;
            }

            __result = season == Season.Christmas;
            return false;
        }
    }
}
