namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Ryuisou : Yaku
{
    public Ryuisou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Ryuuiisou";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Green tiles: green dragons (Hatsu) and 2, 3, 4, 6, 8 of sou (19, 20, 21, 23, 25)
        var greenIndices = new[] { 19, 20, 21, 23, 25, Constants.Hatsu };
        var allTiles = hand.SelectMany(set => set);
        
        return allTiles.All(tile => greenIndices.Contains(tile));
    }
}
