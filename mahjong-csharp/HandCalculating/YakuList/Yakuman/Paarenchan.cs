namespace Mahjong.HandCalculating.YakuList.Yakuman;

public class Paarenchan : Yaku
{
    public int Count { get; set; } = 0;

    public Paarenchan(int? yakuId = null) : base(yakuId) { }

    public override void SetAttributes()
    {
        Name = "Paarenchan";
        HanOpen = 13;
        HanClosed = 13;
        IsYakuman = true;
    }

    public void SetPaarenchanCount(int count)
    {
        Count = count;
        HanOpen = 13 * count;
        HanClosed = 13 * count;
    }

    public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
    {
        // Condition is controlled by superior code
        return true;
    }

    public override string ToString()
    {
        return $"Paarenchan {Count}";
    }
}
