namespace Mahjong.HandCalculating.YakuList;

public class Tanyao : Yaku
{
    public Tanyao(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Tanyao";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        foreach (var meld in hand)
        {
            foreach (var tile in meld)
            {
                if (Utils.IsTerminal(tile) || Utils.IsHonor(tile))
                {
                    return false;
                }
            }
        }
        return true;
    }
}