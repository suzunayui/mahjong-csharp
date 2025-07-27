namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Tsuisou : Yaku
{
    public Tsuisou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Tsuisou";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Hand composed entirely of honor tiles
        var allTiles = hand.SelectMany(set => set);
        return allTiles.All(tile => Utils.IsHonor(tile));
    }
}
