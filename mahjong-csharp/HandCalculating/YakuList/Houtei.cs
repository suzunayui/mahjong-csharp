namespace Mahjong.HandCalculating.YakuList;

public class Houtei : Yaku
{
    public Houtei(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Houtei Raoyui";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code
        return true;
    }
}
