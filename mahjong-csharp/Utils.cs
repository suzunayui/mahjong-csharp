namespace Mahjong;

public static class Utils
{
    public static bool IsAkaDora(int tile136, bool akaEnabled)
    {
        if (!akaEnabled) return false;
        return Constants.AkaDoraList.Contains(tile136);
    }

    public static int PlusDora(int tile136, IEnumerable<int> doraIndicators136, bool addAkaDora = false)
    {
        var tileIndex = tile136 / 4;
        var doraCount = 0;

        if (addAkaDora && IsAkaDora(tile136, true))
        {
            doraCount++;
        }

        foreach (var dora in doraIndicators136)
        {
            var doraIndex = dora / 4;

            // sou, pin, man
            if (tileIndex < Constants.East)
            {
                // with indicator 9, dora will be 1
                if (doraIndex == 8) doraIndex = -1;
                else if (doraIndex == 17) doraIndex = 8; 
                else if (doraIndex == 26) doraIndex = 17;

                if (tileIndex == doraIndex + 1)
                    doraCount++;
            }
            else
            {
                if (doraIndex < Constants.East) continue;

                doraIndex -= 9 * 3;
                var tileIndexTemp = tileIndex - 9 * 3;

                // dora indicator is north, next dora is east
                if (doraIndex == 3) doraIndex = -1;
                // dora indicator is hatsu, next dora is north  
                if (doraIndex == 6) doraIndex = 3;

                if (tileIndexTemp == doraIndex + 1)
                    doraCount++;
            }
        }

        return doraCount;
    }

    public static bool IsChi(IReadOnlyList<int> item)
    {
        if (item.Count != 3) return false;
        return item[0] == item[1] - 1 && item[1] == item[2] - 1;
    }

    public static bool IsPon(IReadOnlyList<int> item)
    {
        if (item.Count != 3) return false;
        return item[0] == item[1] && item[1] == item[2];
    }

    public static bool IsKan(IReadOnlyList<int> item)
    {
        return item.Count == 4;
    }

    public static bool IsPonOrKan(IReadOnlyList<int> item)
    {
        return IsPon(item) || IsKan(item);
    }

    public static bool IsPair(IReadOnlyList<int> item)
    {
        return item.Count == 2;
    }

    public static bool IsMan(int tile)
    {
        return tile <= 8;
    }

    public static bool IsPin(int tile)
    {
        return tile > 8 && tile <= 17;
    }

    public static bool IsSou(int tile)
    {
        return tile > 17 && tile <= 26;
    }

    public static bool IsHonor(int tile)
    {
        return tile >= 27;
    }

    public static bool IsWind(int tile)
    {
        return tile >= 27 && tile <= 30;
    }

    public static bool IsSangenpai(int tile34)
    {
        return tile34 >= 31;
    }

    public static bool IsTerminal(int tile)
    {
        return Constants.TerminalIndices.Contains(tile);
    }

    public static bool IsDoraIndicatorForTerminal(int tile)
    {
        return tile == 7 || tile == 8 || tile == 16 || tile == 17 || tile == 25 || tile == 26;
    }

    public static bool ContainsTerminals(IEnumerable<int> handSet)
    {
        return handSet.Any(x => Constants.TerminalIndices.Contains(x));
    }

    public static int Simplify(int tile)
    {
        return tile - 9 * (tile / 9);
    }

    public static List<int> FindIsolatedTileIndices(IReadOnlyList<int> hand34)
    {
        var isolatedIndices = new List<int>();

        for (int x = 0; x <= Constants.Chun; x++)
        {
            if (IsHonor(x) && hand34[x] == 0)
            {
                isolatedIndices.Add(x);
            }
            else
            {
                var simplified = Simplify(x);

                // 1 suit tile
                if (simplified == 0)
                {
                    if (hand34[x] == 0 && hand34[x + 1] == 0)
                        isolatedIndices.Add(x);
                }
                // 9 suit tile
                else if (simplified == 8)
                {
                    if (hand34[x] == 0 && hand34[x - 1] == 0)
                        isolatedIndices.Add(x);
                }
                // 2-8 tiles
                else
                {
                    if (hand34[x] == 0 && hand34[x - 1] == 0 && hand34[x + 1] == 0)
                        isolatedIndices.Add(x);
                }
            }
        }

        return isolatedIndices;
    }

    public static bool IsTileStrictlyIsolated(IReadOnlyList<int> hand34, int tile34)
    {
        if (IsHonor(tile34))
        {
            return hand34[tile34] - 1 <= 0;
        }

        var simplified = Simplify(tile34);
        List<int> indices;

        // 1 suit tile
        if (simplified == 0)
        {
            indices = new List<int> { tile34, tile34 + 1, tile34 + 2 };
        }
        // 2 suit tile
        else if (simplified == 1)
        {
            indices = new List<int> { tile34 - 1, tile34, tile34 + 1, tile34 + 2 };
        }
        // 8 suit tile
        else if (simplified == 7)
        {
            indices = new List<int> { tile34 - 2, tile34 - 1, tile34, tile34 + 1 };
        }
        // 9 suit tile
        else if (simplified == 8)
        {
            indices = new List<int> { tile34 - 2, tile34 - 1, tile34 };
        }
        // 3-7 tiles
        else
        {
            indices = new List<int> { tile34 - 2, tile34 - 1, tile34, tile34 + 1, tile34 + 2 };
        }

        var isolated = true;
        foreach (var tileIndex in indices)
        {
            if (tileIndex == tile34)
            {
                isolated &= hand34[tileIndex] - 1 <= 0;
            }
            else
            {
                isolated &= hand34[tileIndex] == 0;
            }
        }

        return isolated;
    }

    public static List<SuitCount> CountTilesBySuits(IReadOnlyList<int> tiles34)
    {
        var suits = new List<SuitCount>
        {
            new() { Count = 0, Name = "sou", Function = IsSou },
            new() { Count = 0, Name = "man", Function = IsMan },
            new() { Count = 0, Name = "pin", Function = IsPin },
            new() { Count = 0, Name = "honor", Function = IsHonor }
        };

        for (int x = 0; x < 34; x++)
        {
            var tile = tiles34[x];
            if (tile == 0) continue;

            foreach (var suit in suits)
            {
                if (suit.Function(x))
                {
                    suit.Count += tile;
                }
            }
        }

        return suits;
    }
}

public class SuitCount
{
    public int Count { get; set; }
    public string Name { get; set; } = "";
    public Func<int, bool> Function { get; set; } = _ => false;
}