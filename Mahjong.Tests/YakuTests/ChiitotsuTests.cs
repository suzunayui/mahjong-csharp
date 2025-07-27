using Mahjong;
using Mahjong.HandCalculating.YakuList;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests;

public class ChiitotsuTests
{
    [Fact]
    public void TestChiitoitsu_WithSevenPairs_ShouldReturnTrue()
    {
        // Arrange: 11m 33m 55p 77p 99s 東東 白白 (seven pairs)
        var chiitoitsu = new Chiitoitsu();
        var hand = new List<List<int>>
        {
            new() { 0, 0 },    // 11m
            new() { 2, 2 },    // 33m
            new() { 13, 13 },  // 55p
            new() { 15, 15 },  // 77p
            new() { 26, 26 },  // 99s
            new() { 27, 27 },  // 東東
            new() { 31, 31 }   // 白白
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("七対子（成功例）", hand);

        // Act
        var result = chiitoitsu.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestChiitoitsu_WithSequence_ShouldReturnFalse()
    {
        // Arrange: 123m 44m 55p 77p 99s 東東 白白 (contains sequence)
        var chiitoitsu = new Chiitoitsu();
        var hand = new List<List<int>>
        {
            new() { 0, 1, 2 },  // 123m (sequence)
            new() { 3, 3 },     // 44m
            new() { 13, 13 },   // 55p
            new() { 15, 15 },   // 77p
            new() { 26, 26 },   // 99s
            new() { 27, 27 },   // 東東
            new() { 31, 31 }    // 白白
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("七対子（失敗例：順子混入）", hand);

        // Act
        var result = chiitoitsu.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestChiitoitsu_WithTriplet_ShouldReturnFalse()
    {
        // Arrange: 111m 33m 55p 77p 99s 東東 白白 (contains triplet)
        var chiitoitsu = new Chiitoitsu();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },  // 111m (triplet)
            new() { 2, 2 },     // 33m
            new() { 13, 13 },   // 55p
            new() { 15, 15 },   // 77p
            new() { 26, 26 },   // 99s
            new() { 27, 27 },   // 東東
            new() { 31, 31 }    // 白白
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("七対子（失敗例：刻子混入）", hand);

        // Act
        var result = chiitoitsu.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestChiitoitsu_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var chiitoitsu = new Chiitoitsu();

        // Act & Assert
        Assert.Equal("Chiitoitsu", chiitoitsu.Name);
        Assert.Equal(0, chiitoitsu.HanOpen);
        Assert.Equal(2, chiitoitsu.HanClosed);
        Assert.False(chiitoitsu.IsYakuman);
    }
}