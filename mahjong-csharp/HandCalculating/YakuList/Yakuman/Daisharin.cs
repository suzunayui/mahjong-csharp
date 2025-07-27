using System.Linq;

namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Daisharin : Yaku
{
    public Daisharin(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Daisharin";
        HanOpen = null; // Only available for closed hands
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Seven pairs: 2-2 3-3 4-4 5-5 6-6 7-7 8-8 of pin suit
        if (hand.Count() != 7)
            return false;

        // All sets must be pairs
        if (!hand.All(set => set.Count() == 2))
            return false;

        var allTiles = hand.SelectMany(set => set).ToList();
        
        // All tiles must be pin (sou)
        if (!allTiles.All(tile => Utils.IsSou(tile)))
            return false;

        // Check for exactly 2-2 3-3 4-4 5-5 6-6 7-7 8-8 pattern
        var simplifiedTiles = allTiles.Select(Utils.Simplify).OrderBy(x => x).ToList();
        var expectedPattern = new[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };
        
        return simplifiedTiles.SequenceEqual(expectedPattern);
    }
}
