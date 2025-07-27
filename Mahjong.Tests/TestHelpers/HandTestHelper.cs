using System;
using System.Collections.Generic;
using System.Linq;

namespace Mahjong.Tests.TestHelpers;

/// <summary>
/// 麻雀のテスト用ヘルパークラス
/// </summary>
public static class HandTestHelper
{
    /// <summary>
    /// 牌のインデックスを人間が読みやすい文字列に変換
    /// </summary>
    public static string TileIndexToString(int index)
    {
        return index switch
        {
            // 萬子 (0-8)
            0 => "1m", 1 => "2m", 2 => "3m", 3 => "4m", 4 => "5m", 5 => "6m", 6 => "7m", 7 => "8m", 8 => "9m",
            // 筒子 (9-17)
            9 => "1p", 10 => "2p", 11 => "3p", 12 => "4p", 13 => "5p", 14 => "6p", 15 => "7p", 16 => "8p", 17 => "9p",
            // 索子 (18-26)  
            18 => "1s", 19 => "2s", 20 => "3s", 21 => "4s", 22 => "5s", 23 => "6s", 24 => "7s", 25 => "8s", 26 => "9s",
            // 字牌 (27-33)
            27 => "東", 28 => "南", 29 => "西", 30 => "北", 31 => "白", 32 => "發", 33 => "中",
            _ => $"?{index}"
        };
    }

    /// <summary>
    /// 手牌を人間が読みやすい文字列に変換
    /// </summary>
    public static string HandToString(IEnumerable<IEnumerable<int>> hand)
    {
        var allTiles = hand.SelectMany(set => set).OrderBy(tile => tile).ToList();
        return string.Join(" ", allTiles.Select(TileIndexToString));
    }

    /// <summary>
    /// 手牌の詳細情報を表示
    /// </summary>
    public static void DisplayHandInfo(string yakuName, IEnumerable<IEnumerable<int>> hand)
    {
        var allTiles = hand.SelectMany(set => set).OrderBy(tile => tile).ToList();
        var tileStrings = allTiles.Select(TileIndexToString);
        
        Console.WriteLine($"=== {yakuName} ===");
        Console.WriteLine($"14枚: {string.Join(" ", tileStrings)}");
        Console.WriteLine($"面子構成:");
        foreach (var set in hand)
        {
            var setTiles = set.Select(TileIndexToString);
            var setType = set.Count() switch
            {
                2 => "雀頭",
                3 when set.All(tile => tile == set.First()) => "刻子",
                3 => "順子",
                4 => "槓子",
                _ => "不明"
            };
            Console.WriteLine($"  {setType}: {string.Join(" ", setTiles)}");
        }
        Console.WriteLine($"--- {yakuName} 終了 ---");
        Console.WriteLine();
    }

    /// <summary>
    /// テスト用の手牌例
    /// </summary>
    public static class TestHands
    {
        /// <summary>一盃口の例: 123m123m456p789s11z</summary>
        public static List<List<int>> Iipeiko => new()
        {
            new() { 0, 1, 2 },    // 123m
            new() { 0, 1, 2 },    // 123m (identical)
            new() { 12, 13, 14 }, // 456p
            new() { 24, 25, 26 }, // 789s
            new() { 27, 27 }      // 11z
        };

        /// <summary>断么九の例: 234m345p456s567m88m</summary>
        public static List<List<int>> Tanyao => new()
        {
            new() { 1, 2, 3 },    // 234m
            new() { 12, 13, 14 }, // 345p
            new() { 21, 22, 23 }, // 456s
            new() { 4, 5, 6 },    // 567m
            new() { 7, 7 }        // 88m
        };

        /// <summary>大三元の例: 111m白白白發發發中中中22p</summary>
        public static List<List<int>> Daisangen => new()
        {
            new() { 0, 0, 0 },    // 111m
            new() { 31, 31, 31 }, // 白白白
            new() { 32, 32, 32 }, // 發發發
            new() { 33, 33, 33 }, // 中中中
            new() { 10, 10 }      // 22p
        };
    }
}
