namespace Mahjong.HandCalculating.YakuList;

public class Iipeiko : Yaku
{
    public Iipeiko(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Iipeiko";
        HanOpen = null; // Only available for closed hands
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Iipeiko is only valid for closed hands
        if (args.Length > 0 && args[0] is bool isOpenHand && isOpenHand)
        {
            return false;
        }

        var chiSets = hand.Where(set => Utils.IsChi(set.ToList())).ToList();

        // Group identical sequences
        var sequenceGroups = chiSets.GroupBy(set => string.Join(",", set.OrderBy(x => x)))
                                   .Select(g => g.Count())
                                   .OrderByDescending(count => count)
                                   .ToList();

        // Iipeiko requires at least one pair of identical sequences, but not ryanpeiko
        return sequenceGroups.Count > 0 && sequenceGroups[0] >= 2 && 
               !(sequenceGroups.Count == 2 && sequenceGroups[0] == 2 && sequenceGroups[1] == 2);
    }
}
