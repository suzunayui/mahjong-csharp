using Mahjong;
using Xunit;

namespace Mahjong.Tests;

public class UtilsTests
{
    [Fact]
    public void TestUtils_IsHonor_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsHonor(27));  // East
        Assert.True(Utils.IsHonor(33));  // Red
        Assert.False(Utils.IsHonor(0));  // 1m
        Assert.False(Utils.IsHonor(26)); // 9s
    }

    [Fact]
    public void TestUtils_IsTerminal_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsTerminal(0));   // 1m
        Assert.True(Utils.IsTerminal(8));   // 9m
        Assert.True(Utils.IsTerminal(9));   // 1p
        Assert.True(Utils.IsTerminal(17));  // 9p
        Assert.True(Utils.IsTerminal(18));  // 1s
        Assert.True(Utils.IsTerminal(26));  // 9s
        
        Assert.False(Utils.IsTerminal(1));  // 2m
        Assert.False(Utils.IsTerminal(4));  // 5m
        Assert.False(Utils.IsTerminal(27)); // East
    }

    [Fact]
    public void TestUtils_IsMan_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsMan(0));   // 1m
        Assert.True(Utils.IsMan(4));   // 5m
        Assert.True(Utils.IsMan(8));   // 9m
        
        Assert.False(Utils.IsMan(9));  // 1p
        Assert.False(Utils.IsMan(18)); // 1s
        Assert.False(Utils.IsMan(27)); // East
    }

    [Fact]
    public void TestUtils_IsPin_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsPin(9));   // 1p
        Assert.True(Utils.IsPin(13));  // 5p
        Assert.True(Utils.IsPin(17));  // 9p
        
        Assert.False(Utils.IsPin(0));  // 1m
        Assert.False(Utils.IsPin(18)); // 1s
        Assert.False(Utils.IsPin(27)); // East
    }

    [Fact]
    public void TestUtils_IsSou_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsSou(18));  // 1s
        Assert.True(Utils.IsSou(22));  // 5s
        Assert.True(Utils.IsSou(26));  // 9s
        
        Assert.False(Utils.IsSou(0));  // 1m
        Assert.False(Utils.IsSou(9));  // 1p
        Assert.False(Utils.IsSou(27)); // East
    }

    [Fact]
    public void TestUtils_Simplify_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.Equal(0, Utils.Simplify(0));   // 1m
        Assert.Equal(4, Utils.Simplify(4));   // 5m
        Assert.Equal(8, Utils.Simplify(8));   // 9m
        Assert.Equal(0, Utils.Simplify(9));   // 1p -> 0 (relative to pin)
        Assert.Equal(4, Utils.Simplify(13));  // 5p -> 4 (relative to pin)
        Assert.Equal(8, Utils.Simplify(17));  // 9p -> 8 (relative to pin)
        Assert.Equal(0, Utils.Simplify(18));  // 1s -> 0 (relative to sou)
        Assert.Equal(4, Utils.Simplify(22));  // 5s -> 4 (relative to sou)
        Assert.Equal(8, Utils.Simplify(26));  // 9s -> 8 (relative to sou)
    }

    [Fact]
    public void TestUtils_IsChi_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsChi(new[] { 0, 1, 2 }));   // 123m
        Assert.True(Utils.IsChi(new[] { 9, 10, 11 })); // 123p
        
        Assert.False(Utils.IsChi(new[] { 0, 0, 0 }));  // 111m (triplet)
        Assert.False(Utils.IsChi(new[] { 0, 2, 4 }));  // 135m (not sequence)
        Assert.False(Utils.IsChi(new[] { 0, 1 }));     // Too few tiles
    }

    [Fact]
    public void TestUtils_IsPon_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(Utils.IsPon(new[] { 0, 0, 0 }));   // 111m
        Assert.True(Utils.IsPon(new[] { 27, 27, 27 })); // 東東東
        
        Assert.False(Utils.IsPon(new[] { 0, 1, 2 }));  // 123m (sequence)
        Assert.False(Utils.IsPon(new[] { 0, 0 }));     // Too few tiles
        Assert.False(Utils.IsPon(new[] { 0, 0, 0, 0 })); // Too many tiles
    }
}