namespace Mahjong.HandCalculating.YakuList;

public class Honroto : Yaku
{
    public Honroto(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Honroutou";
        HanOpen = 2;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // All tiles must be terminals or honors
        var allTiles = hand.SelectMany(set => set);
        return allTiles.All(tile => Utils.IsTerminal(tile) || Utils.IsHonor(tile));
    }
}
