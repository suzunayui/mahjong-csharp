namespace Mahjong.HandCalculating.YakuList;

public class Junchan : Yaku
{
    public Junchan(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Junchan";
        HanOpen = 2;
        HanClosed = 3;
        IsYakuman = false;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        var terminalSets = 0;
        var countOfChi = 0;

        foreach (var set in hand)
        {
            var setList = set.ToList();
            
            if (Utils.IsChi(setList))
                countOfChi++;

            if (setList.Any(tile => Utils.IsTerminal(tile)))
                terminalSets++;
        }

        // Must have at least one sequence
        if (countOfChi == 0)
            return false;

        // Every set must contain terminals (no honors allowed)
        return terminalSets == 5;
    }
}
