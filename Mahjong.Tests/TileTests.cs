using Mahjong;
using Xunit;

namespace Mahjong.Tests;

public class TileTests
{
    [Fact]
    public void TestTile_Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var tile = new Tile(5, true);
        
        // Assert
        Assert.Equal(5, tile.Value);
        Assert.True(tile.IsTsumogiri);
    }

    [Fact]
    public void TestTilesConverter_ToOneLineString_ShouldFormatCorrectly()
    {
        // Arrange
        var tiles = new List<int> { 0, 4, 8, 36, 40, 44, 72, 76, 80 }; // 1m,5m,9m,1p,5p,9p,1s,5s,9s
        
        // Act
        var result = TilesConverter.ToOneLineString(tiles);
        
        // Assert
        Assert.Equal("159m159p159s", result);
    }

    [Fact]
    public void TestTilesConverter_To34Array_ShouldConvertCorrectly()
    {
        // Arrange
        var tiles = new List<int> { 0, 1, 2, 3 }; // Four 1m tiles
        
        // Act
        var result = TilesConverter.To34Array(tiles);
        
        // Assert
        Assert.Equal(4, result[0]); // Four 1m tiles
        Assert.Equal(0, result[1]); // No 2m tiles
    }

    [Fact]
    public void TestTilesConverter_StringTo136Array_ShouldParseCorrectly()
    {
        // Arrange & Act
        var result = TilesConverter.StringTo136Array(man: "123", pin: "456", sou: "789");
        
        // Assert
        Assert.Equal(9, result.Count);
        Assert.Contains(0, result);  // 1m
        Assert.Contains(4, result);  // 2m  
        Assert.Contains(8, result);  // 3m
        Assert.Contains(48, result); // 5p
        Assert.Contains(52, result); // 6p
        Assert.Contains(56, result); // 7p
        Assert.Contains(96, result); // 8s
        Assert.Contains(100, result);// 9s
        Assert.Contains(104, result);// 1s (next)
    }
}