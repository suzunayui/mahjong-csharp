namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Tenhou : Yaku
{
    public Tenhou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Tenhou";
        HanOpen = null; // Only available for closed hands
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code
        return true;
    }
}
