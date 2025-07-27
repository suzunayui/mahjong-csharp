namespace Mahjong.HandCalculating.YakuList;

public class Dora : Yaku
{
    public Dora(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Dora";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        return true;
    }
}