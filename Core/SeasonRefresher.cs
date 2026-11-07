using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Logger = Silk.Logger;

namespace Seasons
{
    // Re-applies the seasonal theme to what is already on screen, so a change takes effect without
    // reloading the level. The game only checks the season when a SeasonalSwitch starts and when
    // the lobby or victory music starts.
    public static class SeasonRefresher
    {
        private static readonly MethodInfo SwitchStart = AccessTools.Method(typeof(SeasonalSwitch), "Start");
        private static readonly FieldInfo InstanceField = AccessTools.Field(typeof(AudioController), "instance");
        private static readonly FieldInfo MusicSourceField = AccessTools.Field(typeof(AudioController), "musicSource");
        private static readonly FieldInfo LobbyThemeField = AccessTools.Field(typeof(AudioController), "lobbyTheme");
        private static readonly FieldInfo LobbyHalloweenField = AccessTools.Field(typeof(AudioController), "lobbyThemeHalloween");
        private static readonly FieldInfo LobbyChristmasField = AccessTools.Field(typeof(AudioController), "lobbyThemeChristmas");
        private static readonly MethodInfo PlayLobbyTheme = AccessTools.Method(typeof(AudioController), "PlayLobbyTheme", Type.EmptyTypes);

        public static void Apply()
        {
            RefreshSwitches();
            RefreshLobbyMusic();
        }

        private static void RefreshSwitches()
        {
            if (SwitchStart == null)
            {
                Logger.LogWarning("[Seasons] SeasonalSwitch.Start not found; decorations change at the next level load.");
                return;
            }

            int count = 0;
            try
            {
                // Includes switches on inactive objects; skips prefabs, which have no scene.
                foreach (var sw in Resources.FindObjectsOfTypeAll<SeasonalSwitch>())
                {
                    if (sw == null || !sw.gameObject.scene.IsValid())
                    {
                        continue;
                    }

                    SwitchStart.Invoke(sw, null);
                    count++;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"[Seasons] Could not refresh the seasonal decorations: {ex.Message}");
                return;
            }

            Logger.LogInfo($"[Seasons] Refreshed {count} seasonal object(s).");
        }

        // Restarts the lobby music only if a lobby theme is playing now. Battle and victory music
        // are left alone; the victory theme picks up the new season the next time it plays.
        private static void RefreshLobbyMusic()
        {
            try
            {
                if (InstanceField == null || MusicSourceField == null || PlayLobbyTheme == null)
                {
                    return;
                }

                var controller = InstanceField.GetValue(null) as AudioController;
                if (controller == null)
                {
                    return;
                }

                var source = MusicSourceField.GetValue(controller) as AudioSource;
                if (source == null || !source.isPlaying || source.clip == null)
                {
                    return;
                }

                var clip = source.clip;
                bool lobbyPlaying = clip == LobbyThemeField?.GetValue(controller) as AudioClip
                    || clip == LobbyHalloweenField?.GetValue(controller) as AudioClip
                    || clip == LobbyChristmasField?.GetValue(controller) as AudioClip;
                if (!lobbyPlaying)
                {
                    return;
                }

                PlayLobbyTheme.Invoke(controller, null);
                Logger.LogInfo($"[Seasons] Lobby music now '{source.clip?.name}'.");
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"[Seasons] Could not restart the lobby music: {ex.Message}");
            }
        }
    }
}
