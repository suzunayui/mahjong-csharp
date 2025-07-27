namespace Mahjong.HandCalculating.YakuList;

public class Sanshoku : Yaku
{
    public Sanshoku(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Sanshoku Doujun";
        HanOpen = 1;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var chiSets = hand.Where(set => Utils.IsChi(set.ToList())).ToList();
        if (chiSets.Count < 3) return false;

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

        foreach (var souItem in souChi)
        {
            foreach (var pinItem in pinChi)
            {
                foreach (var manItem in manChi)
                {
                    // Convert to 0-8 representation for comparison
                    var souSimplified = souItem.Select(Utils.Simplify).ToArray();
                    var pinSimplified = pinItem.Select(Utils.Simplify).ToArray();
                    var manSimplified = manItem.Select(Utils.Simplify).ToArray();

                    if (souSimplified.SequenceEqual(pinSimplified) && 
                        pinSimplified.SequenceEqual(manSimplified))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
