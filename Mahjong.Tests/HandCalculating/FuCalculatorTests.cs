using Xunit;
using Mahjong.HandCalculating;
using System.Collections.Generic;
using System.Linq;

namespace Mahjong.Tests.HandCalculating
{
    /// <summary>
    /// 符計算のテストクラス
    /// </summary>
    public class FuCalculatorTests
    {
        private readonly FuCalculator _fuCalculator;

        public FuCalculatorTests()
        {
            _fuCalculator = new FuCalculator();
        }

        [Fact]
        public void TestBaseFu_ShouldReturn20()
        {
            // 基本的な手牌：234m 456p 789s 11z 22p
            var hand = new List<List<int>>
            {
                new() { 1, 2, 3 },    // 234m
                new() { 12, 13, 14 }, // 456p
                new() { 24, 25, 26 }, // 789s
                new() { 27, 27 },     // 11z (東東)
                new() { 10, 10 }      // 22p
            };

            var config = new HandConfig { IsTsumo = false };
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 10 * 4, new[] { 10, 10 }, config);

            // 基本符30（ロン） + 単騎待ち2 = 32 → 40符に切り上げ
            Assert.Equal(40, totalFu);
            Assert.Contains(details, d => d.Reason == FuCalculator.BASE && d.Fu == 30);
        }

        [Fact]
        public void TestChiitoitsu_ShouldReturn25Fu()
        {
            // 七対子：11m 22m 33p 44p 55s 66s 77z
            var hand = new List<List<int>>
            {
                new() { 0, 0 },     // 11m
                new() { 1, 1 },     // 22m
                new() { 11, 11 },   // 33p
                new() { 12, 12 },   // 44p
                new() { 22, 22 },   // 55s
                new() { 23, 23 },   // 66s
                new() { 33, 33 }    // 中中
            };

            var config = new HandConfig { IsTsumo = false };
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 33 * 4, new[] { 33, 33 }, config);

            Assert.Equal(25, totalFu);
            Assert.Single(details);
            Assert.Equal(FuCalculator.BASE, details.First().Reason);
        }

        [Fact]
        public void TestClosedPon_ShouldAddCorrectFu()
        {
            // 暗刻を含む手牌：111m 456p 789s 22z 33p
            var hand = new List<List<int>>
            {
                new() { 0, 0, 0 },    // 111m (暗刻)
                new() { 12, 13, 14 }, // 456p
                new() { 24, 25, 26 }, // 789s
                new() { 28, 28 },     // 22z (南南)
                new() { 11, 11 }      // 33p
            };

            var config = new HandConfig { IsTsumo = false };
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 11 * 4, new[] { 11, 11 }, config);

            // 基本符30（ロン） + 暗刻(么九牌)8 + 単騎待ち2 = 40符
            Assert.Equal(40, totalFu);
            Assert.Contains(details, d => d.Reason == FuCalculator.CLOSED_TERMINAL_PON && d.Fu == 8);
        }

        [Fact]
        public void TestTsumo_ShouldAdd2Fu()
        {
            // ツモ上がりの手牌
            var hand = new List<List<int>>
            {
                new() { 1, 2, 3 },    // 234m
                new() { 12, 13, 14 }, // 456p
                new() { 24, 25, 26 }, // 789s
                new() { 4, 5, 6 },    // 567m
                new() { 7, 7 }        // 88m
            };

            var config = new HandConfig { IsTsumo = true };
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 7 * 4, new[] { 7, 7 }, config);

            // 基本符20 + ツモ2 + 単騎待ち2 = 24 → 30符に切り上げ
            Assert.Equal(30, totalFu);
            Assert.Contains(details, d => d.Reason == FuCalculator.TSUMO && d.Fu == 2);
        }

        [Fact]
        public void TestValuredPair_ShouldAdd2Fu()
        {
            // 役牌雀頭の手牌
            var hand = new List<List<int>>
            {
                new() { 1, 2, 3 },    // 234m
                new() { 12, 13, 14 }, // 456p
                new() { 24, 25, 26 }, // 789s
                new() { 4, 5, 6 },    // 567m
                new() { 31, 31 }      // 白白
            };

            var config = new HandConfig { IsTsumo = false };
            var valuedTiles = new[] { 31 }; // 白が役牌
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 31 * 4, new[] { 31, 31 }, config, valuedTiles);

            // 基本符30（ロン） + 役牌雀頭2 + 単騎待ち2 = 34 → 40符に切り上げ  
            Assert.Equal(40, totalFu);
            Assert.Contains(details, d => d.Reason == FuCalculator.VALUED_PAIR && d.Fu == 2);
        }

        [Fact]
        public void TestPenchan_ShouldAdd2Fu()
        {
            // 辺張待ちの手牌：123m で3m待ち
            var hand = new List<List<int>>
            {
                new() { 0, 1, 2 },    // 123m (辺張)
                new() { 12, 13, 14 }, // 456p
                new() { 24, 25, 26 }, // 789s
                new() { 4, 5, 6 },    // 567m
                new() { 7, 7 }        // 88m
            };

            var config = new HandConfig { IsTsumo = false };
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 2 * 4, new[] { 0, 1, 2 }, config);

            // 基本符30（ロン） + 辺張2 = 32 → 40符に切り上げ
            Assert.Equal(40, totalFu);
            Assert.Contains(details, d => d.Reason == FuCalculator.PENCHAN && d.Fu == 2);
        }

        [Fact]
        public void TestKanchan_ShouldAdd2Fu()
        {
            // 嵌張待ちの手牌：135m で3m待ち
            var hand = new List<List<int>>
            {
                new() { 0, 2, 4 },    // 135m (嵌張)
                new() { 12, 13, 14 }, // 456p
                new() { 24, 25, 26 }, // 789s
                new() { 5, 6, 7 },    // 678m
                new() { 8, 8 }        // 99m
            };

            var config = new HandConfig { IsTsumo = false };
            var (details, totalFu) = _fuCalculator.CalculateFu(hand, 2 * 4, new[] { 0, 2, 4 }, config);

            // 基本符30（ロン） + 嵌張2 = 32 → 40符に切り上げ
            Assert.Equal(40, totalFu);
            Assert.Contains(details, d => d.Reason == FuCalculator.KANCHAN && d.Fu == 2);
        }
    }
}
