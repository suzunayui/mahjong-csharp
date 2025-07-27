using System;
using Mahjong;
using Mahjong.HandCalculating;
using Mahjong.Tests.TestHelpers;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests
{
    public class ScoreCalculationTest
    {
        public static void Main(string[] args)
        {
            var calculator = new HandCalculator();
            
            Console.WriteLine("=== 麻雀点数計算テスト ===\n");
            
            // Test 1: 基本的なタンヤオ
            TestHand(calculator, "基本的なタンヤオ", 
                "234567m 234567p 88s", "5m");
            
            // Test 2: リーチ + タンヤオ + ツモ
            TestHand(calculator, "リーチ + タンヤオ + ツモ", 
                "234567m 234567p 88s", "5m", 
                isRiichi: true, isTsumo: true);
            
            // Test 3: 役牌 (東)
            TestHand(calculator, "役牌 (東)", 
                "112233m 456p 789s 111z", "3m", 
                playerWind: Constants.East);
            
            // Test 4: ドラ付き
            TestHand(calculator, "ドラ付きタンヤオ", 
                "234567m 234567p 88s", "5m",
                doraIndicators: new[] { "4m" });
            
            // Test 5: ピンフ + ツモ
            TestHand(calculator, "ピンフ + ツモ", 
                "234567m 345678p 99s", "6p", 
                isTsumo: true);
            
            // Test 6: チートイツ
            TestHand(calculator, "チートイツ", 
                "1122m 3344p 5566s 77z", "2m");
            
            // Test 7: 三色同順
            TestHand(calculator, "三色同順 + タンヤオ", 
                "234m 234p 234s 567m 88p", "4m");
            
            // Test 8: 一気通貫
            TestHand(calculator, "一気通貫 + タンヤオ", 
                "123456789m 234p 88s", "5p");
            
            // Test 9: 対々和
            TestHand(calculator, "対々和",
                "111m 222p 333s 444z 55m", "5m",
                melds: new[] { "111m", "222p", "333s" });
            
            // Test 10: 高い手 (ホンイツ)
            TestHand(calculator, "混一色 + 三暗刻",
                "111222333m 11122z", "2z",
                melds: new[] { "111m" });
            
            Console.WriteLine("\n=== テスト完了 ===");
        }
        
        private static void TestHand(HandCalculator calculator, string description, 
            string tilesStr, string winTileStr, 
            bool isRiichi = false, bool isTsumo = false, 
            int? playerWind = null, int? roundWind = null,
            string[] doraIndicators = null, string[] melds = null)
        {
            try
            {
                Console.WriteLine($"【{description}】");
                
                // Parse tiles
                var tiles = ParseTileString(tilesStr);
                var winTile = ParseSingleTile(winTileStr);
                
                // Create config
                var config = MakeHandConfig(
                    isRiichi: isRiichi, 
                    isTsumo: isTsumo,
                    playerWind: playerWind,
                    roundWind: roundWind
                );
                
                // Parse dora indicators
                var doraList = new List<int>();
                if (doraIndicators != null)
                {
                    foreach (var dora in doraIndicators)
                    {
                        doraList.Add(ParseSingleTile(dora));
                    }
                }
                
                // Parse melds
                var meldList = new List<Meld>();
                if (melds != null)
                {
                    foreach (var meldStr in melds)
                    {
                        // Assume all melds are pon for simplicity
                        var meldTiles = ParseTileString(meldStr);
                        meldList.Add(new Meld
                        {
                            Type = Meld.Pon,
                            Tiles136 = meldTiles,
                            Tiles34 = ConvertTo34Array(meldTiles)
                        });
                    }
                }
                
                // Calculate hand value
                var result = calculator.EstimateHandValue(
                    tiles, winTile, 
                    melds: meldList.Count > 0 ? meldList : null,
                    doraIndicators: doraList.Count > 0 ? doraList : null,
                    config: config
                );
                
                // Display result
                Console.WriteLine($"  手牌: {tilesStr} 和了牌: {winTileStr}");
                
                if (!string.IsNullOrEmpty(result.Error))
                {
                    Console.WriteLine($"  エラー: {result.Error}");
                }
                else
                {
                    Console.WriteLine($"  翻数: {result.Han}翻");
                    Console.WriteLine($"  符数: {result.Fu}符");
                    
                    if (result.Cost != null)
                    {
                        if (isTsumo)
                        {
                            Console.WriteLine($"  ツモ点数: 子 {result.Cost.Ko} / 親 {result.Cost.Oya}");
                        }
                        else
                        {
                            Console.WriteLine($"  ロン点数: {result.Cost.Main}");
                        }
                    }
                    
                    if (result.Yaku.Any())
                    {
                        Console.WriteLine("  役:");
                        foreach (var yaku in result.Yaku)
                        {
                            var hanValue = yaku.HanClosed ?? yaku.HanOpen ?? 0;
                            if (yaku.IsYakuman)
                            {
                                Console.WriteLine($"    - {yaku.Name} (役満)");
                            }
                            else
                            {
                                Console.WriteLine($"    - {yaku.Name} ({hanValue}翻)");
                            }
                        }
                    }
                }
                
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  エラー: {ex.Message}");
                Console.WriteLine();
            }
        }
        
        private static List<int> ParseTileString(string str)
        {
            // Simple parser for strings like "234567m 234567p 88s"
            var tiles = new List<int>();
            var parts = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var part in parts)
            {
                if (part.EndsWith("m"))
                {
                    var numbers = part[..^1];
                    foreach (var c in numbers)
                    {
                        var num = int.Parse(c.ToString());
                        tiles.Add((num - 1) * 4); // Convert to 136 format
                    }
                }
                else if (part.EndsWith("p"))
                {
                    var numbers = part[..^1];
                    foreach (var c in numbers)
                    {
                        var num = int.Parse(c.ToString());
                        tiles.Add((9 + num - 1) * 4); // Convert to 136 format
                    }
                }
                else if (part.EndsWith("s"))
                {
                    var numbers = part[..^1];
                    foreach (var c in numbers)
                    {
                        var num = int.Parse(c.ToString());
                        tiles.Add((18 + num - 1) * 4); // Convert to 136 format
                    }
                }
                else if (part.EndsWith("z"))
                {
                    var numbers = part[..^1];
                    foreach (var c in numbers)
                    {
                        var num = int.Parse(c.ToString());
                        tiles.Add((27 + num - 1) * 4); // Convert to 136 format
                    }
                }
            }
            
            return tiles;
        }
        
        private static int ParseSingleTile(string str)
        {
            return ParseTileString(str).First();
        }
        
        private static List<int> ConvertTo34Array(List<int> tiles136)
        {
            return tiles136.Select(t => t / 4).ToList();
        }
    }
}