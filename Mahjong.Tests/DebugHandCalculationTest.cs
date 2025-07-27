using Xunit;
using Mahjong;
using Mahjong.HandCalculating;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests;

public class DebugHandCalculationTest
{
    [Fact]
    public void TestBasicHandCalculationDebug()
    {
        var hand = new HandCalculator();

        // Debug: Test case 1: Fu calculation with melds
        // Python: tiles = TilesConverter.string_to_136_array(pin="112233999", honors="11177")
        // Python: win_tile = _string_to_136_tile(pin="9") 
        var tiles = StringTo136Array(pin: "112233999", honors: "11177");
        var winTile = StringTo136Tile(pin: "9");
        var melds = new List<Meld>
        {
            MakeMeld(Meld.Pon, honors: "111"),
            MakeMeld(Meld.Chi, pin: "123"),
            MakeMeld(Meld.Chi, pin: "123")
        };

        // Check input data
        Console.WriteLine($"Tiles count: {tiles.Count}");
        Console.WriteLine($"Tiles: {string.Join(", ", tiles)}");
        Console.WriteLine($"Win tile: {winTile}");
        Console.WriteLine($"Melds count: {melds.Count}");
        foreach (var meld in melds)
        {
            Console.WriteLine($"Meld: {meld.Type}, Tiles34: [{string.Join(", ", meld.Tiles34)}]");
        }
        Console.WriteLine($"Contains win tile: {tiles.Contains(winTile)}");

        // Check 34 array conversion
        var tiles34 = TilesConverter.To34Array(tiles);
        Console.WriteLine($"Tiles34: {string.Join(", ", tiles34)}");
        Console.WriteLine($"Total tiles in 34: {tiles34.Sum()}");

        // Check agari
        var agari = new Agari();
        var isAgari = agari.IsAgari(tiles34, melds.Select(m => m.Tiles34).ToList());
        Console.WriteLine($"Is Agari: {isAgari}");

        // Add config with player wind = East to make the 東 pon valuable
        var config = MakeHandConfig(playerWind: Constants.East);
        var result = hand.EstimateHandValue(tiles, winTile, melds: melds, config: config);
        Console.WriteLine($"Result error: {result.Error}");
        Console.WriteLine($"Result fu: {result.Fu}");
        Console.WriteLine($"Result han: {result.Han}");
        
        Assert.Equal(30, result.Fu);
    }
}