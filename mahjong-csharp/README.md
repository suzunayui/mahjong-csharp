# Mahjong C# Library

This is a C# port of the Python mahjong library. It provides functionality for:

- Mahjong hand calculation and scoring
- Yaku (winning patterns) detection
- Shanten (steps to winning) calculation
- Tile conversion utilities

## Current Status

This library is a work in progress, ported from the Python version. The following features have been implemented:

### Core Features
- ✅ Tile representation and conversion (136/34 tile format)
- ✅ Agari (winning hand) detection
- ✅ Shanten calculation (regular, chiitoitsu, kokushi)
- ✅ Basic hand calculation and scoring

### Implemented Yaku (Winning Patterns)

#### Situational Yaku
- ✅ Tsumo (self-draw)
- ✅ Riichi
- ✅ Daburu Riichi (double riichi)
- ✅ Nagashi Mangan

#### 1-Han Yaku
- ✅ Tanyao (all simples)
- ✅ Pinfu (basic hand structure)
- ✅ Iipeiko (identical sequences)
- ✅ Yakuhai - Haku (white dragon)
- ✅ Yakuhai - Hatsu (green dragon)
- ✅ Yakuhai - Chun (red dragon)

#### 2-Han Yaku
- ✅ Chiitoitsu (seven pairs)
- ✅ Sanshoku Doujun (three-color straight)
- ✅ Ittsu (straight)
- ✅ Chantai (mixed terminals)
- ✅ Toitoi (all triplets)
- ✅ Sanankou (three concealed triplets)
- ✅ Sankantsu (three quads)
- ✅ Sanshoku Doukou (three-color triplets)
- ✅ Honroto (all terminals and honors)
- ✅ Shosangen (small three dragons)

#### 3-Han Yaku
- ✅ Ryanpeiko (two identical sequences)
- ✅ Honitsu (half flush)
- ✅ Junchan (pure terminals)

#### 6-Han Yaku
- ✅ Chinitsu (full flush)

#### Yakuman
- ✅ Kokushi Musou (thirteen terminals)
- ✅ Daburu Kokushi Musou (double thirteen terminals)

#### Other
- ✅ Dora (bonus tiles)
- ✅ Aka Dora (red fives)

### To Be Implemented

#### Missing Yaku
- ⏳ Wind/seat yaku (East, South, West, North)
- ⏳ Haitei/Houtei (last tile/discard)
- ⏳ Rinshan Kaihou (after kan)
- ⏳ Chankan (robbing a kan)
- ⏳ Ippatsu (one shot)
- ⏳ Many more yakuman patterns (Daisangen, Suukantsu, etc.)

#### Missing Features
- ⏳ Advanced hand divider for complex yaku detection
- ⏳ Fu (minipoints) calculation improvements
- ⏳ Meld handling improvements
- ⏳ Full wind/seat yaku support
- ⏳ Special scoring situations

## Usage Example

```csharp
using Mahjong;
using Mahjong.HandCalculating;

// Parse a hand from string notation
var tiles = TilesConverter.OneLineStringTo136Array("123m456p789s1122z");

// Calculate shanten
var shanten = new Shanten();
var tiles34 = TilesConverter.To34Array(tiles);
var shantenValue = shanten.CalculateShanten(tiles34);

// Check if winning
var agari = new Agari();
var isWinning = agari.IsAgari(tiles34);

// Calculate hand value
var calculator = new HandCalculator();
var config = new HandConfig(isTsumo: true, isRiichi: true);
var winTile = tiles[0]; // Example win tile

var result = calculator.EstimateHandValue(tiles, winTile, config: config);
Console.WriteLine($"Han: {result.Han}, Fu: {result.Fu}");
```

## Building

```bash
dotnet build
dotnet run
```

## Original Python Library

This C# port is based on the Python mahjong library. The original can be found at the Python source directory in this repository.

## License

This project follows the same license as the original Python library.
