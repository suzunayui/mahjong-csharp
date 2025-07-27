using Mahjong;
using Mahjong.HandCalculating.YakuList.Yakuman;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests.Yakuman;

public class TsuisouTests
{
    [Fact]
    public void TestTsuisou_WithOnlyHonorTiles_ShouldReturnTrue()
    {
        // Arrange: 東東東 南南南 西西西 北北 白白 (all honor tiles)
        var tsuisou = new Tsuisou();
        var hand = new List<List<int>>
        {
            new() { 27, 27, 27 }, // 東東東 (east pon)
            new() { 28, 28, 28 }, // 南南南 (south pon)
            new() { 29, 29, 29 }, // 西西西 (west pon)
            new() { 30, 30 },     // 北北 (north pair)
            new() { 31, 31 }      // 白白 (haku pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("字一色（成功例：字牌のみ）", hand);

        // Act
        var result = tsuisou.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestTsuisou_WithNumberTiles_ShouldReturnFalse()
    {
        // Arrange: 111m 東東東 南南南 西西西 北北 (contains number tiles)
        var tsuisou = new Tsuisou();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m (number tiles - should fail)
            new() { 27, 27, 27 }, // 東東東 (east pon)
            new() { 28, 28, 28 }, // 南南南 (south pon)
            new() { 29, 29, 29 }, // 西西西 (west pon)
            new() { 30, 30 }      // 北北 (north pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("字一色（失敗例：数牌混入）", hand);

        // Act
        var result = tsuisou.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestTsuisou_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var tsuisou = new Tsuisou();

        // Act & Assert
        Assert.Equal("Tsuisou", tsuisou.Name);
        Assert.Equal(13, tsuisou.HanOpen);
        Assert.Equal(13, tsuisou.HanClosed);
        Assert.True(tsuisou.IsYakuman);
    }
}
