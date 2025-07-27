using Mahjong;
using Mahjong.HandCalculating.YakuList;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests;

public class PinfuTests
{
    [Fact]
    public void TestPinfu_WithValidSequencesAndNoValuedPair_ShouldReturnTrue()
    {
        // Arrange: 123m 456p 789s 234m 55p (all sequences, non-valued pair)
        var pinfu = new Pinfu();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 1, 2, 3 },    // 234m
            new() { 13, 13 }      // 55p (non-valued pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("平和（成功例：順子のみ）", hand);

        // Act
        var result = pinfu.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestPinfu_WithTriplet_ShouldReturnFalse()
    {
        // Arrange: 111m 456p 789s 234m 55p (contains triplet)
        var pinfu = new Pinfu();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m (triplet)
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 1, 2, 3 },    // 234m
            new() { 13, 13 }      // 55p
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("平和（失敗例：刻子混入）", hand);

        // Act
        var result = pinfu.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestPinfu_WithValuedPair_ShouldReturnFalse()
    {
        // Arrange: 123m 456p 789s 234m 東東 (valued pair)
        var pinfu = new Pinfu();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },    // 123m
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 1, 2, 3 },    // 234m
            new() { 27, 27 }      // 東東 (valued pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("平和（失敗例：役牌対子）", hand);

        // Act
        var result = pinfu.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestPinfu_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var pinfu = new Pinfu();

        // Act & Assert
        Assert.Equal("Pinfu", pinfu.Name);
        Assert.Equal(0, pinfu.HanOpen);
        Assert.Equal(1, pinfu.HanClosed);
        Assert.False(pinfu.IsYakuman);
    }
}