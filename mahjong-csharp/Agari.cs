namespace Mahjong;

public class Agari
{
    public bool IsAgari(IReadOnlyList<int> tiles34, IEnumerable<IReadOnlyList<int>>? openSets34 = null)
    {
        var tiles = tiles34.ToArray();

        if (openSets34 != null)
        {
            var isolatedTiles = Utils.FindIsolatedTileIndices(tiles).ToList();
            foreach (var meld in openSets34)
            {
                if (!isolatedTiles.Any())
                    break;

                var isolatedTile = isolatedTiles.Last();
                isolatedTiles.RemoveAt(isolatedTiles.Count - 1);

                tiles[meld[0]]--;
                tiles[meld[1]]--;
                tiles[meld[2]]--;
                
                if (meld.Count > 3)
                    tiles[meld[3]]--;
                
                tiles[isolatedTile] = 3;
            }
        }

        var j = (1 << tiles[27]) | (1 << tiles[28]) | (1 << tiles[29]) | (1 << tiles[30]) |
                (1 << tiles[31]) | (1 << tiles[32]) | (1 << tiles[33]);

        if (j >= 0x10)
            return false;

        // 13 orphans
        if ((j & 3) == 2 && tiles[0] * tiles[8] * tiles[9] * tiles[17] * tiles[18] * tiles[26] *
            tiles[27] * tiles[28] * tiles[29] * tiles[30] * tiles[31] * tiles[32] * tiles[33] == 2)
        {
            return true;
        }

        // seven pairs
        if ((j & 10) == 0 && tiles.Take(34).Count(t => t == 2) == 7)
        {
            return true;
        }

        // Chuuren Poutou (Nine Gates) - check for pattern 1112345678999 + one more
        if ((j & 14) == 0) // no honors
        {
            // Check if all tiles are from one suit only
            var manCount = tiles.Take(9).Sum();
            var pinCount = tiles.Skip(9).Take(9).Sum();
            var souCount = tiles.Skip(18).Take(9).Sum();

            var singleSuitCounts = new[] { manCount, pinCount, souCount };
            var nonZeroSuits = singleSuitCounts.Count(c => c > 0);

            if (nonZeroSuits == 1 && singleSuitCounts.Sum() == 14) // exactly one suit with 14 tiles
            {
                int suitOffset = 0;
                if (pinCount > 0) suitOffset = 9;
                else if (souCount > 0) suitOffset = 18;

                // Check Chuuren Poutou pattern: 1112345678999
                if (tiles[suitOffset] >= 3 && tiles[suitOffset + 8] >= 3) // at least 3 of 1 and 9
                {
                    var remaining = tiles.Skip(suitOffset).Take(9).ToArray();
                    remaining[0] -= 3; // remove 3 of tile 1
                    remaining[8] -= 3; // remove 3 of tile 9

                    // After removing 1-1-1 and 9-9-9, should have exactly one of each 1-9 plus one extra
                    var expectedPattern = new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 }; // one of each
                    var difference = remaining.Zip(expectedPattern, (a, e) => a - e).ToArray();
                    
                    // Should have exactly one extra tile in one position, and all others should be exactly 1
                    var extraCount = difference.Count(d => d == 1);
                    var correctCount = difference.Count(d => d == 0);
                    var negativeCount = difference.Count(d => d < 0);

                    if (extraCount == 1 && correctCount == 8 && negativeCount == 0)
                    {
                        return true;
                    }
                }
            }
        }

        if ((j & 2) != 0)
            return false;

        var n00 = tiles[0] + tiles[3] + tiles[6];
        var n01 = tiles[1] + tiles[4] + tiles[7];
        var n02 = tiles[2] + tiles[5] + tiles[8];

        var n10 = tiles[9] + tiles[12] + tiles[15];
        var n11 = tiles[10] + tiles[13] + tiles[16];
        var n12 = tiles[11] + tiles[14] + tiles[17];

        var n20 = tiles[18] + tiles[21] + tiles[24];
        var n21 = tiles[19] + tiles[22] + tiles[25];
        var n22 = tiles[20] + tiles[23] + tiles[26];

        var n0 = (n00 + n01 + n02) % 3;
        if (n0 == 1) return false;

        var n1 = (n10 + n11 + n12) % 3;
        if (n1 == 1) return false;

