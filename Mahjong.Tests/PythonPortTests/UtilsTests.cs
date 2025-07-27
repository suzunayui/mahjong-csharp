using Xunit;
using Mahjong;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests.PythonPortTests;

/// <summary>
/// Python版のtests_utils.pyに対応するテスト
/// </summary>
public class UtilsTests
{
    [Fact]
    public void TestFindIsolatedTiles()
    {
        // Python: hand_34 = TilesConverter.string_to_34_array(sou="1369", pin="15678", man="25", honors="124")
        var hand34 = StringTo34Array(sou: "1369", pin: "15678", man: "25", honors: "124");
        var isolatedTiles = Utils.FindIsolatedTileIndices(hand34);

        // 索子のテスト
        Assert.DoesNotContain(StringTo34Tile(sou: "1"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "2"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "3"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "4"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "5"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "6"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "7"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "8"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(sou: "9"), isolatedTiles);

        // 筒子のテスト
        Assert.DoesNotContain(StringTo34Tile(pin: "1"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "2"), isolatedTiles);
        Assert.Contains(StringTo34Tile(pin: "3"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "4"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "5"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "6"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "7"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "8"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(pin: "9"), isolatedTiles);

        // 萬子のテスト
        Assert.DoesNotContain(StringTo34Tile(man: "1"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(man: "2"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(man: "3"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(man: "4"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(man: "5"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(man: "6"), isolatedTiles);
        Assert.Contains(StringTo34Tile(man: "7"), isolatedTiles);
        Assert.Contains(StringTo34Tile(man: "8"), isolatedTiles);
        Assert.Contains(StringTo34Tile(man: "9"), isolatedTiles);

        // 字牌のテスト
        Assert.DoesNotContain(StringTo34Tile(honors: "1"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(honors: "2"), isolatedTiles);
        Assert.Contains(StringTo34Tile(honors: "3"), isolatedTiles);
        Assert.DoesNotContain(StringTo34Tile(honors: "4"), isolatedTiles);
        Assert.Contains(StringTo34Tile(honors: "5"), isolatedTiles);
        Assert.Contains(StringTo34Tile(honors: "6"), isolatedTiles);
        Assert.Contains(StringTo34Tile(honors: "7"), isolatedTiles);
    }

    [Fact]
    public void TestIsStrictlyIsolatedTile()
    {
        // Python: hand_34 = TilesConverter.string_to_34_array(sou="1399", pin="1567", man="25", honors="1224")
        var hand34 = StringTo34Array(sou: "1399", pin: "1567", man: "25", honors: "1224");

        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "1")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "2")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "3")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "4")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "5")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "6")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "7")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "8")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(sou: "9")));

        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "1")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "2")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "3")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "4")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "5")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "6")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "7")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "8")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(pin: "9")));

        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "1")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "2")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "3")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "4")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "5")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "6")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "7")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "8")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(man: "9")));

        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "1")));
        Assert.False(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "2")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "3")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "4")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "5")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "6")));
        Assert.True(Utils.IsTileStrictlyIsolated(hand34, StringTo34Tile(honors: "7")));
    }
}