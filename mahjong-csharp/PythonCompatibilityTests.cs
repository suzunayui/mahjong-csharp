using System;
using System.Collections.Generic;
using System.Linq;
using Mahjong;
using Mahjong.HandCalculating;

namespace Mahjong
{
    /// <summary>
    /// Python版麻雀ライブラリとの互換性テスト
    /// 元のPythonテストケース: tests/hand_calculating/tests_yaku_calculation.py
    /// </summary>
    public class PythonCompatibilityTests
    {
        private static int testCount = 0;
        private static int passCount = 0;
        private static int failCount = 0;

        public static void RunAllTests()
        {
            Console.WriteLine("=== Python互換性テスト開始 ===");
            
            // 基本的な役のテスト
            TestIsRiichi();
            TestIsTsumo();
            TestIsPinfu();
            TestIsTanyao();
            TestIsChiitoitsu();
            TestIsIipeiko();
            TestIsRyanpeiko();
            
            // ドラテスト
            TestDoraInHand();
            
            // 複合役のテスト
            TestComplexHands();
            
            // 特殊ケースのテスト
            TestSpecialCases();
            
            // 手牌計算の総合テスト
            TestHandsCalculation();
            
            // 結果表示
            Console.WriteLine("\n=== Python互換性テスト結果 ===");
            Console.WriteLine($"実行テスト数: {testCount}");
            Console.WriteLine($"成功: {passCount}");
            Console.WriteLine($"失敗: {failCount}");
            Console.WriteLine($"成功率: {(double)passCount / testCount * 100:F1}%");
        }

        public static void TestIsRiichi()
        {
            Console.WriteLine("\n--- リーチテスト ---");
            var calculator = new HandCalculator();
            
            var tiles = TilesConverter.OneLineStringTo136Array("123444s234456m66p");
            var winTile = TilesConverter.OneLineStringTo136Array("4s")[0];
            
            TestHand(calculator, "リーチ基本形",
                tiles, winTile,
                new HandConfig { IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 40, expectedYakuCount: 1);
        }

        public static void TestIsTsumo()
        {
            Console.WriteLine("\n--- ツモテスト ---");
            var calculator = new HandCalculator();
            
            var tiles = TilesConverter.OneLineStringTo136Array("123444s234456m66p");
            var winTile = TilesConverter.OneLineStringTo136Array("4s")[0];
            
            // 門前ツモ
            TestHand(calculator, "門前ツモ",
                tiles, winTile,
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1);
            
            // 鳴き手でのツモ（役なし）
            var melds = new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("123s").Take(3)) };
            TestHand(calculator, "鳴き手ツモ（役なし）",
                tiles, winTile, 
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                melds: melds,
                expectError: true);
        }

