using Mahjong;
using Mahjong.HandCalculating.YakuList.Yakuman;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests.Yakuman;

public class DaisangenTests
{
    [Fact]
    public void TestDaisangen_WithAllThreeDragons_ShouldReturnTrue()
    {
        // Arrange: 111m 白白白 發發發 中中中 22p (all three dragon pon sets)
        var daisangen = new Daisangen();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m
            new() { 31, 31, 31 }, // 白白白 (haku pon)
            new() { 32, 32, 32 }, // 發發發 (hatsu pon)
            new() { 33, 33, 33 }, // 中中中 (chun pon)
            new() { 10, 10 }      // 22p (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("大三元（成功例：三元牌全刻子）", hand);

        // Act
        var result = daisangen.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestDaisangen_WithOnlyTwoDragons_ShouldReturnFalse()
    {
        // Arrange: 111m 白白白 發發發 222p 33s (only 2 dragon pon sets)
        var daisangen = new Daisangen();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m
            new() { 31, 31, 31 }, // 白白白 (haku pon)
            new() { 32, 32, 32 }, // 發發發 (hatsu pon)
            new() { 10, 10, 10 }, // 222p (pon, not dragon)
            new() { 20, 20 }      // 33s (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("大三元（失敗例：三元牌2つのみ）", hand);

        // Act
        var result = daisangen.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestDaisangen_WithDragonPair_ShouldReturnFalse()
    {
        // Arrange: 111m 白白白 發發發 222p 中中 (dragon pair instead of pon)
        var daisangen = new Daisangen();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m
            new() { 31, 31, 31 }, // 白白白 (haku pon)
            new() { 32, 32, 32 }, // 發發發 (hatsu pon)
            new() { 10, 10, 10 }, // 222p
            new() { 33, 33 }      // 中中 (chun pair, not pon)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("大三元（失敗例：中が雀頭のみ）", hand);

        // Act
        var result = daisangen.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestDaisangen_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var daisangen = new Daisangen();

        // Act & Assert
        Assert.Equal("Daisangen", daisangen.Name);
        Assert.Equal(13, daisangen.HanOpen);
        Assert.Equal(13, daisangen.HanClosed);
        Assert.True(daisangen.IsYakuman);
    }
}
