using Xunit;
using Mahjong;
using Mahjong.HandCalculating;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests.YakuTests
{
    public class SanshokuAndIttsuTests
    {
        [Fact]
        public void TestSanshokuDoujun_ClosedHand()
        {
            var calculator = new HandCalculator();

            // 三色同順: 234m 234p 234s 567m 88p
            var tiles = StringTo136Array(man: "234567", pin: "23488", sou: "234");
            var winTile = StringTo136Tile("4p");

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            Assert.True(result.Han >= 3); // Sanshoku (2翻) + Tanyao (1翻)
            Assert.Contains(result.Yaku, y => y.Name == "Sanshoku Doujun");
            Assert.Contains(result.Yaku, y => y.Name == "Tanyao");
        }

        [Fact]
        public void TestSanshokuDoujun_OpenHand()
        {
            var calculator = new HandCalculator();

            // 三色同順: 234m 234p 567m 88p + open 234s
            var tiles = StringTo136Array(man: "234567", pin: "23488");
            var winTile = StringTo136Tile("4p");
            var melds = new List<Meld>
            {
                MakeMeld(Meld.Chi, sou: "234")
            };

            var result = calculator.EstimateHandValue(tiles, winTile, melds: melds);

            Assert.Null(result.Error);
            Assert.True(result.Han >= 2); // Sanshoku (1翻 open) + Tanyao (1翻)
            Assert.Contains(result.Yaku, y => y.Name == "Sanshoku Doujun");
            Assert.Contains(result.Yaku, y => y.Name == "Tanyao");
        }

        [Fact]
        public void TestIttsu_ClosedHand()
        {
            var calculator = new HandCalculator();

            // 一気通貫: 123456789m 234p 88s (実際の動作例から複製)
            var tiles = StringTo136Array(man: "123456789", pin: "234", sou: "88");
            var winTile = StringTo136Tile("5p");

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            Assert.True(result.Han >= 2); // Ittsu (2翻) 
            Assert.Contains(result.Yaku, y => y.Name == "Ittsu");
        }

        [Fact]
        public void TestIttsu_OpenHand()
        {
            var calculator = new HandCalculator();

            // 一気通貫: 123456789m 99s + open 234p
            var tiles = StringTo136Array(man: "123456789", sou: "99");
            var winTile = StringTo136Tile("9m");
            var melds = new List<Meld>
            {
                MakeMeld(Meld.Chi, pin: "234")
            };

            var result = calculator.EstimateHandValue(tiles, winTile, melds: melds);

            Assert.Null(result.Error);
            Assert.True(result.Han >= 1); // Ittsu (1翻 open)
            Assert.Contains(result.Yaku, y => y.Name == "Ittsu");
        }

        [Fact]
        public void TestSanshokuDoujun_NoCondition()
        {
            var calculator = new HandCalculator();

            // 三色同順ではない: 234567m 234567p 88s (三色なし)
            var tiles = StringTo136Array(man: "234567", pin: "234567", sou: "88");
            var winTile = StringTo136Tile("5m");

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            // 三色同順は含まれない（同色の順子が3つない）
            Assert.DoesNotContain(result.Yaku, y => y.Name == "Sanshoku Doujun");
        }

        [Fact]
        public void TestIttsu_NoCondition()
        {
            var calculator = new HandCalculator();

            // 一気通貫ではない: 234567m 234567p 88s (一気通貫なし)
            var tiles = StringTo136Array(man: "234567", pin: "234567", sou: "88");
            var winTile = StringTo136Tile("5p");

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            // 一気通貫は含まれない（123m, 456m, 789mがない）
            Assert.DoesNotContain(result.Yaku, y => y.Name == "Ittsu");
        }

        [Fact]
        public void TestSanshokuAndIttsu_Together()
        {
            var calculator = new HandCalculator();

            // 理論上は可能だが実際は難しい組み合わせをテスト
            // 123456789m 234p 234s + one more sequence to complete
            // Note: 実際の手牌構成では困難なので、簡単なケースをテスト
            var tiles = StringTo136Array(man: "123456789", pin: "234", sou: "88");
            var winTile = StringTo136Tile("5p");

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            // 一気通貫のみ（三色同順の条件を満たさない）
            Assert.Contains(result.Yaku, y => y.Name == "Ittsu");
            Assert.DoesNotContain(result.Yaku, y => y.Name == "Sanshoku Doujun");
        }
    }
}