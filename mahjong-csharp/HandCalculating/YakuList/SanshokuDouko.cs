namespace Mahjong.HandCalculating.YakuList;

public class SanshokuDouko : Yaku
{
    public SanshokuDouko(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Sanshoku Doukou";
        HanOpen = 2;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var ponSets = hand.Where(set => Utils.IsPonOrKan(set.ToList())).ToList();
        
        if (ponSets.Count < 3)
            return false;

        var souPon = new List<IEnumerable<int>>();
        var pinPon = new List<IEnumerable<int>>();
        var manPon = new List<IEnumerable<int>>();

        foreach (var set in ponSets)
        {
            var firstTile = set.First();
            if (Utils.IsSou(firstTile))
                souPon.Add(set);
            else if (Utils.IsPin(firstTile))
                pinPon.Add(set);
            else if (Utils.IsMan(firstTile))
                manPon.Add(set);
        }

        foreach (var souItem in souPon)
        {
            foreach (var pinItem in pinPon)
            {
                foreach (var manItem in manPon)
                {
                    // Cast tile indices to 0-8 representation
                    var souSimplified = souItem.Select(Utils.Simplify).ToHashSet();
                    var pinSimplified = pinItem.Select(Utils.Simplify).ToHashSet();
                    var manSimplified = manItem.Select(Utils.Simplify).ToHashSet();

                    if (souSimplified.SetEquals(pinSimplified) && pinSimplified.SetEquals(manSimplified))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
