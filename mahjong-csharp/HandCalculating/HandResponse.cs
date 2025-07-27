namespace Mahjong.HandCalculating;

public class HandResponse
{
    public Dictionary<string, object>? Cost { get; set; }
    public int? Han { get; set; }
    public int? Fu { get; set; }
    public List<Dictionary<string, object>>? FuDetails { get; set; }
    public List<Yaku>? Yaku { get; set; }
    public string? Error { get; set; }
    public bool IsOpenHand { get; set; } = false;

    public HandResponse(
        Dictionary<string, object>? cost = null,
        int? han = null,
        int? fu = null,
        IEnumerable<Yaku>? yaku = null,
        string? error = null,
        IEnumerable<Dictionary<string, object>>? fuDetails = null,
        bool isOpenHand = false)
    {
        Cost = cost;
        Han = han;
        Fu = fu;
        Error = error;
        IsOpenHand = isOpenHand;

        if (fuDetails != null)
        {
            FuDetails = fuDetails.OrderByDescending(x => 
                x.ContainsKey("fu") ? Convert.ToInt32(x["fu"]) : 0).ToList();
        }
        else
        {
            FuDetails = null;
        }

        if (yaku != null)
        {
            Yaku = yaku.OrderBy(x => x.YakuId).ToList();
        }
        else
        {
            Yaku = null;
        }
    }

    public override string ToString()
    {
        if (Error != null)
        {
            return Error;
        }
        else
        {
            return $"{Han} han, {Fu} fu";
        }
    }
}