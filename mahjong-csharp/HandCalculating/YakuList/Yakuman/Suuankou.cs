namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Suuankou : Yaku
{
    public Suuankou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Suu Ankou";
        HanOpen = null; // Only available for closed hands
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        if (args.Length < 2) return false;

        var winTile = args[0] is int wt ? wt / 4 : 0;
        var isTsumo = args[1] is bool tsumo && tsumo;

        var closedHand = new List<IEnumerable<int>>();

        foreach (var set in hand)
        {
            var setList = set.ToList();
            // If we do ron on shanpon wait, our pon will be considered as open
            if (Utils.IsPonOrKan(setList) && setList.Contains(winTile) && !isTsumo)
                continue;

            closedHand.Add(set);
        }

        var countOfPon = closedHand.Count(set => Utils.IsPonOrKan(set.ToList()));
        return countOfPon == 4;
    }
}
