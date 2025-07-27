namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Suukantsu : Yaku
{
    public Suukantsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Suu Kantsu";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        if (args.Length < 1)
            return false;

        var melds = args[0] as IEnumerable<Meld>;
        if (melds == null)
            return false;

        var kanSets = melds.Count(meld => meld.Type == Meld.Kan || meld.Type == Meld.Shouminkan);
        return kanSets == 4;
    }
}
