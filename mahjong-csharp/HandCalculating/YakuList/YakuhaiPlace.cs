namespace Mahjong.HandCalculating.YakuList;

public class YakuhaiPlace : Yaku
{
    public YakuhaiPlace(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Yakuhai (wind of place)";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code (needs player wind info)
        return true;
    }
}