        public static void TestIsPinfu()
        {
            Console.WriteLine("\n--- 平和テスト ---");
            var calculator = new HandCalculator();
            
            // 基本的な平和
            var tiles = TilesConverter.OneLineStringTo136Array("123456s123456m55p");
            var winTile = TilesConverter.OneLineStringTo136Array("6m")[0];
            
            TestHand(calculator, "平和基本形",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1);
            
            // 両面待ち以外（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("123456s123555m55p");
            winTile = TilesConverter.OneLineStringTo136Array("5m")[0];
            
            TestHand(calculator, "両面待ち以外（平和成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true);
            
            // 刻子を含む（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("111456s123456m55p");
            winTile = TilesConverter.OneLineStringTo136Array("6m")[0];
            
            TestHand(calculator, "刻子含み（平和成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true);
            
            // 辺張待ち（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("123456s123456m55p");
            winTile = TilesConverter.OneLineStringTo136Array("3s")[0];
            
            TestHand(calculator, "辺張待ち（平和成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true);
            
            // 嵌張待ち（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("123567s123456m55p");
            winTile = TilesConverter.OneLineStringTo136Array("6s")[0];
            
            TestHand(calculator, "嵌張待ち（平和成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true);
            
            // 単騎待ち（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("22456678m123678p");
            winTile = TilesConverter.OneLineStringTo136Array("2m")[0];
            
            TestHand(calculator, "単騎待ち（平和成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true);
            
            // 役牌雀頭（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("123678s123456m11z");
            winTile = TilesConverter.OneLineStringTo136Array("6s")[0];
            
            TestHand(calculator, "役牌雀頭（平和成立せず）",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = false, 
                    PlayerWind = 27, // 東
                    RoundWind = 30,  // 西
                    Yaku = new YakuConfig() 
                },
                expectError: true);
            
            // 非役牌雀頭（平和成立）
            tiles = TilesConverter.OneLineStringTo136Array("123678s123456m22z");
            winTile = TilesConverter.OneLineStringTo136Array("6s")[0];
            
            TestHand(calculator, "非役牌雀頭（平和成立）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1);
            
            // 鳴き手（平和成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("12399s123456m456p");
            winTile = TilesConverter.OneLineStringTo136Array("1m")[0];
            var melds = new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("123s").Take(3)) };
            
            TestHand(calculator, "鳴き手（平和成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                melds: melds,
                expectError: true);
        }

        public static void TestIsTanyao()
        {
            Console.WriteLine("\n--- 断么九テスト ---");
            var calculator = new HandCalculator();
            
            // 基本的な断么九
            var tiles = TilesConverter.OneLineStringTo136Array("234567s234567m22p");
            var winTile = TilesConverter.OneLineStringTo136Array("7m")[0];
            
            TestHand(calculator, "断么九基本形",
                tiles, winTile,
                new HandConfig { IsTsumo = false, IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 3, expectedFu: 30, expectedYakuCount: 3); // 断么九 + 平和 + リーチ
            
            // 一九牌を含む（断么九成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("123456s234567m22p");
            winTile = TilesConverter.OneLineStringTo136Array("7m")[0];
            
            TestHand(calculator, "一九牌含み（断么九成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1); // 平和のみ
            
            // 字牌を含む（断么九成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("234567s234567m22z");
            winTile = TilesConverter.OneLineStringTo136Array("7m")[0];
            
            TestHand(calculator, "字牌含み（断么九成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true); // 役なし
            
            // 鳴き断么九（オプション有効）
            tiles = TilesConverter.OneLineStringTo136Array("234567m234567p22s");
            winTile = TilesConverter.OneLineStringTo136Array("7p")[0];
            var melds = new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("234s").Take(3)) };
            
            TestHand(calculator, "鳴き断么九（オプション有効）",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = false, 
                    Yaku = new YakuConfig(),
                    Options = new OptionalRules(hasOpenTanyao: true)
                },
                melds: melds,
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1);
            
            // 鳴き断么九（オプション無効）
            TestHand(calculator, "鳴き断么九（オプション無効）",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = false, 
                    Yaku = new YakuConfig(),
                    Options = new OptionalRules(hasOpenTanyao: false)
                },
                melds: melds,
                expectError: true);
        }

        public static void TestIsChiitoitsu()
        {
            Console.WriteLine("\n--- 七対子テスト ---");
            var calculator = new HandCalculator();
            
            // 基本的な七対子
            var tiles = TilesConverter.OneLineStringTo136Array("113355s113355m11p");
            var winTile = TilesConverter.OneLineStringTo136Array("1p")[0];
            
            TestHand(calculator, "七対子基本形",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 2, expectedFu: 25, expectedYakuCount: 1);
            
            // 七対子（混合）
            tiles = TilesConverter.OneLineStringTo136Array("2299s2299m1199p44z");
            winTile = TilesConverter.OneLineStringTo136Array("4z")[0];
            
            TestHand(calculator, "七対子（混合）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 2, expectedFu: 25, expectedYakuCount: 1);
            
            // 同一牌4枚（七対子成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("11335555s1133m11p");
            winTile = TilesConverter.OneLineStringTo136Array("1p")[0];
            
            TestHand(calculator, "同一牌4枚（七対子成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectError: true);
        }

        public static void TestIsIipeiko()
        {
            Console.WriteLine("\n--- 一盃口テスト ---");
            var calculator = new HandCalculator();
            
            // 基本的な一盃口
            var tiles = TilesConverter.OneLineStringTo136Array("112233s333m12344p");
            var winTile = TilesConverter.OneLineStringTo136Array("3m")[0];
            
            TestHand(calculator, "一盃口基本形",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 40, expectedYakuCount: 1);
            
            // 鳴き手（一盃口成立せず）
            var melds = new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("123s").Take(3)) };
            TestHand(calculator, "鳴き手（一盃口成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                melds: melds,
                expectError: true);
        }

        public static void TestIsRyanpeiko()
        {
            Console.WriteLine("\n--- 二盃口テスト ---");
            var calculator = new HandCalculator();
            
            // 基本的な二盃口
            var tiles = TilesConverter.OneLineStringTo136Array("112233s33m223344p");
            var winTile = TilesConverter.OneLineStringTo136Array("3p")[0];
            
            TestHand(calculator, "二盃口基本形",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 3, expectedFu: 40, expectedYakuCount: 1);
            
            // 同一順子4組の二盃口
            tiles = TilesConverter.OneLineStringTo136Array("111122223333m22p");
            winTile = TilesConverter.OneLineStringTo136Array("2p")[0];
            
            TestHand(calculator, "同一順子4組の二盃口",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 3, expectedFu: 40, expectedYakuCount: 1);
            
            // 一盃口（二盃口成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("112233s123m23444p");
            winTile = TilesConverter.OneLineStringTo136Array("4p")[0];
            
            TestHand(calculator, "一盃口（二盃口成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 40, expectedYakuCount: 1); // 一盃口
            
            // 鳴き手（二盃口成立せず）
            tiles = TilesConverter.OneLineStringTo136Array("112233s33m223344p");
            winTile = TilesConverter.OneLineStringTo136Array("3p")[0];
            var melds = new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("123s").Take(3)) };
            
            TestHand(calculator, "鳴き手（二盃口成立せず）",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                melds: melds,
                expectError: true);
        }

        public static void TestDoraInHand()
        {
            Console.WriteLine("\n--- ドラテスト ---");
            var calculator = new HandCalculator();
            
            // ドラなしでは役なしエラー
            var tiles = TilesConverter.OneLineStringTo136Array("345678s456789m55z");
            var winTile = TilesConverter.OneLineStringTo136Array("5s")[0];
            var doraIndicators = new List<int> { TilesConverter.OneLineStringTo136Array("5s")[0] };
            var melds = new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("678s").Take(3)) };
            
