using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Seasons.Patches
{
    // Silk opens a mod's settings page with Announcer.ModsPopup(<mod name>); this fills in ours.
    // Buttons update their own labels in place instead of rebuilding the page: destroying the
    // clicked button from its own onClick throws NullReferenceException and DOTween warnings.
    [HarmonyPatch(typeof(Announcer), nameof(Announcer.ModsPopup))]
    public static class MenuPatches
    {
        private static readonly Season[] Choices = { Season.Auto, Season.Halloween, Season.Christmas, Season.Off };

        public static void Postfix(string title, ref ModsMenuPopup __result)
        {
            if (title == SeasonsMod.ModName && __result != null)
            {
                Build(__result);
            }
        }

        private static void Build(ModsMenuPopup menu)
        {
            menu.CreateParagraph("<size=200%>\n<b><color=#FFD700>Seasons</color></b></size>");
            var status = LastText(menu, () => menu.CreateParagraph(StatusText()));
            menu.CreateParagraph("<size=95%><color=#AAAAAA>Choose the game's seasonal theme any day of the year: the lobby and victory music and the seasonal decorations.</color></size>");
            Spacer(menu);

            var labels = new Dictionary<Season, TMPro.TMP_Text>();
            foreach (var choice in Choices)
            {
                var season = choice;
                labels[season] = AddButton(menu, ButtonText(season), () =>
                {
                    SeasonConfig.Set(season);
                    SeasonRefresher.Apply();

                    if (status != null)
                    {
                        status.text = StatusText();
                    }
                    foreach (var pair in labels)
                    {
                        if (pair.Value != null)
                        {
                            pair.Value.text = ButtonText(pair.Key);
                        }
                    }
                });
            }

            Spacer(menu);
            menu.CreateParagraph("<size=95%><color=#AAAAAA>Auto follows the game's dates: Halloween Oct 24 - Nov 2, Christmas Dec 1 - Jan 7. Only your game changes; other players see their own theme.</color></size>");
        }

        private static string StatusText()
        {
            var season = SeasonConfig.Current;
            string now = season != Season.Auto ? SeasonConfig.Describe(season) : $"Auto (today: {GameSeasonToday()})";
            return $"<size=115%><b>Theme:</b> <color=#00FF00>{now}</color></size>";
        }

        // What the game itself shows today. Only meaningful while the setting is Auto, when the
        // patches let the game's own date checks run.
        private static string GameSeasonToday()
        {
            if (SeasonChecker.IsItHalloween())
            {
                return "Halloween";
            }
            return SeasonChecker.IsItChristmas() ? "Christmas" : "no seasonal theme";
        }

        private static string ButtonText(Season season)
        {
            string mark = SeasonConfig.Current == season ? "[x] " : "[  ] ";
            return mark + SeasonConfig.Describe(season);
        }

        private static TMPro.TMP_Text AddButton(ModsMenuPopup menu, string label, Action onClick)
        {
            int before = ChildCount(menu);
            menu.CreateButton(label, () => onClick());
            return ResizeLastCreatedButton(menu, before);
        }

        private static TMPro.TMP_Text LastText(ModsMenuPopup menu, Action create)
        {
            int before = ChildCount(menu);
            create();
            if (menu.objectParent == null || menu.objectParent.childCount <= before)
            {
                return null;
            }
            return menu.objectParent.GetChild(menu.objectParent.childCount - 1).GetComponentInChildren<TMPro.TMP_Text>();
        }

        private static void Spacer(ModsMenuPopup menu)
        {
            menu.CreateParagraph("<size=100%>\n </size>");
        }

        private static int ChildCount(ModsMenuPopup menu)
        {
            return menu.objectParent != null ? menu.objectParent.childCount : 0;
        }

        // Same button sizing as SpiderHeck Jukebox and Palette.
        private static TMPro.TMP_Text ResizeLastCreatedButton(ModsMenuPopup menu, int childCountBefore)
        {
            if (menu == null || menu.objectParent == null || menu.objectParent.childCount <= childCountBefore)
            {
                return null;
            }

            var button = menu.objectParent.GetChild(menu.objectParent.childCount - 1).GetComponent<Button>();
            if (button == null)
            {
                return null;
            }

            var rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x * 1.1f, rect.sizeDelta.y * 1.45f);
            }

            var label = button.GetComponentInChildren<TMPro.TMP_Text>();
            if (label != null)
            {
                label.fontSize *= 1.2f;
            }
            return label;
        }
    }
}
