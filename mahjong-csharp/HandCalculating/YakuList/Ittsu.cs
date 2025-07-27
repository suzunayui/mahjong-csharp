namespace Mahjong.HandCalculating.YakuList;

public class Ittsu : Yaku
{
    public Ittsu(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Ittsu";
        HanOpen = 1;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var chiSets = hand.Where(set => Utils.IsChi(set.ToList())).ToList();
        
        if (chiSets.Count < 3)
            return false;

        var souChi = new List<IEnumerable<int>>();
        var pinChi = new List<IEnumerable<int>>();
        var manChi = new List<IEnumerable<int>>();

        foreach (var set in chiSets)
        {
            var firstTile = set.First();
            if (Utils.IsSou(firstTile))
                souChi.Add(set);
            else if (Utils.IsPin(firstTile))
                pinChi.Add(set);
            else if (Utils.IsMan(firstTile))
                manChi.Add(set);
        }

        var suits = new[] { souChi, pinChi, manChi };

        foreach (var suitSets in suits)
        {
            if (suitSets.Count < 3)
                continue;

            var castedSets = new List<int[]>();
            foreach (var set in suitSets)
            {
                var setArray = set.ToArray();
                castedSets.Add(new[] 
                { 
                    Utils.Simplify(setArray[0]), 
                    Utils.Simplify(setArray[1]), 
                    Utils.Simplify(setArray[2]) 
                });
            }

            var has123 = castedSets.Any(set => set.SequenceEqual(new[] { 0, 1, 2 }));
            var has456 = castedSets.Any(set => set.SequenceEqual(new[] { 3, 4, 5 }));
            var has789 = castedSets.Any(set => set.SequenceEqual(new[] { 6, 7, 8 }));

            if (has123 && has456 && has789)
                return true;
        }

        return false;
    }
}
