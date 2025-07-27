using Mahjong;
using Mahjong.HandCalculating.YakuList.Yakuman;
using Mahjong.Tests.TestHelpers;
using Xunit;

namespace Mahjong.Tests.YakuTests.Yakuman;

public class SuuankouTests
{
    [Fact]
    public void TestSuuankou_WithFourConcealedTriplets_ShouldReturnTrue()
    {
        // Arrange: 111m 222p 333s 444m 55p (four concealed triplets)
        var suuankou = new Suuankou();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m
            new() { 10, 10, 10 }, // 222p
            new() { 20, 20, 20 }, // 333s
            new() { 3, 3, 3 },    // 444m
            new() { 13, 13 }      // 55p (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("四暗刻（成功例）", hand);

        // Act
        var result = suuankou.IsConditionMet(hand);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestSuuankou_WithOpenTriplet_ShouldReturnFalse()
    {
        // Arrange: Hand with open meld (pon)
        var suuankou = new Suuankou();
        var hand = new List<List<int>>
        {
            new() { 0, 0, 0 },    // 111m (concealed)
            new() { 10, 10, 10 }, // 222p (concealed)
            new() { 20, 20, 20 }, // 333s (concealed)
            new() { 0, 1, 2 },    // 123m (sequence, not triplet)
            new() { 13, 13 }      // 55p (pair)
        };

        // Display hand info
        HandTestHelper.DisplayHandInfo("四暗刻（失敗例：刻子不足）", hand);

        // Act
        var result = suuankou.IsConditionMet(hand);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TestSuuankou_Attributes_ShouldBeCorrect()
    {
        // Arrange
        var suuankou = new Suuankou();

        // Act & Assert
        Assert.Equal("Suu Ankou", suuankou.Name);
        Assert.Equal(0, suuankou.HanOpen);
        Assert.Equal(13, suuankou.HanClosed);
        Assert.True(suuankou.IsYakuman);
    }
}