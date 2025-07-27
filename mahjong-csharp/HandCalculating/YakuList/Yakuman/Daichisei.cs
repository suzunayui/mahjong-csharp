using System.Linq;

namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Daichisei : Yaku
{
    public Daichisei(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Daichisei";
        HanOpen = null; // Only available for closed hands
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Seven pairs of honor tiles only
        if (hand.Count() != 7)
            return false;

        var allTiles = hand.SelectMany(set => set);
        
        // All tiles must be honor tiles
        if (!allTiles.All(tile => Utils.IsHonor(tile)))
            return false;

        // All sets must be pairs
        return hand.All(set => set.Count() == 2);
    }
}
