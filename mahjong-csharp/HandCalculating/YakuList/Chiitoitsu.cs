namespace Mahjong.HandCalculating.YakuList;

public class Chiitoitsu : Yaku
{
    public Chiitoitsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Chiitoitsu";
        HanOpen = null; // Only available for closed hands
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Python implementation: simply check if hand has exactly 7 groups
        // The hand divider should already ensure these are valid pairs for chiitoitsu pattern
        return hand.Count() == 7;
    }
}