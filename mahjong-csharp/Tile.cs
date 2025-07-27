namespace Mahjong;

public class Tile
{
    public int Value { get; set; }
    public bool IsTsumogiri { get; set; }

    public Tile(int value, bool isTsumogiri)
    {
        Value = value;
        IsTsumogiri = isTsumogiri;
    }
}

public static class TilesConverter
{
    public static string ToOneLineString(IEnumerable<int> tiles, bool printAkaDora = false)
    {
        var tilesList = tiles.OrderBy(t => t).ToList();

        var man = tilesList.Where(t => t < 36).ToList();
        var pin = tilesList.Where(t => t >= 36 && t < 72).Select(t => t - 36).ToList();
        var sou = tilesList.Where(t => t >= 72 && t < 108).Select(t => t - 72).ToList();
        var honors = tilesList.Where(t => t >= 108).Select(t => t - 108).ToList();

        string Words(List<int> suits, int redFive, string suffix)
        {
            if (!suits.Any()) return "";
            
            var result = string.Join("", suits.Select(i => 
                i == redFive && printAkaDora ? "0" : ((i / 4) + 1).ToString()));
            return result + suffix;
        }

        var souStr = Words(sou, Constants.FiveRedSou - 72, "s");
        var pinStr = Words(pin, Constants.FiveRedPin - 36, "p");
        var manStr = Words(man, Constants.FiveRedMan, "m");
        var honorsStr = Words(honors, -1 - 108, "z");

        return manStr + pinStr + souStr + honorsStr;
    }

    public static int[] To34Array(IEnumerable<int> tiles)
    {
        var results = new int[34];
        foreach (var tile in tiles)
        {
            results[tile / 4]++;
        }
        return results;
    }

    public static List<int> To136Array(IReadOnlyList<int> tiles)
    {
        var temp = new List<int>();
        var results = new List<int>();
        
        for (int x = 0; x < 34; x++)
        {
            if (tiles[x] > 0)
            {
                var tempValue = Enumerable.Repeat(x * 4, tiles[x]).ToList();
                foreach (var tile in tempValue)
                {
                    if (results.Contains(tile))
                    {
                        var countOfTiles = temp.Count(t => t == tile);
                        var newTile = tile + countOfTiles;
                        results.Add(newTile);
                        temp.Add(tile);
                    }
                    else
                    {
                        results.Add(tile);
                        temp.Add(tile);
                    }
                }
            }
        }
        return results;
    }

    public static List<int> StringTo136Array(
        string? sou = null,
        string? pin = null,
        string? man = null,
        string? honors = null,
        bool hasAkaDora = false)
    {
        List<int> SplitString(string? str, int offset, int? red = null)
        {
            var data = new List<int>();
            var temp = new List<int>();

            if (string.IsNullOrEmpty(str)) return data;

            foreach (var c in str)
            {
                if ((c == 'r' || c == '0') && hasAkaDora)
                {
                    if (red == null) throw new ArgumentException("Red tile not specified");
                    temp.Add(red.Value);
                    data.Add(red.Value);
                }
                else
                {
                    var tile = offset + (int.Parse(c.ToString()) - 1) * 4;
                    if (tile == red && hasAkaDora)
                    {
                        tile++;
                    }
                    if (data.Contains(tile))
                    {
                        var countOfTiles = temp.Count(t => t == tile);
                        var newTile = tile + countOfTiles;
                        data.Add(newTile);
                        temp.Add(tile);
                    }
                    else
                    {
                        data.Add(tile);
                        temp.Add(tile);
                    }
                }
            }
            return data;
        }

        var results = SplitString(man, 0, Constants.FiveRedMan);
        results.AddRange(SplitString(pin, 36, Constants.FiveRedPin));
        results.AddRange(SplitString(sou, 72, Constants.FiveRedSou));
        results.AddRange(SplitString(honors, 108));

        return results;
    }

    public static int[] StringTo34Array(
        string? sou = null,
        string? pin = null,
        string? man = null,
        string? honors = null)
    {
        var results = StringTo136Array(sou, pin, man, honors);
        return To34Array(results);
    }

    public static int? Find34TileIn136Array(int? tile34, IEnumerable<int> tiles)
    {
        if (tile34 == null || tile34 > 33) return null;

        var tile = tile34.Value * 4;
        var possibleTiles = new[] { tile, tile + 1, tile + 2, tile + 3 };

        return possibleTiles.FirstOrDefault(tiles.Contains, -1) == -1 ? null : possibleTiles.FirstOrDefault(tiles.Contains);
    }

    public static List<int> OneLineStringTo136Array(string str, bool hasAkaDora = false)
    {
        var sou = "";
        var pin = "";
        var man = "";
        var honors = "";

        var splitStart = 0;

        for (int index = 0; index < str.Length; index++)
        {
            var c = str[index];
            switch (c)
            {
                case 'm':
                    man += str[splitStart..index];
                    splitStart = index + 1;
                    break;
                case 'p':
                    pin += str[splitStart..index];
                    splitStart = index + 1;
                    break;
                case 's':
                    sou += str[splitStart..index];
                    splitStart = index + 1;
                    break;
                case 'z' or 'h':
                    honors += str[splitStart..index];
                    splitStart = index + 1;
                    break;
            }
        }

        return StringTo136Array(sou, pin, man, honors, hasAkaDora);
    }

    public static int[] OneLineStringTo34Array(string str, bool hasAkaDora = false)
    {
        var results = OneLineStringTo136Array(str, hasAkaDora);
        return To34Array(results);
    }
}