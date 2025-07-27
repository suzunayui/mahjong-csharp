using System.Linq;

namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Shousuushii : Yaku
{
    public Shousuushii(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Shousuushii";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var windSets = 0;
        var windPair = false;

        foreach (var set in hand)
        {
            var firstTile = set.First();
            if (Utils.IsWind(firstTile))
            {
                if (Utils.IsPonOrKan(set.ToList()))
                    windSets++;
                else if (set.Count() == 2)
                    windPair = true;
            }
        }

        return windSets == 3 && windPair;
    }
}
