namespace Mahjong.HandCalculating.YakuList;

public class Ippatsu : Yaku
{
    public Ippatsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Ippatsu";
        HanOpen = null; // Only available for closed hands
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code
        return true;
    }
}
