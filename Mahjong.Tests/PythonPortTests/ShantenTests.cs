using Xunit;
using Mahjong;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests.PythonPortTests;

/// <summary>
/// Python版のtests_shanten.pyに対応するテスト
/// </summary>
public class ShantenTests
{
    [Fact]
    public void TestShantenNumber()
    {
        var shanten = new Shanten();

        // Test case 1: AGARI state - 111234567s 11p 567m
        var tiles = StringTo34Array(sou: "111234567", pin: "11", man: "567");
        Assert.Equal(Shanten.AgariState, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 2: 0 shanten - 111345677s 11p 567m
        tiles = StringTo34Array(sou: "111345677", pin: "11", man: "567");
        Assert.Equal(0, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 3: 1 shanten - 111345677s 15p 567m
        tiles = StringTo34Array(sou: "111345677", pin: "15", man: "567");
        Assert.Equal(1, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 4: 2 shanten - 11134567s 15p 1578m
        tiles = StringTo34Array(sou: "11134567", pin: "15", man: "1578");
        Assert.Equal(2, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 5: 3 shanten - 113456s 1358p 1358m
        tiles = StringTo34Array(sou: "113456", pin: "1358", man: "1358");
        Assert.Equal(3, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 6: 4 shanten - 1589s 13588p 1358m 1z
        tiles = StringTo34Array(sou: "1589", pin: "13588", man: "1358", honors: "1");
        Assert.Equal(4, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 7: 5 shanten - 159s 13588p 1358m 12z
        tiles = StringTo34Array(sou: "159", pin: "13588", man: "1358", honors: "12");
        Assert.Equal(5, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 8: 6 shanten - 1589s 258p 1358m 123z
        tiles = StringTo34Array(sou: "1589", pin: "258", man: "1358", honors: "123");
        Assert.Equal(6, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 9: AGARI state - 11123456788999s
        tiles = StringTo34Array(sou: "11123456788999");
        Assert.Equal(Shanten.AgariState, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 10: 0 shanten - 11122245679999s
        tiles = StringTo34Array(sou: "11122245679999");
        Assert.Equal(0, shanten.CalculateShantenForRegularHand(tiles));

        // Test case 11: 2 shanten using general calculate method - 4566677s 1367p 8m 12z
        tiles = StringTo34Array(sou: "4566677", pin: "1367", man: "8", honors: "12");
        Assert.Equal(2, shanten.CalculateShanten(tiles));
    }

    [Fact]
    public void TestChitoitsuShanten()
    {
        var shanten = new Shanten();

        // Perfect chitoitsu - Python: TilesConverter.string_to_34_array(sou="114477", pin="114477", man="77")
        var tiles = StringTo34Array(sou: "114477", pin: "114477", man: "77");
        var result = shanten.CalculateShanten(tiles, useChiitoitsu: true, useKokushi: false);
        Assert.Equal(Shanten.AgariState, result);

        // 0 shanten chitoitsu - Python: TilesConverter.string_to_34_array(sou="114477", pin="114477", man="76")
        tiles = StringTo34Array(sou: "114477", pin: "114477", man: "76");
        result = shanten.CalculateShanten(tiles, useChiitoitsu: true, useKokushi: false);
        Assert.Equal(0, result);

        // 1 shanten chitoitsu - Python: TilesConverter.string_to_34_array(sou="114477", pin="114479", man="76")
        tiles = StringTo34Array(sou: "114477", pin: "114479", man: "76");
        result = shanten.CalculateShanten(tiles, useChiitoitsu: true, useKokushi: false);
        Assert.Equal(1, result);
    }

    [Fact]
    public void TestKokushiShanten()
    {
        var shanten = new Shanten();

        // Perfect kokushi - Python: TilesConverter.string_to_34_array(sou="19", pin="19", man="19", honors="12345677")
        var tiles = StringTo34Array(sou: "19", pin: "19", man: "19", honors: "12345677");
        var result = shanten.CalculateShanten(tiles, useChiitoitsu: false, useKokushi: true);
        Assert.Equal(Shanten.AgariState, result);

        // 0 shanten kokushi - Python: TilesConverter.string_to_34_array(sou="129", pin="19", man="19", honors="1234567")
        tiles = StringTo34Array(sou: "129", pin: "19", man: "19", honors: "1234567");
        result = shanten.CalculateShanten(tiles, useChiitoitsu: false, useKokushi: true);
        Assert.Equal(0, result);

        // 1 shanten kokushi - Python: TilesConverter.string_to_34_array(sou="129", pin="129", man="19", honors="123456")
        tiles = StringTo34Array(sou: "129", pin: "129", man: "19", honors: "123456");
        result = shanten.CalculateShanten(tiles, useChiitoitsu: false, useKokushi: true);
        Assert.Equal(1, result);
    }
}