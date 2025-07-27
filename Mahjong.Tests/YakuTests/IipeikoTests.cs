using Mahjong;
using Mahjong.HandCalculating.YakuList;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests;

public class IipeikoTests
{
    [Fact]
    public void TestIipeiko_WithTwoIdenticalChi_ShouldReturnTrue()
    {
        // Arrange: 123m 123m 456p 789s 11z (two identical 123m chi sets)
        var iipeiko = new Iipeiko();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m (first chi)
            new() { 0, 1, 2 },    // 123m (second chi - identical)
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 27, 27 }      // 11z (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("一盃口（成功例：同一順子×2）", hand);

        // Act
        var result = iipeiko.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestIipeiko_WithoutIdenticalChi_ShouldReturnFalse()
    {
        // Arrange: 123m 456m 789p 111s 22z (no identical chi sets)
        var iipeiko = new Iipeiko();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m
            new() { 3, 4, 5 },    // 456m
            new() { 15, 16, 17 }, // 789p
            new() { 18, 18, 18 }, // 111s (pon, not chi)
            new() { 28, 28 }      // 22z (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("一盃口（失敗例：同一順子なし）", hand);

        // Act
        var result = iipeiko.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestIipeiko_WithPonSets_ShouldReturnFalse()
    {
        // Arrange: 111m 222m 333p 444s 55z (all pon sets, no chi)
        var iipeiko = new Iipeiko();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m (pon)
            new() { 1, 1, 1 },    // 222m (pon)
            new() { 11, 11, 11 }, // 333p (pon)
            new() { 21, 21, 21 }, // 444s (pon)
            new() { 31, 31 }      // 55z (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("一盃口（失敗例：刻子のみ）", hand);

        // Act
        var result = iipeiko.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestIipeiko_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var iipeiko = new Iipeiko();

        // Act & Assert
        Assert.Equal("Iipeiko", iipeiko.Name);
        Assert.Null(iipeiko.HanOpen);  // Only closed hands
        Assert.Equal(1, iipeiko.HanClosed);
        Assert.False(iipeiko.IsYakuman);
    }
}