        var n2 = (n20 + n21 + n22) % 3;
        if (n2 == 1) return false;

        var pairCount = (n0 == 2 ? 1 : 0) + (n1 == 2 ? 1 : 0) + (n2 == 2 ? 1 : 0) +
                       (tiles[27] == 2 ? 1 : 0) + (tiles[28] == 2 ? 1 : 0) + (tiles[29] == 2 ? 1 : 0) +
                       (tiles[30] == 2 ? 1 : 0) + (tiles[31] == 2 ? 1 : 0) + (tiles[32] == 2 ? 1 : 0) +
                       (tiles[33] == 2 ? 1 : 0);

        if (pairCount != 1)
            return false;

        var nn0 = (n00 * 1 + n01 * 2) % 3;
        var m0 = ToMeld(tiles, 0);
        var nn1 = (n10 * 1 + n11 * 2) % 3;
        var m1 = ToMeld(tiles, 9);
        var nn2 = (n20 * 1 + n21 * 2) % 3;
        var m2 = ToMeld(tiles, 18);

        if ((j & 4) != 0)
        {
            return (n0 | nn0 | n1 | nn1 | n2 | nn2) == 0 &&
                   IsMentsu(m0) && IsMentsu(m1) && IsMentsu(m2);
        }

        if (n0 == 2)
        {
            return (n1 | nn1 | n2 | nn2) == 0 &&
                   IsMentsu(m1) && IsMentsu(m2) && IsAtamaMentsu(nn0, m0);
        }

        if (n1 == 2)
        {
            return (n2 | nn2 | n0 | nn0) == 0 &&
                   IsMentsu(m2) && IsMentsu(m0) && IsAtamaMentsu(nn1, m1);
        }

        if (n2 == 2)
        {
            return (n0 | nn0 | n1 | nn1) == 0 &&
                   IsMentsu(m0) && IsMentsu(m1) && IsAtamaMentsu(nn2, m2);
        }

        return false;
    }

    private bool IsMentsu(int m)
    {
        var a = m & 7;
        var b = 0;
        var c = 0;
        
        if (a == 1 || a == 4)
        {
            b = c = 1;
        }
        else if (a == 2)
        {
            b = c = 2;
        }
        
        m >>= 3;
        a = (m & 7) - b;

        if (a < 0) return false;

        for (int i = 0; i < 6; i++)
        {
            b = c;
            c = 0;
            
            if (a == 1 || a == 4)
            {
                b += 1;
                c += 1;
            }
            else if (a == 2)
            {
                b += 2;
                c += 2;
            }
            
            m >>= 3;
            a = (m & 7) - b;
            
            if (a < 0) return false;
        }

        m >>= 3;
        a = (m & 7) - c;

        return a == 0 || a == 3;
    }

    private bool IsAtamaMentsu(int nn, int m)
    {
        if (nn == 0)
        {
            if ((m & (7 << 6)) >= (2 << 6) && IsMentsu(m - (2 << 6)))
                return true;
            if ((m & (7 << 15)) >= (2 << 15) && IsMentsu(m - (2 << 15)))
                return true;
            if ((m & (7 << 24)) >= (2 << 24) && IsMentsu(m - (2 << 24)))
                return true;
        }
        else if (nn == 1)
        {
            if ((m & (7 << 3)) >= (2 << 3) && IsMentsu(m - (2 << 3)))
                return true;
            if ((m & (7 << 12)) >= (2 << 12) && IsMentsu(m - (2 << 12)))
                return true;
            if ((m & (7 << 21)) >= (2 << 21) && IsMentsu(m - (2 << 21)))
                return true;
        }
        else if (nn == 2)
        {
            if ((m & (7 << 0)) >= (2 << 0) && IsMentsu(m - (2 << 0)))
                return true;
            if ((m & (7 << 9)) >= (2 << 9) && IsMentsu(m - (2 << 9)))
                return true;
            if ((m & (7 << 18)) >= (2 << 18) && IsMentsu(m - (2 << 18)))
                return true;
        }
        
        return false;
    }

    private int ToMeld(int[] tiles, int d)
    {
        var result = 0;
        for (int i = 0; i < 9; i++)
        {
            result |= tiles[d + i] << (i * 3);
        }
        return result;
    }
}