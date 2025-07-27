namespace Mahjong.HandCalculating.YakuList;

public class Renhou : Yaku
{
    public Renhou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Renhou";
        HanOpen = null; // Only available for closed hands
        HanClosed = 5;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code
        return true;
    }
}
