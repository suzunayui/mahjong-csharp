namespace Mahjong.HandCalculating.YakuList;

public class Toitoi : Yaku
{
    public Toitoi(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Toitoi";
        HanOpen = 2;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // All melds must be pon/kan (triplets/quads), no sequences allowed
        var meldsCount = hand.Count(meld => Utils.IsPonOrKan(meld.ToList()));
        return meldsCount == 4; // Should have 4 pon/kan + 1 pair
    }
}
