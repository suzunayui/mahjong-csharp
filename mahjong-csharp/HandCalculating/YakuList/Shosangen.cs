namespace Mahjong.HandCalculating.YakuList;

public class Shosangen : Yaku
{
    public Shosangen(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Shou Sangen";
        HanOpen = 2;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var dragons = new[] { Constants.Chun, Constants.Haku, Constants.Hatsu };
        var countOfConditions = 0;

        foreach (var set in hand)
        {
            var setList = set.ToList();
            var firstTile = setList.FirstOrDefault();
            
            // Dragon pon/kan or pair
            if ((Utils.IsPair(setList) || Utils.IsPonOrKan(setList)) && dragons.Contains(firstTile))
            {
                countOfConditions++;
            }
        }

        return countOfConditions == 3;
    }
}
