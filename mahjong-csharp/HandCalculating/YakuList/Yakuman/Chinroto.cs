namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Chinroto : Yaku
{
    public Chinroto(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Chinroutou";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Hand composed entirely of terminal tiles
        var allTiles = hand.SelectMany(set => set);
        return allTiles.All(tile => Utils.IsTerminal(tile));
    }
}
