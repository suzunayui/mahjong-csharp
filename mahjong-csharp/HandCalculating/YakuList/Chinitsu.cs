namespace Mahjong.HandCalculating.YakuList;

public class Chinitsu : Yaku
{
    public Chinitsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Chinitsu";
        HanOpen = 5;
        HanClosed = 6;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var honorSets = 0;
        var souSets = 0;
        var pinSets = 0;
        var manSets = 0;

        foreach (var set in hand)
        {
            var firstTile = set.First();
            
            if (Utils.IsHonor(firstTile))
                honorSets++;
            else if (Utils.IsSou(firstTile))
                souSets++;
            else if (Utils.IsPin(firstTile))
                pinSets++;
            else if (Utils.IsMan(firstTile))
                manSets++;
        }

        var suits = new[] { souSets, pinSets, manSets };
        var nonZeroSuits = suits.Count(x => x != 0);

        // Must have exactly one suit and no honor tiles
        return nonZeroSuits == 1 && honorSets == 0;
    }
}
