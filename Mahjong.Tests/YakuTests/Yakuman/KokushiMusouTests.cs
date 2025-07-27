using Mahjong;
using Mahjong.HandCalculating.YakuList.Yakuman;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests.Yakuman;

public class KokushiMusouTests
{
    [Fact]
    public void TestKokushiMusou_WithAllTerminalsAndHonors_ShouldReturnTrue()
    {
        // Arrange: 19m 19p 19s 東南西北白發中 + one duplicate
        var kokushi = new KokushiMusou();
        var hand = new List<List<int>>
        {
            new() { 0, 8, 9, 17, 18, 26, 27, 28, 29, 30, 31, 32, 33, 33 } // Kokushi hand
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("国士無双（成功例）", hand);

        // Act
        var result = kokushi.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestKokushiMusou_WithMissingTerminal_ShouldReturnFalse()
    {
        // Arrange: Missing one terminal/honor tile
        var kokushi = new KokushiMusou();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m (not kokushi pattern)
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 27, 28, 29 }, // 東南西
            new() { 30, 30 }      // 北北
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("国士無双（失敗例：么九牌不足）", hand);

        // Act
        var result = kokushi.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestKokushiMusou_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var kokushi = new KokushiMusou();

        // Act & Assert
        Assert.Equal("Kokushi Musou", kokushi.Name);
        Assert.Equal(0, kokushi.HanOpen);
        Assert.Equal(13, kokushi.HanClosed);
        Assert.True(kokushi.IsYakuman);
    }
}