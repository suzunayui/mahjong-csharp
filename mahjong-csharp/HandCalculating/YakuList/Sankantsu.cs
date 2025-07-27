namespace Mahjong.HandCalculating.YakuList;

public class Sankantsu : Yaku
{
    public Sankantsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "San Kantsu";
        HanOpen = 2;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Three kan sets - this would be better handled with meld information
        // For now, check if we have 3 sets of 4 identical tiles
        if (args.Length < 1)
            return false;

        var melds = args[0] as IEnumerable<Meld>;
        if (melds == null)
            return false;

        var kanSets = melds.Count(meld => meld.Type == Meld.Kan || meld.Type == Meld.Shouminkan);
        return kanSets == 3;
    }
}
