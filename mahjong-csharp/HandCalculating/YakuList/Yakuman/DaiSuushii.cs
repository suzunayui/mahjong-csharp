using System.Linq;

namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class DaiSuushii : Yaku
{
    public DaiSuushii(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Dai Suushii";
        HanOpen = 26; // Double yakuman
        HanClosed = 26;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var windSets = 0;

        foreach (var set in hand)
        {
            var firstTile = set.First();
            if (Utils.IsWind(firstTile) && Utils.IsPonOrKan(set.ToList()))
                windSets++;
        }

        return windSets == 4;
    }
}
