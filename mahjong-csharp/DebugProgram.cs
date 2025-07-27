using System;
using System.Collections.Generic;
using Mahjong;

namespace Mahjong
{
    public class DebugProgram
    {
        public static void TestDoraCalculation()
        {
            Console.WriteLine("=== ドラ計算テスト ===");
            
            // Test case: dora indicator is 3 (1m indicator), so dora should be 4 (2m)
            var doraIndicators = new List<int> { 3 }; // 1m indicator
            var testTiles = new List<int> { 0, 4, 8 }; // 1m tiles
            
            Console.WriteLine($"ドラ指示牌: {string.Join(",", doraIndicators)}");
            Console.WriteLine($"テスト牌: {string.Join(",", testTiles)}");
            
            foreach (var tile in testTiles)
            {
                var doraCount = Utils.PlusDora(tile, doraIndicators);
                Console.WriteLine($"牌 {tile}: ドラ数 = {doraCount}");
            }
            
            // Test with next tile (should be dora)
            var nextTiles = new List<int> { 4, 5, 6, 7 }; // 2m tiles
            Console.WriteLine("\n次の牌（ドラになるはず）:");
            foreach (var tile in nextTiles)
            {
                var doraCount = Utils.PlusDora(tile, doraIndicators);
                Console.WriteLine($"牌 {tile}: ドラ数 = {doraCount}");
            }
        }

        public static void TestHandValidation()
        {
            Console.WriteLine("\n=== 手牌検証テスト ===");
            
            // Test 7の手牌をチェック
            var testTiles = new List<int> { 4, 5, 6, 12, 16, 20, 24, 28, 32, 36, 40, 44, 72, 73 };
            var winTile = 72;
            
            Console.WriteLine($"テスト牌: {string.Join(",", testTiles)}");
            Console.WriteLine($"上がり牌: {winTile}");
            
            var tiles34 = TilesConverter.To34Array(testTiles);
            Console.WriteLine($"34形式: {string.Join(",", tiles34)}");
            
            // 合計枚数チェック
            var totalTiles = tiles34.Sum();
            Console.WriteLine($"合計枚数: {totalTiles}");
            
            // 和了チェック
            var agari = new Agari();
            var isAgari = agari.IsAgari(tiles34);
            Console.WriteLine($"和了判定: {isAgari}");
            
            // 正しい和了形を作る (123m 456p 789s 234p 11s)
            var correctTiles = TilesConverter.OneLineStringTo136Array("123m456p789s234p11s");
            var correctTiles34 = TilesConverter.To34Array(correctTiles);
            var correctWinTile = TilesConverter.OneLineStringTo136Array("1s")[0];
            
            Console.WriteLine($"\n正しい手牌例: {string.Join(",", correctTiles)}");
            Console.WriteLine($"正しい34形式: {string.Join(",", correctTiles34)}");
            Console.WriteLine($"正しい和了判定: {agari.IsAgari(correctTiles34)}");
        }
    }
}