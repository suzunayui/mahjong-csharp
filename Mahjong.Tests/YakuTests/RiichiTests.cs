using Mahjong;
using Mahjong.HandCalculating.YakuList;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests;

public class RiichiTests
{
    [Fact]
    public void TestRiichi_WithRiichiCondition_ShouldReturnTrue()
    {
        // Arrange: Any hand with riichi declared
        var riichi = new Riichi();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 1, 2, 3 },    // 234m
            new() { 13, 13 }      // 55p
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("立直（成功例）", hand);

        // Act
        var result = riichi.IsConditionMet(hand);

        // Assert
        Assert.True(result); // Riichi is always valid when declared
    }

    [Fact]
    public void TestRiichi_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var riichi = new Riichi();

        // Act & Assert
        Assert.Equal("Riichi", riichi.Name);
        Assert.Equal(0, riichi.HanOpen);
        Assert.Equal(1, riichi.HanClosed);
        Assert.False(riichi.IsYakuman);
    }
}