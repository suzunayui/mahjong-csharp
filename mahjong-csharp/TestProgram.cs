using System;
using System.Collections.Generic;
using System.Linq;
using Mahjong;
using Mahjong.HandCalculating;
using Mahjong.Test;

namespace Mahjong
{
    public class TestProgram
    {
        public static void TestHandCalculator()
        {
            Console.WriteLine("HandCalculator Python互換テストを開始します...");
            
            // Python完全互換テストを実行
            PythonCompatibilityTests.RunAllTests();
            
            // Python等価テストを実行
            PythonEquivalentTests.RunAllTests();
            
            Console.WriteLine("\n=== 従来のテストケース ===");
            var calculator = new HandCalculator();
            
            // Python test: test_is_pinfu_hand - basic pinfu
            TestHand(calculator, "Test 1: Pinfu基本形", 
                TilesConverter.OneLineStringTo136Array("123456s123456m55p"), 
                TilesConverter.OneLineStringTo136Array("6m")[0],
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedRole: "Pinfu");

            // Python test: test_is_tanyao - basic tanyao
            TestHand(calculator, "Test 2: Tanyao基本形",
                TilesConverter.OneLineStringTo136Array("234567s234567m22p"), 
                TilesConverter.OneLineStringTo136Array("7m")[0],
                new HandConfig { IsTsumo = false, IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 3, expectedFu: 30, expectedRole: "Tanyao, Pinfu, Riichi");

            // Python test: test_is_tsumo - menzen tsumo
            TestHand(calculator, "Test 3: Menzen Tsumo",
                TilesConverter.OneLineStringTo136Array("123444s234456m66p"),
                TilesConverter.OneLineStringTo136Array("4s")[0],
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedRole: "Menzen Tsumo");

            // Python test: test_is_riichi - riichi
            TestHand(calculator, "Test 4: Riichi",
                TilesConverter.OneLineStringTo136Array("123444s234456m66p"),
                TilesConverter.OneLineStringTo136Array("4s")[0],
                new HandConfig { IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 40, expectedRole: "Riichi");

            // Python test: test_is_ryanpeiko - ryanpeiko
            TestHand(calculator, "Test 5: Ryanpeiko",
                TilesConverter.OneLineStringTo136Array("112233s33m223344p"),
                TilesConverter.OneLineStringTo136Array("3p")[0],
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 3, expectedFu: 40, expectedRole: "Ryanpeiko");

            // Python test: hand with dora
            TestHand(calculator, "Test 6: Dora付き手",
                TilesConverter.OneLineStringTo136Array("123456s123456m22p"),
                TilesConverter.OneLineStringTo136Array("6m")[0],
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                doraIndicators: new List<int> { TilesConverter.OneLineStringTo136Array("1m")[0] }, // 2mがドラ
                expectedHan: 2, expectedFu: 30, expectedRole: "Pinfu, Dora");

            // Python test: yakuhai east
            TestHand(calculator, "Test 7: 役牌（東）",
                TilesConverter.OneLineStringTo136Array("123s456m789s111z22p"),
                TilesConverter.OneLineStringTo136Array("2p")[0],
                new HandConfig { 
                    IsTsumo = false, 
                    PlayerWind = 27, // 東
                    RoundWind = 27,  // 東
                    Yaku = new YakuConfig() 
                },
                expectedHan: 1, expectedFu: 40, expectedRole: "Yakuhai (east)");

            // Python test: open hand tanyao
            TestHand(calculator, "Test 8: 鳴きタンヤオ",
                TilesConverter.OneLineStringTo136Array("234567m234567p22s"),
                TilesConverter.OneLineStringTo136Array("7p")[0],
                new HandConfig { 
                    IsTsumo = false, 
                    Yaku = new YakuConfig(), 
                    Options = new OptionalRules(hasOpenTanyao: true) 
                },
                melds: new List<Meld> { new Meld(Meld.Chi, TilesConverter.OneLineStringTo136Array("234s").Take(3)) },
                expectedHan: 1, expectedFu: 30, expectedRole: "Tanyao");

            // Simple winning hand
            TestHand(calculator, "Test 9: 基本和了形",
                TilesConverter.OneLineStringTo136Array("123345678s678m22p"),
                TilesConverter.OneLineStringTo136Array("3s")[0],
                new HandConfig { IsTsumo = false, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedRole: "Tanyao");

            // Simple riichi winning hand  
            TestHand(calculator, "Test 10: リーチ基本形",
                TilesConverter.OneLineStringTo136Array("12399s123456m456p"),
                TilesConverter.OneLineStringTo136Array("1s")[0],
                new HandConfig { IsRiichi = true, Yaku = new YakuConfig() },
                expectedHan: 1, expectedFu: 30, expectedRole: "Riichi");

            Console.WriteLine("\n=== テスト結果サマリー ===");
            Console.WriteLine($"実行テスト数: {testCount}");
            Console.WriteLine($"成功: {passCount}");
            Console.WriteLine($"失敗: {failCount}");
            Console.WriteLine("テスト完了");
            
            // Additional test for Ryanpeiko vs Chiitoitsu from Python tests
            Console.WriteLine("\n=== 二盃口 vs 七対子 テスト（Python準拠） ===");
            TestRyanpeikoVsChiitoitsu(calculator);
        }
        
        private static void TestRyanpeikoVsChiitoitsu(HandCalculator calculator)
        {
            // Test: 二盃口 - proper ryanpeiko hand: 112233m 445566p 77s
            // This should form: [123m][123m][456p][456p][77s] = ryanpeiko
            var testTiles = TilesConverter.OneLineStringTo136Array("112233m445566p77s");
            Console.WriteLine($"二盃口テスト牌: {string.Join(",", testTiles)}");
            
            // 34配列に変換して確認
            var tiles34 = TilesConverter.To34Array(testTiles);
            Console.WriteLine("34配列:");
            for (int i = 0; i < 34; i++)
            {
                if (tiles34[i] > 0)
                    Console.WriteLine($"  牌{i}: {tiles34[i]}枚");
            }
            
            TestHand(calculator, "二盃口テスト (4翻30符)",
                testTiles,
                TilesConverter.OneLineStringTo136Array("7s")[0], // 7s
                new HandConfig { IsTsumo = true, Yaku = new YakuConfig() },
                expectedHan: 4, expectedFu: 30, expectedRole: "Ryanpeiko, Menzen Tsumo"); // 二盃口3翻 + ツモ1翻
        }

        private static int testCount = 0;
        private static int passCount = 0;
        private static int failCount = 0;

        private static void TestHand(
            HandCalculator calculator,
            string testName,
            List<int> tiles,
            int winTile,
            HandConfig config,
            int? expectedHan = null,
            int? expectedFu = null,
            string? expectedRole = null,
            List<int>? doraIndicators = null,
            List<Meld>? melds = null)
        {
            testCount++;
            Console.WriteLine($"\n{testName}");
            
            // Debug HandDivider results for ryanpeiko test
            if (testName.Contains("二盃口"))
            {
                var handDivider = new HandDivider();
                var tiles34 = TilesConverter.To34Array(tiles);
                var possibleHands = handDivider.DivideHand(tiles34);
                
                Console.WriteLine($"HandDivider結果: {possibleHands.Count}個の分割");
                for (int i = 0; i < possibleHands.Count; i++)
                {
                    var hand = possibleHands[i];
                    Console.WriteLine($"  分割{i+1}: {hand.Count}組 - [{string.Join("], [", hand.Select(g => string.Join(",", g)))}]");
                }
            }
            
            try
            {
                var result = calculator.EstimateHandValue(
                    tiles, 
                    winTile, 
                    melds, 
                    doraIndicators ?? new List<int>(), 
                    config);
                
                if (result.Error != null)
                {
                    Console.WriteLine($"  ❌ エラー: {result.Error}");
                    failCount++;
                    return;
                }

                var actualHan = result.Han ?? 0;
                var actualFu = result.Fu ?? 0;
                var actualRoles = string.Join(", ", result.Yaku?.Select(y => y.Name ?? "Unknown") ?? new List<string>());
                var actualScore = result.Cost?.ContainsKey("main") == true ? result.Cost["main"] : 0;

                Console.WriteLine($"  翻数: {actualHan}, 符: {actualFu}");
                Console.WriteLine($"  役: {actualRoles}");
                Console.WriteLine($"  点数: {actualScore}");

                // 期待値チェック
                bool passed = true;
                if (expectedHan.HasValue && actualHan != expectedHan.Value)
                {
                    Console.WriteLine($"  ❌ 翻数不一致: 期待={expectedHan.Value}, 実際={actualHan}");
                    passed = false;
                }
                if (expectedFu.HasValue && actualFu != expectedFu.Value)
                {
                    Console.WriteLine($"  ❌ 符不一致: 期待={expectedFu.Value}, 実際={actualFu}");
                    passed = false;
                }
                if (!string.IsNullOrEmpty(expectedRole))
                {
                    var expectedRoles = expectedRole.Split(", ").OrderBy(x => x).ToList();
                    var actualRolesList = actualRoles.Split(", ").OrderBy(x => x).ToList();
                    if (!expectedRoles.SequenceEqual(actualRolesList))
                    {
                        Console.WriteLine($"  ❌ 役不一致: 期待=[{expectedRole}], 実際=[{actualRoles}]");
                        passed = false;
                    }
                }

                if (passed)
                {
                    Console.WriteLine("  ✅ PASS");
                    passCount++;
                }
                else
                {
                    failCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ 例外: {ex.Message}");
                failCount++;
            }
        }
    }
}
