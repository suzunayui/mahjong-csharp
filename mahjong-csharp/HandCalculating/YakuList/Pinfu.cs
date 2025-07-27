using Mahjong;

namespace Mahjong.HandCalculating.YakuList;

public class Pinfu : Yaku
{
    public Pinfu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Pinfu";
        HanOpen = null;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Pinfu is only valid for closed hands (門前清)
        if (args.Length > 0 && args[0] is bool isOpenHand && isOpenHand)
        {
            return false;
        }

        // Extract additional parameters for strict validation
        int winTile = args.Length > 1 && args[1] is int wt ? wt : -1;
        int? playerWind = args.Length > 2 && args[2] is int pw ? pw : null;
        int? roundWind = args.Length > 3 && args[3] is int rw ? rw : null;

        var handList = hand.ToList();
        
        // Check for any triplets (刻子) - Pinfu cannot have triplets
        var triplets = handList.Where(set => Utils.IsPon(set.ToList())).ToList();
        if (triplets.Any())
        {
            return false;
        }
        
        var sequences = handList.Where(set => Utils.IsChi(set.ToList())).ToList();
        var pairs = handList.Where(set => Utils.IsPair(set.ToList())).ToList();
        
        // Must have exactly 4 sequences and 1 pair (no triplets, no kans)
        if (sequences.Count != 4 || pairs.Count != 1 || handList.Count != 5)
        {
            return false;
        }

        // Check if pair is yakuhai (valued tiles)
        var pairTile = pairs[0].First();
        if (IsYakuhaiTile(pairTile, playerWind, roundWind))
        {
            return false;
        }

        // Check wait type - must be ryanmen (two-sided wait)
        if (winTile != -1 && !IsRyanmenWait(sequences, winTile))
        {
            return false;
        }
        
        return true;
    }

    private bool IsYakuhaiTile(int tile, int? playerWind, int? roundWind)
    {
        // Honor tiles (winds and dragons)
        if (tile >= Constants.East)
        {
            // Dragons are always yakuhai
            if (tile >= Constants.Haku)
            {
                return true;
            }

            // Winds are yakuhai if they match player wind or round wind
            if (playerWind != null && tile == playerWind)
            {
                return true;
            }
            if (roundWind != null && tile == roundWind)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsRyanmenWait(List<IEnumerable<int>> sequences, int winTile)
    {
        // Find which sequence contains the winning tile
        foreach (var sequence in sequences)
        {
            var seqList = sequence.OrderBy(t => t).ToList();
            if (seqList.Contains(winTile))
            {
                // Convert to 34-tile representation for easier checking
                var tile34Values = seqList.Select(t => t / 4).ToList();
                var winTile34 = winTile / 4;
                
                // Sort the sequence values
                tile34Values.Sort();
                
                // Must be a valid sequence (consecutive numbers in same suit)
                if (tile34Values[1] != tile34Values[0] + 1 || tile34Values[2] != tile34Values[1] + 1)
                {
                    return false; // Not a valid sequence
                }
                
                // Check for penchan (edge wait)
                // 123 sequence: if we won with tile 1 or 3, it's penchan
                // 789 sequence: if we won with tile 7 or 9, it's penchan
                if ((tile34Values[0] % 9 == 0 && winTile34 % 9 == 0) ||  // Won with 1 in 123
                    (tile34Values[0] % 9 == 0 && winTile34 % 9 == 2) ||  // Won with 3 in 123
                    (tile34Values[2] % 9 == 8 && winTile34 % 9 == 6) ||  // Won with 7 in 789
                    (tile34Values[2] % 9 == 8 && winTile34 % 9 == 8))    // Won with 9 in 789
                {
                    return false; // This is penchan
                }
                
                // Check for kanchan (middle wait)
                // If we won with the middle tile of the sequence, it's kanchan
                if (winTile34 == tile34Values[1])
                {
                    return false; // This is kanchan
                }
                
                // If it's not penchan or kanchan, it should be ryanmen
                return true;
            }
        }
        
        // If winning tile is not found in any sequence, it might be a pair wait (tanki)
        // which is not allowed for Pinfu
        return false;
    }
}