namespace Mahjong.HandCalculating.YakuList;

public class YakuhaiRound : Yaku
{
    public YakuhaiRound(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Yakuhai (wind of round)";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code (needs round wind info)
        return true;
    }
}
