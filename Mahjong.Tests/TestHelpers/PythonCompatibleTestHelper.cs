using System;
using System.Collections.Generic;
using System.Linq;
using Mahjong;
using Mahjong.HandCalculating;

namespace Mahjong.Tests.TestHelpers;

/// <summary>
/// Python麻雀ライブラリとの互換性を保つテストヘルパークラス
/// </summary>
public static class PythonCompatibleTestHelper
{
    /// <summary>
    /// 文字列から136形式の牌配列に変換（Python互換）
    /// 重複牌を正しく処理できるよう改善
    /// </summary>
    public static List<int> StringTo136Array(string man = "", string pin = "", string sou = "", string honors = "")
    {
        var tiles = new List<int>();
        var tileUsageCount = new Dictionary<int, int>(); // 各牌の使用回数を追跡
        
        // 萬子 (0-35)
        foreach (char c in man)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                int tileIndex = value * 4;
                int usageCount = tileUsageCount.GetValueOrDefault(tileIndex, 0);
                tiles.Add(tileIndex + usageCount); // 0, 1, 2, 3の順で使用
                tileUsageCount[tileIndex] = Math.Min(usageCount + 1, 3); // 最大4枚まで
            }
        }
        
        // 筒子 (36-71) 
        foreach (char c in pin)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                int tileIndex = 36 + value * 4;
                int usageCount = tileUsageCount.GetValueOrDefault(tileIndex, 0);
                tiles.Add(tileIndex + usageCount);
                tileUsageCount[tileIndex] = Math.Min(usageCount + 1, 3);
            }
        }
        
        // 索子 (72-107)
        foreach (char c in sou)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                int tileIndex = 72 + value * 4;
                int usageCount = tileUsageCount.GetValueOrDefault(tileIndex, 0);
                tiles.Add(tileIndex + usageCount);
                tileUsageCount[tileIndex] = Math.Min(usageCount + 1, 3);
            }
        }
        
        // 字牌 (108-135)
        foreach (char c in honors)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                int tileIndex = 108 + value * 4;
                int usageCount = tileUsageCount.GetValueOrDefault(tileIndex, 0);
                tiles.Add(tileIndex + usageCount);
                tileUsageCount[tileIndex] = Math.Min(usageCount + 1, 3);
            }
        }
        
        return tiles;
    }
    
    /// <summary>
    /// 文字列から34形式の牌配列に変換（Python互換）
    /// </summary>
    public static List<int> StringTo34Array(string man = "", string pin = "", string sou = "", string honors = "")
    {
        var tiles = new int[34];
        
        // 萬子 (0-8)
        foreach (char c in man)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                tiles[value]++;
            }
        }
        
        // 筒子 (9-17)
        foreach (char c in pin)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                tiles[9 + value]++;
            }
        }
        
        // 索子 (18-26)
        foreach (char c in sou)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                tiles[18 + value]++;
            }
        }
        
        // 字牌 (27-33)
        foreach (char c in honors)
        {
            if (char.IsDigit(c))
            {
                int value = c - '1';
                tiles[27 + value]++;
            }
        }
        
        return tiles.ToList();
    }
    
    /// <summary>
    /// 文字列から単一の136形式の牌に変換（"5s", "3m", "7p", "1z"形式をサポート）
    /// </summary>
    public static int StringTo136Tile(string tile)
    {
        if (string.IsNullOrEmpty(tile) || tile.Length != 2)
            throw new ArgumentException("Tile must be in format like '5s', '3m', '7p', '1z'");
            
        var numberChar = tile[0];
        var suitChar = tile[1];
        
        if (!char.IsDigit(numberChar))
            throw new ArgumentException("First character must be a digit");
            
        var value = numberChar - '1';
        int tileIndex;
        
        switch (suitChar)
        {
            case 'm': // 萬子 (0-35)
                tileIndex = value * 4;
                break;
            case 'p': // 筒子 (36-71)
                tileIndex = 36 + value * 4;
                break;
            case 's': // 索子 (72-107)
                tileIndex = 72 + value * 4;
                break;
            case 'z': // 字牌 (108-135)
                tileIndex = 108 + value * 4;
                break;
            default:
                throw new ArgumentException($"Invalid suit character: {suitChar}");
        }
        
        return tileIndex;
    }

    /// <summary>
    /// 文字列から単一の136形式の牌に変換（旧バージョン、互換性のため残す）
    /// </summary>
    public static int StringTo136Tile(string man = "", string pin = "", string sou = "", string honors = "")
    {
        var tiles = StringTo136Array(man, pin, sou, honors);
        return tiles.First();
    }
    
    /// <summary>
    /// 文字列から単一の34形式の牌に変換
    /// </summary>
    public static int StringTo34Tile(string man = "", string pin = "", string sou = "", string honors = "")
    {
        // 萬子 (0-8)
        if (!string.IsNullOrEmpty(man) && char.IsDigit(man[0]))
        {
            return man[0] - '1';
        }
        
        // 筒子 (9-17)
        if (!string.IsNullOrEmpty(pin) && char.IsDigit(pin[0]))
        {
            return 9 + (pin[0] - '1');
        }
        
        // 索子 (18-26)
        if (!string.IsNullOrEmpty(sou) && char.IsDigit(sou[0]))
        {
            return 18 + (sou[0] - '1');
        }
        
        // 字牌 (27-33)
        if (!string.IsNullOrEmpty(honors) && char.IsDigit(honors[0]))
        {
            return 27 + (honors[0] - '1');
        }
        
        throw new ArgumentException("Invalid tile specification");
    }
    
    /// <summary>
    /// テスト用のMeld作成
    /// </summary>
    public static Meld MakeMeld(string meldType, bool isOpen = true, string man = "", string pin = "", string sou = "", string honors = "")
    {
        var tiles = StringTo136Array(man, pin, sou, honors);
        return new Meld(
            meldType: meldType,
            tiles: tiles,
            opened: isOpen,
            calledTile: tiles.First(),
            who: 0,
            fromWho: 0
        );
    }
    
    /// <summary>
    /// テスト用のHandConfig作成
    /// </summary>
    public static HandConfig MakeHandConfig(
        bool isTsumo = false,
        bool isRiichi = false,
        bool isIppatsu = false,
        bool isRinshan = false,
        bool isChankan = false,
        bool isHaitei = false,
        bool isHoutei = false,
        bool isDaburuRiichi = false,
        bool isNagashiMangan = false,
        bool isTenhou = false,
        bool isRenhou = false,
        bool isChiihou = false,
        int? playerWind = null,
        int? roundWind = null,
        bool hasOpenTanyao = false,
        bool hasAkaDora = false,
        bool disableDoubleYakuman = false,
        bool renhouAsYakuman = false,
        bool allowDaisharin = false,
        bool allowDaisharinOtherSuits = false,
        bool isOpenRiichi = false,
        bool hasSashikomiYakuman = false,
        bool limitToSextupleYakuman = true,
        bool paarenchanNeedsYaku = true,
        bool hasDaichisei = false,
        int paarenchan = 0)
    {
        return new HandConfig
        {
            IsTsumo = isTsumo,
            IsRiichi = isRiichi,
            IsIppatsu = isIppatsu,
            IsRinshan = isRinshan,
            IsChankan = isChankan,
            IsHaitei = isHaitei,
            IsHoutei = isHoutei,
            IsDaburuRiichi = isDaburuRiichi,
            IsNagashiMangan = isNagashiMangan,
            IsTenhou = isTenhou,
            IsRenhou = isRenhou,
            IsChiihou = isChiihou,
            PlayerWind = playerWind,
            RoundWind = roundWind,
            Options = new OptionalRules
            {
                HasOpenTanyao = hasOpenTanyao,
                HasAkaDora = hasAkaDora,
                HasDoubleYakuman = !disableDoubleYakuman,
                RenhouAsYakuman = renhouAsYakuman,
                HasDaisharin = allowDaisharin,
                HasDaisharinOtherSuits = allowDaisharinOtherSuits,
                HasDaichisei = hasDaichisei,
                HasSashikomiYakuman = hasSashikomiYakuman,
                LimitToSextupleYakuman = limitToSextupleYakuman,
                PaarenchanNeedsYaku = paarenchanNeedsYaku
            },
            IsOpenRiichi = isOpenRiichi,
            Paarenchan = paarenchan
        };
    }
}