            TestHand(calculator, "ドラのみは役なしエラー",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                doraIndicators: doraIndicators,
                melds: melds,
                expectError: true);
            
            // 平和+ドラ
            tiles = TilesConverter.OneLineStringTo136Array("123456s123456m33p");
            winTile = TilesConverter.OneLineStringTo136Array("6m")[0];
            doraIndicators = new List<int> { TilesConverter.OneLineStringTo136Array("2p")[0] }; // 3pがドラ
            
            TestHand(calculator, "平和+ドラ",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                doraIndicators: doraIndicators,
                expectedHan: 3, expectedFu: 30, expectedYakuCount: 2); // 平和 + ドラ2
            
            // 複数ドラ
            tiles = TilesConverter.OneLineStringTo136Array("22456678m123678p");
            winTile = TilesConverter.OneLineStringTo136Array("2m")[0];
            doraIndicators = new List<int> { 
                TilesConverter.OneLineStringTo136Array("1m")[0], // 2mがドラ
                TilesConverter.OneLineStringTo136Array("2p")[0]  // 3pがドラ
            };
            
            TestHand(calculator, "複数ドラ",
                tiles, winTile,
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                doraIndicators: doraIndicators,
                expectedHan: 4, expectedFu: 30, expectedYakuCount: 2); // ツモ + ドラ3
            
            // 同一ドラ指示牌
            tiles = TilesConverter.OneLineStringTo136Array("678m34577p123345s");
            winTile = TilesConverter.OneLineStringTo136Array("7p")[0];
            doraIndicators = new List<int> { 
                TilesConverter.OneLineStringTo136Array("4s")[0], // 5sがドラ
                TilesConverter.OneLineStringTo136Array("4s")[0]  // 同じドラ指示牌
            };
            
