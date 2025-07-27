using Xunit;
using Mahjong;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests.PythonPortTests;

/// <summary>
/// Python版のtests_agari.pyに対応するテスト
/// </summary>
public class AgariTests
{
    [Fact]
    public void TestIsAgari()
    {
        var agari = new Agari();

        // Test case 1: 123456789s 123p 33m
        var tiles = StringTo34Array(sou: "123456789", pin: "123", man: "33");
        Assert.True(agari.IsAgari(tiles));

        // Test case 2: 123456789s 11123p
        tiles = StringTo34Array(sou: "123456789", pin: "11123");
        Assert.True(agari.IsAgari(tiles));

        // Test case 3: 123456789s 11777z
        tiles = StringTo34Array(sou: "123456789", honors: "11777");
        Assert.True(agari.IsAgari(tiles));

        // Test case 4: 12345556778899s
        tiles = StringTo34Array(sou: "12345556778899");
        Assert.True(agari.IsAgari(tiles));

        // Test case 5: 11123456788999s
        tiles = StringTo34Array(sou: "11123456788999");
        Assert.True(agari.IsAgari(tiles));

        // Test case 6: 233334s 789p 345m 55z
        tiles = StringTo34Array(sou: "233334", pin: "789", man: "345", honors: "55");
        Assert.True(agari.IsAgari(tiles));
    }

    [Fact]
    public void TestIsNotAgari()
    {
        var agari = new Agari();

        // Test case 1: 123456789s 12345p (not winning)
        var tiles = StringTo34Array(sou: "123456789", pin: "12345");
        Assert.False(agari.IsAgari(tiles));

        // Test case 2: 111222444s 11145p (not winning)
        tiles = StringTo34Array(sou: "111222444", pin: "11145");
        Assert.False(agari.IsAgari(tiles));

        // Test case 3: 11122233356888s (not winning)
        tiles = StringTo34Array(sou: "11122233356888");
        Assert.False(agari.IsAgari(tiles));
    }

    [Fact]
    public void TestIsChitoitsuAgari()
    {
        var agari = new Agari();

        // Test case 1: 1133557799s 1199p (7 pairs)
        var tiles = StringTo34Array(sou: "1133557799", pin: "1199");
        Assert.True(agari.IsAgari(tiles));

        // Test case 2: 2244s 1199p 11m 2277z (7 pairs)
        tiles = StringTo34Array(sou: "2244", pin: "1199", man: "11", honors: "2277");
        Assert.True(agari.IsAgari(tiles));

        // Test case 3: 11223344556677m (7 pairs)
        tiles = StringTo34Array(man: "11223344556677");
        Assert.True(agari.IsAgari(tiles));
    }

    [Fact]
    public void TestIsAgariWithOpenSets()
    {
        var agari = new Agari();

        // Python test: tiles = TilesConverter.string_to_34_array(sou="23455567", pin="222", man="345")
        // melds = [_string_to_open_34_set(man="345"), _string_to_open_34_set(sou="555")]
        // assert not agari.is_agari(tiles, melds)
        var tiles = StringTo34Array(sou: "23455567", pin: "222", man: "345");
        var openSets = new List<List<int>>
        {
            new List<int> { 3, 4, 5 },  // man="345" -> indices 3,4,5 in 34-array
            new List<int> { 22, 22, 22 } // sou="555" -> index 22 (5 of sou)
        };

        // This should NOT be agari according to Python test
        Assert.False(agari.IsAgari(tiles, openSets));
    }

    [Fact]
    public void TestKokushiMusou()
    {
        var agari = new Agari();

        // Test case: Kokushi musou pattern
        var tiles = StringTo34Array(sou: "19", pin: "19", man: "199", honors: "1234567");
        Assert.True(agari.IsAgari(tiles));

        // Test case: Kokushi musou with pair (East pair)
        tiles = StringTo34Array(sou: "19", pin: "19", man: "19", honors: "11234567");
        Assert.True(agari.IsAgari(tiles));

        // Test case: Kokushi musou with pair (North pair)
        tiles = StringTo34Array(sou: "19", pin: "19", man: "19", honors: "12345677");
        Assert.True(agari.IsAgari(tiles));
    }
}