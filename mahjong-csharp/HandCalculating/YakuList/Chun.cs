namespace Mahjong.HandCalculating.YakuList;

public class Chun : Yaku
{
    public Chun(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Yakuhai (Chun)";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Check for pon/kan of red dragons (Chun)
        return hand.Count(set => 
        {
            var setList = set.ToList();
            return Utils.IsPonOrKan(setList) && setList.Count > 0 && setList[0] == Constants.Chun;
        }) >= 1;
    }
}
