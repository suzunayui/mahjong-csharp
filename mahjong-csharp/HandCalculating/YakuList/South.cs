namespace Mahjong.HandCalculating.YakuList;

public class South : Yaku
{
    public South(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Yakuhai (south)";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        return hand.Any(set => set.Count() >= 3 && 
                              Utils.IsPonOrKan(set.ToList()) && 
                              set.First() == Constants.South);
    }
}
