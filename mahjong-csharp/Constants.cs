namespace Mahjong;

public static class Constants
{
    // 1 and 9
    public static readonly int[] TerminalIndices = { 0, 8, 9, 17, 18, 26 };

    // dragons and winds
    public const int East = 27;
    public const int South = 28;
    public const int West = 29;
    public const int North = 30;
    public const int Haku = 31;
    public const int Hatsu = 32;
    public const int Chun = 33;

    public static readonly int[] Winds = { East, South, West, North };
    public static readonly int[] HonorIndices = { East, South, West, North, Haku, Hatsu, Chun };

    public const int FiveRedMan = 16;
    public const int FiveRedPin = 52;
    public const int FiveRedSou = 88;

    public static readonly int[] AkaDoraList = { FiveRedMan, FiveRedPin, FiveRedSou };

    public static readonly Dictionary<int, string> DisplayWinds = new()
    {
        { East, "East" },
        { South, "South" },
        { West, "West" },
        { North, "North" }
    };
}