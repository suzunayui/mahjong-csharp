namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Daisangen : Yaku
{
    public Daisangen(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Daisangen";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var dragons = new[] { Constants.Chun, Constants.Haku, Constants.Hatsu };
        var countOfDragonPonSets = 0;

        foreach (var set in hand)
        {
            var setList = set.ToList();
            if (Utils.IsPonOrKan(setList) && dragons.Contains(setList.FirstOrDefault()))
            {
                countOfDragonPonSets++;
            }
        }

        return countOfDragonPonSets == 3;
    }
}