            TestHand(calculator, "同一ドラ指示牌",
                tiles, winTile,
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                doraIndicators: doraIndicators,
                expectedHan: 3, expectedFu: 30, expectedYakuCount: 2); // ツモ + ドラ2
        }

        public static void TestComplexHands()
        {
            Console.WriteLine("\n--- 複合役テスト ---");
            var calculator = new HandCalculator();
            
            // 断么九 + 平和 + リーチ
            var tiles = TilesConverter.OneLineStringTo136Array("234567s234567m22p");
            var winTile = TilesConverter.OneLineStringTo136Array("7m")[0];
            
            TestHand(calculator, "断么九+平和+リーチ",
                tiles, winTile,
                new HandConfig { IsTsumo = false, IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 3, expectedFu: 30, expectedYakuCount: 3);
            
            // 断么九 + 平和 + リーチ + ツモ
            TestHand(calculator, "断么九+平和+リーチ+ツモ",
                tiles, winTile,
                new HandConfig { IsTsumo = true, IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 4, expectedFu: 20, expectedYakuCount: 4); // 平和ツモは20符
            
            // 役牌（東）
            tiles = TilesConverter.OneLineStringTo136Array("123s456m789s111z22p");
            winTile = TilesConverter.OneLineStringTo136Array("2p")[0];
            
            TestHand(calculator, "役牌（東）",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = false,
                    PlayerWind = 27, // 東
                    RoundWind = 27,  // 東
                    Yaku = new YakuConfig() 
                },
                expectedHan: 2, expectedFu: 40, expectedYakuCount: 2); // 東（自風牌）+ 東（場風牌）
        }

        public static void TestSpecialCases()
        {
            Console.WriteLine("\n--- 特殊ケーステスト ---");
            var calculator = new HandCalculator();
            
            // 符計算テスト：刻子による符
            var tiles = TilesConverter.OneLineStringTo136Array("678s12333456789m");
            var winTile = TilesConverter.OneLineStringTo136Array("3m")[0];
            
            TestHand(calculator, "刻子含み符計算",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 2, expectedFu: 40, expectedYakuCount: 1); // 一盃口
            
            // 七対子 vs 二盃口の優先度テスト（Pythonテストより）
            tiles = TilesConverter.OneLineStringTo136Array("112233m445566p77s");
            winTile = TilesConverter.OneLineStringTo136Array("7s")[0];
            
            TestHand(calculator, "二盃口優先（vs七対子）",
                tiles, winTile,
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                expectedHan: 4, expectedFu: 30, expectedYakuCount: 2); // 二盃口 + ツモ
            
            // リーチ + 一発
            tiles = TilesConverter.OneLineStringTo136Array("123444s234456m66p");
            winTile = TilesConverter.OneLineStringTo136Array("4s")[0];
            
            TestHand(calculator, "リーチ一発",
                tiles, winTile,
                new HandConfig { 
                    IsRiichi = true, 
                    IsIppatsu = true, 
                    Yaku = new YakuConfig() 
                },
                expectedHan: 2, expectedFu: 40, expectedYakuCount: 2);
            
            // ダブルリーチ
            TestHand(calculator, "ダブルリーチ",
                tiles, winTile,
                new HandConfig { 
                    IsDaburuRiichi = true, 
                    IsRiichi = true,
                    Yaku = new YakuConfig() 
                },
                expectedHan: 2, expectedFu: 40, expectedYakuCount: 1);
            
            // 海底撈月
            TestHand(calculator, "海底撈月",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = true,
                    IsHaitei = true,
                    Yaku = new YakuConfig() 
                },
                expectedHan: 2, expectedFu: 30, expectedYakuCount: 2); // ツモ + 海底
            
            // 河底撈魚
            TestHand(calculator, "河底撈魚",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = false,
                    IsHoutei = true,
                    Yaku = new YakuConfig() 
                },
                expectedHan: 1, expectedFu: 40, expectedYakuCount: 1);
            
            // 嶺上開花（暗槓）
            tiles = TilesConverter.OneLineStringTo136Array("1234444s234456m66p");
            winTile = TilesConverter.OneLineStringTo136Array("1s")[0];
            var melds = new List<Meld> { 
                new Meld(Meld.Kan, TilesConverter.OneLineStringTo136Array("4444s").ToList()) { Opened = false }
            };
            
            TestHand(calculator, "嶺上開花（暗槓）",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = true,
                    IsRinshan = true,
                    Yaku = new YakuConfig() 
                },
                melds: melds,
                expectedHan: 2, expectedFu: 40, expectedYakuCount: 2); // 嶺上開花 + ツモ
            
            // 嶺上開花（明槓）
            melds = new List<Meld> { 
                new Meld(Meld.Kan, TilesConverter.OneLineStringTo136Array("4444s").ToList()) { Opened = true }
            };
            
            TestHand(calculator, "嶺上開花（明槓）",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = true,
                    IsRinshan = true,
                    Yaku = new YakuConfig() 
                },
                melds: melds,
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1); // 嶺上開花のみ
            
            // 槍槓
            tiles = TilesConverter.OneLineStringTo136Array("123444s234456m66p");
            winTile = TilesConverter.OneLineStringTo136Array("1s")[0];
            
            TestHand(calculator, "槍槓",
                tiles, winTile,
                new HandConfig { 
                    IsTsumo = false,
                    IsChankan = true,
                    Yaku = new YakuConfig() 
                },
                expectedHan: 1, expectedFu: 40, expectedYakuCount: 1);
        }

        public static void TestHandsCalculation()
        {
            Console.WriteLine("\n--- 手牌計算総合テスト ---");
            var calculator = new HandCalculator();
            
            // Python テストの複雑な手牌例
            // tiles = TilesConverter.string_to_136_array(sou="123345678", man="678", pin="88")
            var tiles = TilesConverter.OneLineStringTo136Array("123345678s678m88p");
            var winTile = TilesConverter.OneLineStringTo136Array("3s")[0];
            
            TestHand(calculator, "Python互換テスト1",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1);
            
            // Python テストケース2
            tiles = TilesConverter.OneLineStringTo136Array("12399s123456m456p");
            winTile = TilesConverter.OneLineStringTo136Array("1s")[0];
            
            TestHand(calculator, "Python互換テスト2",
                tiles, winTile,
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedYakuCount: 1);
        }

        private static void TestHand(
            HandCalculator calculator,
            string testName,
            List<int> tiles,
            int winTile,
            HandConfig config,
            int? expectedHan = null,
            int? expectedFu = null,
            int? expectedYakuCount = null,
            List<int>? doraIndicators = null,
            List<Meld>? melds = null,
            bool expectError = false)
        {
            testCount++;
            Console.WriteLine($"  {testName}");
            
            try
            {
                var result = calculator.EstimateHandValue(
                    tiles, 
                    winTile, 
                    melds, 
                    doraIndicators ?? new List<int>(), 
                    config);
                
                if (expectError)
                {
                    if (result.Error != null)
                    {
                        Console.WriteLine($"    ✓ 期待通りエラー: {result.Error}");
                        passCount++;
                    }
                    else
                    {
                        Console.WriteLine($"    ❌ エラーが期待されましたが成功: {result.Han}翻{result.Fu}符");
                        failCount++;
                    }
                    return;
                }
                
                if (result.Error != null)
                {
                    Console.WriteLine($"    ❌ エラー: {result.Error}");
                    failCount++;
                    return;
                }

                var actualHan = result.Han ?? 0;
                var actualFu = result.Fu ?? 0;
                var actualYakuCount = result.Yaku?.Count ?? 0;
                var passed = true;

                if (expectedHan.HasValue && actualHan != expectedHan.Value)
                {
                    Console.WriteLine($"    ❌ 翻数不一致: 期待{expectedHan}翻 実際{actualHan}翻");
                    passed = false;
                }

                if (expectedFu.HasValue && actualFu != expectedFu.Value)
                {
                    Console.WriteLine($"    ❌ 符数不一致: 期待{expectedFu}符 実際{actualFu}符");
                    passed = false;
                }

                if (expectedYakuCount.HasValue && actualYakuCount != expectedYakuCount.Value)
                {
                    Console.WriteLine($"    ❌ 役数不一致: 期待{expectedYakuCount}種類 実際{actualYakuCount}種類");
                    if (result.Yaku != null)
                    {
                        Console.WriteLine($"       実際の役: {string.Join(", ", result.Yaku.Select(y => y.Name))}");
                    }
                    passed = false;
                }

                if (passed)
                {
                    Console.WriteLine($"    ✓ 成功: {actualHan}翻{actualFu}符 役{actualYakuCount}種類");
                    if (result.Yaku != null && result.Yaku.Any())
                    {
                        Console.WriteLine($"       役: {string.Join(", ", result.Yaku.Select(y => y.Name))}");
                    }
                    passCount++;
                }
                else
                {
                    failCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ❌ 例外: {ex.Message}");
                failCount++;
            }
        }
    }
}
