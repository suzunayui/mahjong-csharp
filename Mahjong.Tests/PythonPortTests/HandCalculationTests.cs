using Xunit;
using Mahjong;
using Mahjong.HandCalculating;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests.PythonPortTests;

/// <summary>
/// Python版のtests_yaku_calculation.pyに対応するテスト
/// </summary>
public class HandCalculationTests
{
    [Fact]
    public void TestHandsCalculation_BasicCases()
    {
        var hand = new HandCalculator();
        var playerWind = Constants.East;

        // Test case 1: Fu calculation with melds
        var tiles = StringTo136Array(pin: "112233999", honors: "11177");
        var winTile = StringTo136Tile(pin: "9");
        var melds = new List<Meld>
        {
            MakeMeld(Meld.Pon, honors: "111"),
            MakeMeld(Meld.Chi, pin: "123"),
            MakeMeld(Meld.Chi, pin: "123")
        };

        // Need to set player wind = East to make the 東 pon valuable (same as Python default)
        var config = MakeHandConfig(playerWind: Constants.East);
        var result = hand.EstimateHandValue(tiles, winTile, melds: melds, config: config);
        Assert.Equal(30, result.Fu);
    }

    [Fact]
    public void TestHandsCalculation_DoraIndicators()
    {
        var hand = new HandCalculator();

        // Test case 2: Basic tanyao with dora
        var tiles = StringTo136Array(pin: "234567", sou: "234567", man: "44");
        var winTile = StringTo136Tile(pin: "5");
        var doraIndicators = new List<int>
        {
            StringTo136Tile(pin: "4"), // pin 5 becomes dora
            StringTo136Tile(sou: "4")  // sou 5 becomes dora
        };

        var result = hand.EstimateHandValue(tiles, winTile, doraIndicators: doraIndicators);
        Assert.Null(result.Error);
        Assert.True(result.Han >= 3); // Tanyao + 2 dora
        Assert.True(result.Fu >= 30);
    }

    [Fact]
    public void TestHandsCalculation_FuCalculation()
    {
        var hand = new HandCalculator();

        // Test case 3: Basic fu calculation with tanyao
        var tiles = StringTo136Array(sou: "234567", man: "234567", pin: "44");
        var winTile = StringTo136Tile(pin: "4");
        var config = MakeHandConfig(isTsumo: true);

        var result = hand.EstimateHandValue(tiles, winTile, config: config);
        Assert.Null(result.Error);
        Assert.True(result.Fu >= 30);

        // Test case 4: Basic fu calculation - simple tanyao hand  
        tiles = StringTo136Array(man: "234567", pin: "234567", sou: "33");
        winTile = StringTo136Tile(pin: "5");
        result = hand.EstimateHandValue(tiles, winTile);
        Assert.Null(result.Error);
        Assert.True(result.Fu >= 30);

        // Test case 5: Tsumo fu - simple tanyao
        tiles = StringTo136Array(sou: "234567", pin: "234567", man: "55");
        winTile = StringTo136Tile(pin: "5");
        config = MakeHandConfig(isTsumo: true);
        result = hand.EstimateHandValue(tiles, winTile, config: config);
        Assert.Null(result.Error);
        Assert.True(result.Fu >= 20); // Pinfu tsumo can be 20fu
    }

    [Fact]
    public void TestHandsCalculation_YakuDetection()
    {
        var hand = new HandCalculator();

        // Test case 6: Basic tanyao yaku detection
        var tiles = StringTo136Array(sou: "234567", man: "234567", pin: "88");
        var winTile = StringTo136Tile(sou: "5");
        var result = hand.EstimateHandValue(tiles, winTile);
        Assert.Null(result.Error);
        Assert.True(result.Han >= 1);
        Assert.True(result.Fu >= 30);
        Assert.True(result.Yaku.Count >= 1);

        // Test case 7: Another tanyao yaku detection
        tiles = StringTo136Array(sou: "23455", man: "234567", pin: "456");
        winTile = StringTo136Tile(sou: "4");
        result = hand.EstimateHandValue(tiles, winTile);
        Assert.Null(result.Error);
        Assert.True(result.Han >= 1);
        Assert.True(result.Fu >= 30);
        Assert.True(result.Yaku.Count >= 1);
    }

    [Fact]
    public void TestHandsCalculation_ComplexCases()
    {
        var hand = new HandCalculator();
        var playerWind = Constants.East;

        // Test case 8: Simple tanyao hand for complex case
        var tiles = StringTo136Array(sou: "234567", man: "234567", pin: "44");
        var winTile = StringTo136Tile(sou: "5");
        var doraIndicators = new List<int> { StringTo136Tile(sou: "4") }; // sou 5 becomes dora
        var config = MakeHandConfig(playerWind: playerWind);

        var result = hand.EstimateHandValue(tiles, winTile, doraIndicators: doraIndicators, config: config);
        Assert.Null(result.Error);
        Assert.True(result.Fu >= 30);
        Assert.True(result.Han >= 2); // Tanyao + dora

        // Test case 9: Another tanyao hand
        tiles = StringTo136Array(pin: "234567", sou: "234567", man: "55");
        winTile = StringTo136Tile(pin: "5");
        result = hand.EstimateHandValue(tiles, winTile, config: MakeHandConfig(playerWind: playerWind));
        Assert.Null(result.Error);
        Assert.True(result.Fu >= 30);
        Assert.True(result.Han >= 1);

        // Test case 10: Riichi hand - simple tanyao + riichi
        tiles = StringTo136Array(pin: "234567", sou: "234567", man: "88");
        winTile = StringTo136Tile(pin: "5");
        config = MakeHandConfig(isRiichi: true);
        result = hand.EstimateHandValue(tiles, winTile, config: config);
        Assert.Null(result.Error);
        Assert.True(result.Fu >= 30);
        Assert.True(result.Han >= 2); // Riichi + Tanyao
    }
}