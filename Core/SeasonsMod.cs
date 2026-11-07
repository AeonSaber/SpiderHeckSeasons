using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Silk;
using Logger = Silk.Logger;

namespace Seasons
{
    [SilkMod(ModName, new string[] { "AeonS" }, "0.1.0", "0.7.0", ModName, 1)]
    public class SeasonsMod : SilkMod
    {
        // Silk shows this name in its mod list and passes it to Announcer.ModsPopup as the menu
        // title. It is also the settings file name: Silk\Config\Mods\SpiderHeckSeasons.yaml.
        public const string ModName = "SpiderHeckSeasons";
        public const string ModId = ModName;
        private const string HarmonyId = "com.AeonS.SpiderHeckSeasons";

        private static string _version;
        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    _version = $"{version.Major}.{version.Minor}.{version.Build}";
                }
                return _version;
            }
        }

        public override void Initialize()
        {
            Logger.LogInfo($"Initializing {ModName} v{Version}...");

            Config.LoadModConfig(ModId, new Dictionary<string, object>
            {
                { "Season", "Auto" },
            });
            SeasonConfig.Load();

            new Harmony(HarmonyId).PatchAll();
            Logger.LogInfo($"[Seasons] Patches applied. Seasonal theme: {SeasonConfig.Describe(SeasonConfig.Current)}.");

            // Mods may load after the first scene has already picked its theme, so apply the
            // chosen one to anything already showing.
            if (SeasonConfig.Current != Season.Auto)
            {
                SeasonRefresher.Apply();
            }
        }

        public override void Unload()
        {
            Logger.LogInfo($"Unloading {ModName}...");
            new Harmony(HarmonyId).UnpatchSelf();
            SeasonRefresher.Apply(); // back to the game's own choice
        }
    }
}
