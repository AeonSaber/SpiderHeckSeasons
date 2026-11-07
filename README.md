# SpiderHeck Seasons

Choose SpiderHeck's seasonal theme any day of the year: Halloween, Christmas, no theme, or the game's own dates.

A mod for SpiderHeck by AeonS. Sister mods: [SpiderHeck Jukebox](https://github.com/AeonSaber/SpiderHeckJukebox), [SpiderHeck Palette](https://github.com/AeonSaber/SpiderHeckPalette).

## Features

- **Pick the season.** Auto, Halloween, Christmas or Off, from the mod's page in Silk's mod menu.
- **Everything the game ties to the season.** The lobby music, the victory music and the seasonal decorations all follow the setting.
- **Applies straight away.** Decorations already on screen switch over, and if the lobby music is playing it restarts with the new theme.
- **Auto changes nothing.** In Auto the game's own date check runs: Halloween Oct 24 - Nov 2, Christmas Dec 1 - Jan 7.
- **Client-side only.** It sends nothing over the network. Other players do not need the mod, and each player sees their own theme.

## Requirements

- SpiderHeck with the [Silk](https://github.com/SilkModding/Silk) mod loader, 0.7.0 or later

No other mods required. Don't use it together with Silk's "Make It Christmas" mod: both patch the same Christmas check.

## Installation

1. Install Silk 0.7.0 or later for SpiderHeck.
2. Download `SpiderHeckSeasons.dll` from the [releases page](https://github.com/AeonSaber/SpiderHeckSeasons/releases).
3. Put it in `SpiderHeck\Silk\Mods\` (inside the game folder, usually `C:\Program Files (x86)\Steam\steamapps\common\SpiderHeck`).
4. Start the game, open Silk's mod list, choose **SpiderHeckSeasons** and pick a theme.

## Configuration

The menu writes `Silk\Config\Mods\SpiderHeckSeasons.yaml` (created on first launch). One key:

- `Season`: `Auto`, `Halloween`, `Christmas` or `Off`. Anything else is read as Auto, with a warning in the Silk log.

```yaml
Season: Halloween
```

## How it works

The game decides its theme only in `SeasonChecker.IsItHalloween` and `SeasonChecker.IsItChristmas`, which compare today's date with the ranges above. Their only callers are `AudioController.PlayLobbyTheme`, `AudioController.PlayVictoryTheme` and `SeasonalSwitch.Start`, which turns on a level object's Halloween, Christmas or normal version.

- Harmony prefixes on both checks return the chosen season, or let the game's check run in Auto.
- After a change, the mod calls `SeasonalSwitch.Start` again on every switch in the loaded scenes and restarts the lobby theme if it is playing.
- A postfix on `Announcer.ModsPopup` builds the settings page in Silk's mod menu.

## Building from source

```
dotnet build -c Release
```

Requires the .NET SDK. Target framework: `net472`. Output: `bin\Release\net472\SpiderHeckSeasons.dll`.

The project references DLLs in a `lib\` folder, which is not in the repository (the game's and Silk's files are not redistributed). Copy them from `SpiderHeck\Silk\Library\`: Silk.dll, 0Harmony.dll, YamlDotNet.dll, NewAssembly.dll, MyScriptAssembly.dll, UnityEngine.CoreModule.dll, UnityEngine.AudioModule.dll, UnityEngine.UI.dll, Unity.TextMeshPro.dll, Unity.Netcode.Runtime.dll, Unity.Collections.dll, Unity.Mathematics.dll, DoozyEngine.dll, I2.dll.

## Credits

Built on [Silk](https://github.com/SilkModding/Silk) and [Harmony](https://github.com/pardeike/Harmony). Silk's "Make It Christmas" mod showed the Christmas check.

## License

[MIT](LICENSE)
