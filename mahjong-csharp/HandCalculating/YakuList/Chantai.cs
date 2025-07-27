namespace Mahjong.HandCalculating.YakuList;

public class Chantai : Yaku
{
    public Chantai(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Chantai";
        HanOpen = 1;
        HanClosed = 2;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var honorSets = 0;
        var terminalSets = 0;
        var countOfChi = 0;

        foreach (var set in hand)
        {
            var setList = set.ToList();
            
            if (Utils.IsChi(setList))
                countOfChi++;

            if (setList.Any(tile => Utils.IsTerminal(tile)))
                terminalSets++;

            if (setList.Any(tile => Utils.IsHonor(tile)))
                honorSets++;
        }

        // Must have at least one sequence
        if (countOfChi == 0)
            return false;

        // Every set must contain terminal or honor, and must have both terminal and honor sets
        return terminalSets + honorSets == 5 && terminalSets > 0 && honorSets > 0;
    }
}
