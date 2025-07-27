namespace Mahjong.HandCalculating.YakuList;

public class Ryanpeiko : Yaku
{
    public Ryanpeiko(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Ryanpeiko";
        HanOpen = null; // Only available for closed hands
        HanClosed = 3;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Ryanpeiko is only valid for closed hands
        if (args.Length > 0 && args[0] is bool isOpenHand && isOpenHand)
        {
            return false;
        }

        var chiSets = hand.Where(set => Utils.IsChi(set.ToList())).ToList();

        if (chiSets.Count != 4)
            return false;

        // Group identical sequences
        var sequenceGroups = chiSets.GroupBy(set => string.Join(",", set.OrderBy(x => x)))
                                   .Select(g => g.Count())
                                   .OrderByDescending(count => count)
                                   .ToList();

        // Ryanpeiko requires exactly two pairs of identical sequences (2+2)
        return sequenceGroups.Count == 2 && sequenceGroups[0] == 2 && sequenceGroups[1] == 2;
    }
}
