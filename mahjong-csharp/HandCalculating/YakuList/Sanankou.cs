namespace Mahjong.HandCalculating.YakuList;

public class Sanankou : Yaku
{
    public Sanankou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "San Ankou";
        HanOpen = 2;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Three closed pon sets
        // This is a simplified implementation - full version would check win tile, melds, and tsumo
        if (args.Length < 3) return false;

        var winTile = args[0] is int wt ? wt / 4 : 0;
        var melds = args[1] as IEnumerable<Meld> ?? new List<Meld>();
        var isTsumo = args[2] is bool tsumo && tsumo;

        var openSets = melds.Where(m => m.Opened).Select(m => m.Tiles34).ToList();
        var ponSets = hand.Where(set => Utils.IsPonOrKan(set.ToList())).ToList();

        var closedPonSets = 0;
        foreach (var set in ponSets)
        {
            var setList = set.ToList();
            if (openSets.Any(openSet => openSet.SequenceEqual(setList)))
                continue;

            // If we do ron on shanpon wait, our pon will be considered as open
            if (setList.Contains(winTile) && !isTsumo)
                continue;

            closedPonSets++;
        }

        return closedPonSets >= 3;
    }
}
