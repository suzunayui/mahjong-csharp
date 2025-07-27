namespace Mahjong.HandCalculating.YakuList;

public class Tsumo : Yaku
{
    public Tsumo(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Menzen Tsumo";
        HanOpen = null;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        return true;
    }
}