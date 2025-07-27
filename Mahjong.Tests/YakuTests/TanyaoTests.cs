using Mahjong;
using Mahjong.HandCalculating.YakuList;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests;

public class TanyaoTests
{
    [Fact]
    public void TestTanyao_WithOnlyMiddleTiles_ShouldReturnTrue()
    {
        // Fix: use middle number tiles only
        var tanyao = new Tanyao();
        var hand = new List<List<int>>
        {
            new() { 1, 2, 3 },    // 234m
            new() { 12, 13, 14 }, // 345p
            new() { 21, 22, 23 }, // 456s
            new() { 4, 5, 6 },    // 567m
            new() { 7, 7 }        // 88m (middle tile pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("断么九（成功例：中張牌のみ）", hand);

        // Act
        var result = tanyao.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestTanyao_WithTerminalTiles_ShouldReturnFalse()
    {
        // Arrange: 123m 345p 456s 789m 22p (contains 1 and 9 - terminals)
        var tanyao = new Tanyao();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m (contains 1m - terminal)
            new() { 12, 13, 14 }, // 345p
            new() { 21, 22, 23 }, // 456s
            new() { 6, 7, 8 },    // 789m (contains 9m - terminal)
            new() { 10, 10 }      // 22p (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("断么九（失敗例：么九牌混入）", hand);

        // Act
        var result = tanyao.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestTanyao_WithHonorTiles_ShouldReturnFalse()
    {
        // Arrange: 234m 345p 456s 567m 東東 (contains honor tiles)
        var tanyao = new Tanyao();
        var hand = new List<List<int>>
        {
            new() { 1, 2, 3 },    // 234m
            new() { 12, 13, 14 }, // 345p
            new() { 21, 22, 23 }, // 456s
            new() { 4, 5, 6 },    // 567m
            new() { 27, 27 }      // 東東 (honor tile pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("断么九（失敗例：字牌混入）", hand);

        // Act
        var result = tanyao.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestTanyao_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var tanyao = new Tanyao();

        // Act & Assert
        Assert.Equal("Tanyao", tanyao.Name);
        Assert.Equal(1, tanyao.HanOpen);
        Assert.Equal(1, tanyao.HanClosed);
        Assert.False(tanyao.IsYakuman);
    }
}
