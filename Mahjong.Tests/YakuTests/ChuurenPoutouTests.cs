using Xunit;
using Mahjong.HandCalculating;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests.YakuTests
{
    public class ChuurenPoutouTests
    {
        [Fact]
        public void TestChuurenPoutou_BasicPattern()
        {
            var calculator = new HandCalculator();

            // 九蓮宝燈: 111234567899s + 9s (手牌13枚 + 和了牌1枚で合計14枚)
            var tiles = StringTo136Array(sou: "1112345678999");
            var winTile = StringTo136Tile("9s");
            tiles.Add(winTile); // 和了牌を手牌に追加

            Console.WriteLine($"Tiles count: {tiles.Count}");
            Console.WriteLine($"Win tile: {winTile}");
            Console.WriteLine($"Tiles contains win tile: {tiles.Contains(winTile)}");
            Console.WriteLine($"Tiles: [{string.Join(", ", tiles)}]");

            var result = calculator.EstimateHandValue(tiles, winTile);

            Console.WriteLine($"Error: {result.Error}");
            if (result.Yaku != null)
            {
                foreach (var yaku in result.Yaku)
                {
                    Console.WriteLine($"Yaku: {yaku.Name}, Han: {(yaku.HanClosed ?? yaku.HanOpen ?? 0)}");
                }
            }
            
            // まずはエラーがないことを確認する
            Assert.Null(result.Error);
        }

        [Fact]
        public void TestDaburuChuurenPoutou_PureNineGates()
        {
            var calculator = new HandCalculator();

            // 純正九蓮宝燈: 1112345678999s + 5s (9面待ちの1つ)
            var tiles = StringTo136Array(sou: "1112345678999");
            var winTile = StringTo136Tile("5s");
            tiles.Add(winTile); // 和了牌を手牌に追加

            var result = calculator.EstimateHandValue(tiles, winTile);

            // テスト結果を見やすく出力
            Console.WriteLine("=== 純正九蓮宝燈テスト (索子) ===");
            Console.WriteLine($"手牌: 1112345678999s + 5s");
            Console.WriteLine($"エラー: {result.Error ?? "なし"}");
            Console.WriteLine($"合計翻数: {result.Han}翻");
            Console.WriteLine($"符: {result.Fu}符");
            if (result.Yaku != null && result.Yaku.Any())
            {
                Console.WriteLine("検出された役:");
                foreach (var yaku in result.Yaku)
                {
                    Console.WriteLine($"  - {yaku.Name}: {(yaku.HanClosed ?? yaku.HanOpen ?? 0)}翻");
                }
            }
            else
            {
                Console.WriteLine("役: なし");
            }
            Console.WriteLine("==================================");

            Assert.Null(result.Error);
            // 純正九蓮宝燈はダブル役満として判定されるべき
            Assert.NotNull(result.Yaku);
            Assert.Contains(result.Yaku, y => y.Name.Contains("Daburu Chuuren Poutou"));
        }

        [Fact]
        public void TestChuurenPoutou_ManSuit()
        {
            var calculator = new HandCalculator();

            // 純正九蓮宝燈: 1112345678999m + 1m
            // これは純正九蓮宝燈（基本形 + 9面待ちの一つで和了）
            var tiles = StringTo136Array(man: "1112345678999");
            var winTile = StringTo136Tile("1m");
            tiles.Add(winTile); // 和了牌を手牌に追加

            var result = calculator.EstimateHandValue(tiles, winTile);

            // テスト結果を見やすく出力
            Console.WriteLine("=== 純正九蓮宝燈テスト (萬子) ===");
            Console.WriteLine($"手牌: 1112345678999m + 1m");
            Console.WriteLine($"エラー: {result.Error ?? "なし"}");
            Console.WriteLine($"合計翻数: {result.Han}翻");
            Console.WriteLine($"符: {result.Fu}符");
            if (result.Yaku != null && result.Yaku.Any())
            {
                Console.WriteLine("検出された役:");
                foreach (var yaku in result.Yaku)
                {
                    Console.WriteLine($"  - {yaku.Name}: {(yaku.HanClosed ?? yaku.HanOpen ?? 0)}翻");
                }
            }
            else
            {
                Console.WriteLine("役: なし");
            }
            Console.WriteLine("================================");

            Assert.Null(result.Error);
            Assert.NotNull(result.Yaku);
            // 1112345678999m + 1m は純正九蓮宝燈（9面待ちの一つ）
            Assert.Contains(result.Yaku, y => y.Name == "Daburu Chuuren Poutou");
        }

        [Fact]
        public void TestChuurenPoutou_PinSuit()
        {
            var calculator = new HandCalculator();

            // 純正九蓮宝燈: 1112345678999p + 1p
            // これは純正九蓮宝燈（基本形 + 9面待ちの一つで和了）
            var tiles = StringTo136Array(pin: "1112345678999");
            var winTile = StringTo136Tile("1p");
            tiles.Add(winTile); // 和了牌を手牌に追加

            var result = calculator.EstimateHandValue(tiles, winTile);

            // テスト結果を見やすく出力
            Console.WriteLine("=== 純正九蓮宝燈テスト (筒子) ===");
            Console.WriteLine($"手牌: 1112345678999p + 1p");
            Console.WriteLine($"エラー: {result.Error ?? "なし"}");
            Console.WriteLine($"合計翻数: {result.Han}翻");
            Console.WriteLine($"符: {result.Fu}符");
            if (result.Yaku != null && result.Yaku.Any())
            {
                Console.WriteLine("検出された役:");
                foreach (var yaku in result.Yaku)
                {
                    Console.WriteLine($"  - {yaku.Name}: {(yaku.HanClosed ?? yaku.HanOpen ?? 0)}翻");
                }
            }
            else
            {
                Console.WriteLine("役: なし");
            }
            Console.WriteLine("================================");

            Assert.Null(result.Error);
            Assert.NotNull(result.Yaku);
            // 1112345678999p + 1p は純正九蓮宝燈（9面待ちの一つ）
            Assert.Contains(result.Yaku, y => y.Name == "Daburu Chuuren Poutou");
        }

        [Fact]
        public void TestChuurenPoutou_NotPure()
        {
            var calculator = new HandCalculator();

            // 九蓮宝燈（純正ではない）: 1122345678999s + 2s
            // これは純正ではない（1と2が2枚ずつ、9面待ちではない）
            var tiles = StringTo136Array(sou: "1122345678999");
            var winTile = StringTo136Tile("2s");
            tiles.Add(winTile); // 和了牌を手牌に追加

            var result = calculator.EstimateHandValue(tiles, winTile);

            // テスト結果を見やすく出力
            Console.WriteLine("=== 九蓮宝燈テスト（純正ではない）(索子) ===");
            Console.WriteLine($"手牌: 1122345678999s + 2s");
            Console.WriteLine($"エラー: {result.Error ?? "なし"}");
            Console.WriteLine($"合計翻数: {result.Han}翻");
            Console.WriteLine($"符: {result.Fu}符");
            if (result.Yaku != null && result.Yaku.Any())
            {
                Console.WriteLine("検出された役:");
                foreach (var yaku in result.Yaku)
                {
                    Console.WriteLine($"  - {yaku.Name}: {(yaku.HanClosed ?? yaku.HanOpen ?? 0)}翻");
                }
            }
            else
            {
                Console.WriteLine("役: なし");
            }
            Console.WriteLine("================================");

            Assert.Null(result.Error);
            Assert.NotNull(result.Yaku);
            // 1122345678999s + 2s は九蓮宝燈パターンではない（清一色のみ）
            Assert.Contains(result.Yaku, y => y.Name == "Chinitsu");
            Assert.DoesNotContain(result.Yaku, y => y.Name == "Chuuren Poutou");
            Assert.DoesNotContain(result.Yaku, y => y.Name == "Daburu Chuuren Poutou");
        }

        [Fact]
        public void TestNotChuurenPoutou_MixedSuits()
        {
            var calculator = new HandCalculator();

            // 九蓮宝燈ではない: 混色（タンヤオ付き）
            var tiles = StringTo136Array(sou: "234567", man: "234567", pin: "8");
            var winTile = StringTo136Tile("8p");
            tiles.Add(winTile); // 和了牌を手牌に追加

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            Assert.True(result.Yaku == null || !result.Yaku.Any(y => y.Name == "Chuuren Poutou"));
        }

        [Fact]
        public void TestNotChuurenPoutou_WrongPattern()
        {
            var calculator = new HandCalculator();

            // 九蓮宝燈ではない: 清一色だが九蓮宝燈パターンではない（通常の清一色）
            var tiles = StringTo136Array(sou: "1122334455667");
            var winTile = StringTo136Tile("7s");
            tiles.Add(winTile); // 和了牌を手牌に追加

            var result = calculator.EstimateHandValue(tiles, winTile);

            Assert.Null(result.Error);
            Assert.True(result.Yaku == null || !result.Yaku.Any(y => y.Name == "Chuuren Poutou"));
            // 清一色の役があることを確認
            Assert.True(result.Yaku != null && result.Yaku.Any(y => y.Name == "Chinitsu"));
        }
    }
}