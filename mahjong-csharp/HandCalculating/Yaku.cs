namespace Mahjong.HandCalculating;

public abstract class Yaku
{
    public int? YakuId { get; set; }
    public int? TenhouId { get; set; }
    public string? Name { get; set; }
    public int? HanOpen { get; set; }
    public int? HanClosed { get; set; }
    public bool IsYakuman { get; set; } = false;

    public Yaku(int? yakuId = null)
    {
        TenhouId = null;
        YakuId = yakuId;
        SetAttributes();
    }

    public override string ToString()
    {
        return Name ?? "";
    }

    public abstract bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args);

    public abstract void SetAttributes();
}