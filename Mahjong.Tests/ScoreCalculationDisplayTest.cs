using Mahjong.HandCalculating;
using static Mahjong.Tests.TestHelpers.PythonCompatibleTestHelper;

namespace Mahjong.Tests
{
    public class ScoreCalculationDisplayTest
    {
        [Fact]
        public void DisplayScoreCalculationExamples()
        {
            var calculator = new HandCalculator();
            
            Console.WriteLine("\n=== 麻雀点数計算テスト ===\n");
            
            // Test 1: 基本的なタンヤオ
            TestAndDisplay(calculator, "基本的なタンヤオ", 
                man: "234567", pin: "234567", sou: "88", winTile: "5m");
            
            // Test 2: リーチ + タンヤオ + ツモ
            TestAndDisplay(calculator, "リーチ + タンヤオ + ツモ", 
                man: "234567", pin: "234567", sou: "88", winTile: "5m",
                isRiichi: true, isTsumo: true);
            
            // Test 3: 役牌 (東)
            TestAndDisplay(calculator, "役牌 (東)", 
                pin: "112233", sou: "456", man: "789", honors: "111", winTile: "3p",
                playerWind: Constants.East);
            
            // Test 4: ドラ付き
            TestAndDisplay(calculator, "ドラ付きタンヤオ", 
                man: "234567", pin: "234567", sou: "88", winTile: "5m",
                doraIndicators: ["4m"]);
            
            // Test 5: ピンフ + ツモ
            TestAndDisplay(calculator, "ピンフ + ツモ", 
                man: "234567", pin: "345678", sou: "99", winTile: "6p",
                isTsumo: true);
            
            // Test 6: チートイツ
            TestAndDisplay(calculator, "チートイツ", 
                man: "1122", pin: "3344", sou: "5566", honors: "77", winTile: "2m");
            
            // Test 7: 三色同順
            TestAndDisplay(calculator, "三色同順 + タンヤオ", 
                man: "234567", pin: "23488", sou: "234", winTile: "4p");
            
            // Test 8: 一気通貫
            TestAndDisplay(calculator, "一気通貫 + タンヤオ", 
                man: "123456789", pin: "234", sou: "88", winTile: "5p");
            
            // Test 9: 九蓮宝燈（テスト用）
            TestAndDisplay(calculator, "九蓮宝燈テスト", 
                sou: "1112345678999", winTile: "5s");
            
            Console.WriteLine("=== テスト完了 ===\n");
        }
        
        private static void TestAndDisplay(HandCalculator calculator, string description,
            string man = "", string pin = "", string sou = "", string honors = "",
            string winTile = "",
            bool isRiichi = false, bool isTsumo = false, 
            int? playerWind = null, int? roundWind = null,
            string[]? doraIndicators = null)
        {
            try
            {
                Console.WriteLine($"【{description}】");
                
                // Parse tiles using existing helper
                var tiles = StringTo136Array(man: man, pin: pin, sou: sou, honors: honors);
                var winTile136 = StringTo136Tile(winTile);
                
                // Add win tile to hand for calculation
                tiles.Add(winTile136);
                
                // Create config
                var config = MakeHandConfig(
                    isRiichi: isRiichi, 
                    isTsumo: isTsumo,
                    playerWind: playerWind,
                    roundWind: roundWind
                );
                
                // Parse dora indicators
                List<int> doraList = [];
                if (doraIndicators != null)
                {
                    foreach (var dora in doraIndicators)
                    {
                        doraList.Add(StringTo136Tile(dora));
                    }
                }
                
                // Calculate hand value
                var result = calculator.EstimateHandValue(
                    tiles, winTile136,
                    doraIndicators: doraList.Count > 0 ? doraList : null,
                    config: config
                );
                
                // Display hand composition
                var handStr = "";
                if (!string.IsNullOrEmpty(man)) handStr += man + "m ";
                if (!string.IsNullOrEmpty(pin)) handStr += pin + "p ";
                if (!string.IsNullOrEmpty(sou)) handStr += sou + "s ";
                if (!string.IsNullOrEmpty(honors)) handStr += honors + "z ";
                
                Console.WriteLine($"  手牌: {handStr.Trim()} 和了牌: {winTile}");
                
                if (!string.IsNullOrEmpty(result.Error))
                {
                    Console.WriteLine($"  ❌ エラー: {result.Error}");
                }
                else
                {
                    Console.WriteLine($"  📊 {result.Han}翻 {result.Fu}符");
                    
                    if (result.Cost != null)
                    {
                        if (isTsumo)
                        {
                            var main = result.Cost.TryGetValue("main", out var mainValue) ? Convert.ToInt32(mainValue) : 0;
                            var additional = result.Cost.TryGetValue("additional", out var additionalValue) ? Convert.ToInt32(additionalValue) : 0;
                            Console.WriteLine($"  💰 ツモ: 子{additional}点 / 親{main}点");
                        }
                        else
                        {
                            var main = result.Cost.TryGetValue("main", out var mainValue) ? Convert.ToInt32(mainValue) : 0;
                            Console.WriteLine($"  💰 ロン: {main}点");
                        }
                    }
                    
                    if (result.Yaku != null && result.Yaku.Count > 0)
                    {
                        Console.WriteLine("  🎯 役:");
                        foreach (var yaku in result.Yaku)
                        {
                            var hanValue = yaku.HanClosed ?? yaku.HanOpen ?? 0;
                            if (yaku.IsYakuman)
                            {
                                Console.WriteLine($"    ⭐ {yaku.Name} (役満)");
                            }
                            else
                            {
                                Console.WriteLine($"    • {yaku.Name} ({hanValue}翻)");
                            }
                        }
                    }
                }
                
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ 例外エラー: {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}