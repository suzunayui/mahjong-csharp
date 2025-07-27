namespace Mahjong.HandCalculating.YakuList;

public class Chankan : Yaku
{
    public Chankan(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Chankan";
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
