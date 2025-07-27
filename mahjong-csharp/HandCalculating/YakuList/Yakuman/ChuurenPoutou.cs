namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class ChuurenPoutou : Yaku
{
    public ChuurenPoutou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Chuuren Poutou";
        HanOpen = null; // Only available for closed hands
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var souSets = 0;
        var pinSets = 0;
        var manSets = 0;
        var honorSets = 0;

        foreach (var set in hand)
        {
            var firstTile = set.First();
            if (Utils.IsSou(firstTile))
                souSets++;
            else if (Utils.IsPin(firstTile))
                pinSets++;
            else if (Utils.IsMan(firstTile))
                manSets++;
            else
                honorSets++;
        }

        var suits = new[] { souSets, pinSets, manSets };
        var onlyOneSuit = suits.Count(x => x != 0) == 1;
        
        if (!onlyOneSuit || honorSets > 0)
            return false;

        var indices = hand.SelectMany(set => set).ToList();
        // Cast tile indices to 0..8 representation
        var simplifiedIndices = indices.Select(Utils.Simplify).ToList();

        // Must have at least 3 of tile 0 (1s)
        if (simplifiedIndices.Count(x => x == 0) < 3)
            return false;

        // Must have at least 3 of tile 8 (9s)
        if (simplifiedIndices.Count(x => x == 8) < 3)
            return false;

        // Remove the required 1-1 and 9-9 (keeping one for 1-2-3-4-5-6-7-8-9 sequence)
        simplifiedIndices.Remove(0);
        simplifiedIndices.Remove(0);
        simplifiedIndices.Remove(8);
        simplifiedIndices.Remove(8);

        // Remove one of each from 1-9
        for (int x = 0; x < 9; x++)
        {
            if (simplifiedIndices.Contains(x))
                simplifiedIndices.Remove(x);
        }

        // Should have exactly one tile left
        return simplifiedIndices.Count == 1;
    }
}
