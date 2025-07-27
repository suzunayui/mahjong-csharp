namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class KokushiMusou : Yaku
{
    public KokushiMusou(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Kokushi Musou";
        HanOpen = null;
        HanClosed = 13;
        IsYakuman = true;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        if (hand != null) return false;

        // For kokushi, we need to check 34-tile array
        if (args.Length > 0 && args[0] is int[] tiles34)
        {
            var terminalAndHonorIndices = Constants.TerminalIndices.Concat(Constants.HonorIndices).ToArray();
            var uniqueTiles = 0;
            var hasPair = false;

            foreach (var index in terminalAndHonorIndices)
            {
                if (tiles34[index] >= 1)
                {
                    uniqueTiles++;
                    if (tiles34[index] == 2)
                    {
                        hasPair = true;
                    }
                }
            }

            return uniqueTiles == 13 && hasPair;
        }

        return false;
    }
}