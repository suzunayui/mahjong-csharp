namespace Mahjong.HandCalculating.YakuList;

public class NagashiMangan : Yaku
{
    public NagashiMangan(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Nagashi Mangan";
        HanOpen = 5;
        HanClosed = 5;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        return true;
    }
}