using Xunit;
using Mahjong;
using Mahjong.Tests.TestHelpers;

namespace Mahjong.Tests.PythonPortTests;

/// <summary>
/// Python版のtests_tiles_converter.pyに対応するテスト
/// </summary>
public class TilesConverterTests
{
    [Fact]
    public void TestConvertToOneLineString()
    {
        // Python test: tiles = [0, 1, 34, 35, 36, 37, 70, 71, 72, 73, 106, 107, 108, 109, 133, 134]
        // Expected: "1199m1199p1199s1177z"
        var tiles = new List<int> { 0, 1, 34, 35, 36, 37, 70, 71, 72, 73, 106, 107, 108, 109, 133, 134 };
        var result = TilesConverter.ToOneLineString(tiles);
        Assert.Equal("1199m1199p1199s1177z", result);
    }

    [Fact]
    public void TestConvertToOneLineStringWithAkaDora()
    {
        // Python test: tiles = [1, 16, 13, 46, 5, 13, 24, 34, 134, 124]
        var tiles = new List<int> { 1, 16, 13, 46, 5, 13, 24, 34, 134, 124 };
        
        // Without aka dora: "1244579m3p57z"
        var result = TilesConverter.ToOneLineString(tiles, printAkaDora: false);
        Assert.Equal("1244579m3p57z", result);
        
        // With aka dora: "1244079m3p57z" (5m becomes 0m for red five)
        result = TilesConverter.ToOneLineString(tiles, printAkaDora: true);
        Assert.Equal("1244079m3p57z", result);
    }

    [Fact]
    public void TestConvertTo34Array()
    {
        // Python test: tiles = [0, 34, 35, 36, 37, 70, 71, 72, 73, 106, 107, 108, 109, 134]
        var tiles = new List<int> { 0, 34, 35, 36, 37, 70, 71, 72, 73, 106, 107, 108, 109, 134 };
        var result = TilesConverter.To34Array(tiles);
        
        Assert.Equal(1, result[0]);  // 1m
        Assert.Equal(2, result[8]);  // 9m (tiles 34, 35)
        Assert.Equal(2, result[9]);  // 1p (tiles 36, 37)
        Assert.Equal(2, result[17]); // 9p (tiles 70, 71)
        Assert.Equal(2, result[18]); // 1s (tiles 72, 73)
        Assert.Equal(2, result[26]); // 9s (tiles 106, 107)
        Assert.Equal(2, result[27]); // East (tiles 108, 109)
        Assert.Equal(1, result[33]); // Red Dragon (tile 134)
        Assert.Equal(14, result.Sum()); // Total 14 tiles
    }

    [Fact]
    public void TestStringToArrayConversions()
    {
        // Test string_to_136_array equivalent
        var tiles136 = TilesConverter.StringTo136Array(man: "123", pin: "456", sou: "789");
        Assert.Equal(9, tiles136.Count);
        
        // Test string_to_34_array equivalent
        var tiles34 = TilesConverter.StringTo34Array(man: "123", pin: "456", sou: "789");
        Assert.Equal(34, tiles34.Length);
        Assert.Equal(9, tiles34.Sum()); // Total should be 9 tiles
        
        // Verify specific positions
        Assert.Equal(1, tiles34[0]); // 1m
        Assert.Equal(1, tiles34[1]); // 2m
        Assert.Equal(1, tiles34[2]); // 3m
        Assert.Equal(1, tiles34[12]); // 4p
        Assert.Equal(1, tiles34[13]); // 5p
        Assert.Equal(1, tiles34[14]); // 6p
        Assert.Equal(1, tiles34[24]); // 7s
        Assert.Equal(1, tiles34[25]); // 8s
        Assert.Equal(1, tiles34[26]); // 9s
    }

    [Fact]
    public void TestHonorTileConversions()
    {
        // Test with honor tiles
        var tiles = TilesConverter.StringTo136Array(honors: "1234567");
        Assert.Equal(7, tiles.Count);
        
        var tiles34 = TilesConverter.StringTo34Array(honors: "1234567");
        Assert.Equal(1, tiles34[27]); // East
        Assert.Equal(1, tiles34[28]); // South
        Assert.Equal(1, tiles34[29]); // West
        Assert.Equal(1, tiles34[30]); // North
        Assert.Equal(1, tiles34[31]); // White
        Assert.Equal(1, tiles34[32]); // Green
        Assert.Equal(1, tiles34[33]); // Red
    }
}