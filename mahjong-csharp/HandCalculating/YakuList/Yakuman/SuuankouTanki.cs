namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class SuuankouTanki : Yaku
{
    public SuuankouTanki(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Suu Ankou Tanki";
        HanOpen = null; // Only available for closed hands
        HanClosed = 26; // Double yakuman
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code (needs special wait pattern info)
        return true;
    }
}
