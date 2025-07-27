namespace Mahjong.HandCalculating.YakuList;

public class Hatsu : Yaku
{
    public Hatsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Yakuhai (Hatsu)";
        HanOpen = 1;
        HanClosed = 1;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Check for pon/kan of green dragons (Hatsu)
        return hand.Count(set => 
        {
            var setList = set.ToList();
            return Utils.IsPonOrKan(setList) && setList.Count > 0 && setList[0] == Constants.Hatsu;
        }) >= 1;
    }
}
