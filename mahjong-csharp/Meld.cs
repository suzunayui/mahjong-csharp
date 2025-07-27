namespace Mahjong;

public class Meld
{
    public const string Chi = "chi";
    public const string Pon = "pon";
    public const string Kan = "kan";
    public const string Shouminkan = "shouminkan";
    public const string Nuki = "nuki";

    public int? Who { get; set; }
    public List<int> Tiles { get; set; } = new();
    public string? Type { get; set; }
    public int? FromWho { get; set; }
    public int? CalledTile { get; set; }
    public bool Opened { get; set; } = true;

    public Meld(
        string? meldType = null,
        IEnumerable<int>? tiles = null,
        bool opened = true,
        int? calledTile = null,
        int? who = null,
        int? fromWho = null)
    {
        Type = meldType;
        Tiles = tiles?.ToList() ?? new List<int>();
        Opened = opened;
        CalledTile = calledTile;
        Who = who;
        FromWho = fromWho;
    }

    public override string ToString()
    {
        return $"Type: {Type}, Tiles: {TilesConverter.ToOneLineString(Tiles)} {string.Join(", ", Tiles)}";
    }

    public List<int> Tiles34 => Tiles.Select(x => x / 4).ToList();

    [Obsolete("Use Shouminkan instead of Chankan")]
    public string Chankan => Shouminkan;
}