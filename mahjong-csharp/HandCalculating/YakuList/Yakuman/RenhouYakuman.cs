namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class RenhouYakuman : Yaku
{
    public RenhouYakuman(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Renhou (yakuman)";
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
