using System.Diagnostics;

namespace Mahjong;

public class Shanten
{
    public const int AgariState = -1;

    private int[] _tiles = new int[34];
    private int _numberMelds = 0;
    private int _numberTatsu = 0;
    private int _numberPairs = 0;
    private int _numberJidahai = 0;
    private int _numberCharacters = 0;
    private int _numberIsolatedTiles = 0;
    private int _minShanten = 0;

    public int CalculateShanten(IReadOnlyList<int> tiles34, bool useChiitoitsu = true, bool useKokushi = true)
    {
        var shantenResults = new List<int> { CalculateShantenForRegularHand(tiles34) };
        
        if (useChiitoitsu)
            shantenResults.Add(CalculateShantenForChiitoitsuHand(tiles34));
        
        if (useKokushi)
            shantenResults.Add(CalculateShantenForKokushiHand(tiles34));

        return shantenResults.Min();
    }

    public int CalculateShantenForChiitoitsuHand(IReadOnlyList<int> tiles34)
    {
        var pairs = tiles34.Count(x => x >= 2);
        if (pairs == 7)
            return AgariState;

        var kinds = tiles34.Count(x => x >= 1);
        return 6 - pairs + (kinds < 7 ? 7 - kinds : 0);
    }

    public int CalculateShantenForKokushiHand(IReadOnlyList<int> tiles34)
    {
        var indices = Constants.TerminalIndices.Concat(Constants.HonorIndices).ToArray();

        var completedTerminals = 0;
        var terminals = 0;
        
        foreach (var i in indices)
        {
            if (tiles34[i] >= 2) completedTerminals++;
            if (tiles34[i] != 0) terminals++;
        }

        return 13 - terminals - (completedTerminals > 0 ? 1 : 0);
    }

    public int CalculateShantenForRegularHand(IReadOnlyList<int> tiles34)
    {
        var tilesCopy = tiles34.ToArray();
        Init(tilesCopy);

        var countOfTiles = tilesCopy.Sum();
        Debug.Assert(countOfTiles <= 14, $"Too many tiles = {countOfTiles}");

        RemoveCharacterTiles(countOfTiles);

        var initMentsu = (14 - countOfTiles) / 3;
        Scan(initMentsu);

        return _minShanten;
    }

    private void Init(int[] tiles)
    {
        _tiles = tiles;
        _numberMelds = 0;
        _numberTatsu = 0;
        _numberPairs = 0;
        _numberJidahai = 0;
        _numberCharacters = 0;
        _numberIsolatedTiles = 0;
        _minShanten = 8;
    }

    private void Scan(int initMentsu)
    {
        for (int i = 0; i < 27; i++)
        {
            _numberCharacters |= (_tiles[i] == 4 ? 1 : 0) << i;
        }
        _numberMelds += initMentsu;
        Run(0);
    }

    private void Run(int depth)
    {
        if (_minShanten == AgariState)
            return;

        while (depth < 27 && _tiles[depth] == 0)
        {
            depth++;
        }

        if (depth >= 27)
        {
            UpdateResult();
            return;
        }

        var i = depth;
        if (i > 8) i -= 9;
        if (i > 8) i -= 9;

        if (_tiles[depth] == 4)
        {
            IncreaseSet(depth);
            if (i < 7 && _tiles[depth + 2] > 0)
            {
                if (_tiles[depth + 1] > 0)
                {
                    IncreaseSyuntsu(depth);
                    Run(depth + 1);
                    DecreaseSyuntsu(depth);
                }
                IncreaseTatsuSecond(depth);
                Run(depth + 1);
                DecreaseTatsuSecond(depth);
            }

            if (i < 8 && _tiles[depth + 1] > 0)
            {
                IncreaseTatsuFirst(depth);
                Run(depth + 1);
                DecreaseTatsuFirst(depth);
            }

            IncreaseIsolatedTile(depth);
            Run(depth + 1);
            DecreaseIsolatedTile(depth);
            DecreaseSet(depth);
            IncreasePair(depth);

            if (i < 7 && _tiles[depth + 2] > 0)
            {
                if (_tiles[depth + 1] > 0)
                {
                    IncreaseSyuntsu(depth);
                    Run(depth);
                    DecreaseSyuntsu(depth);
                }
                IncreaseTatsuSecond(depth);
                Run(depth + 1);
                DecreaseTatsuSecond(depth);
            }

            if (i < 8 && _tiles[depth + 1] > 0)
            {
                IncreaseTatsuFirst(depth);
                Run(depth + 1);
                DecreaseTatsuFirst(depth);
            }

            DecreasePair(depth);
        }

        if (_tiles[depth] == 3)
        {
            IncreaseSet(depth);
            Run(depth + 1);
            DecreaseSet(depth);
            IncreasePair(depth);

            if (i < 7 && _tiles[depth + 1] > 0 && _tiles[depth + 2] > 0)
            {
                IncreaseSyuntsu(depth);
                Run(depth + 1);
                DecreaseSyuntsu(depth);
            }
            else
            {
                if (i < 7 && _tiles[depth + 2] > 0)
                {
                    IncreaseTatsuSecond(depth);
                    Run(depth + 1);
                    DecreaseTatsuSecond(depth);
                }

                if (i < 8 && _tiles[depth + 1] > 0)
                {
                    IncreaseTatsuFirst(depth);
                    Run(depth + 1);
                    DecreaseTatsuFirst(depth);
                }
            }

            DecreasePair(depth);

            if (i < 7 && _tiles[depth + 2] >= 2 && _tiles[depth + 1] >= 2)
            {
                IncreaseSyuntsu(depth);
                IncreaseSyuntsu(depth);
                Run(depth);
                DecreaseSyuntsu(depth);
                DecreaseSyuntsu(depth);
            }
        }

        if (_tiles[depth] == 2)
        {
            IncreasePair(depth);
            Run(depth + 1);
            DecreasePair(depth);
            if (i < 7 && _tiles[depth + 2] > 0 && _tiles[depth + 1] > 0)
            {
                IncreaseSyuntsu(depth);
                Run(depth);
                DecreaseSyuntsu(depth);
            }
        }

        if (_tiles[depth] == 1)
        {
            if (i < 6 && _tiles[depth + 1] == 1 && _tiles[depth + 2] > 0 && _tiles[depth + 3] != 4)
            {
                IncreaseSyuntsu(depth);
                Run(depth + 2);
                DecreaseSyuntsu(depth);
            }
            else
            {
                IncreaseIsolatedTile(depth);
                Run(depth + 1);
                DecreaseIsolatedTile(depth);

                if (i < 7 && _tiles[depth + 2] > 0)
                {
                    if (_tiles[depth + 1] > 0)
                    {
                        IncreaseSyuntsu(depth);
                        Run(depth + 1);
                        DecreaseSyuntsu(depth);
                    }
                    IncreaseTatsuSecond(depth);
                    Run(depth + 1);
                    DecreaseTatsuSecond(depth);
                }

                if (i < 8 && _tiles[depth + 1] > 0)
                {
                    IncreaseTatsuFirst(depth);
                    Run(depth + 1);
                    DecreaseTatsuFirst(depth);
                }
            }
        }
    }

    private void UpdateResult()
    {
        var retShanten = 8 - _numberMelds * 2 - _numberTatsu - _numberPairs;
        var nMentsuKouho = _numberMelds + _numberTatsu;
        
        if (_numberPairs > 0)
        {
            nMentsuKouho += _numberPairs - 1;
        }
        else if (_numberCharacters > 0 && _numberIsolatedTiles > 0)
        {
            if ((_numberCharacters | _numberIsolatedTiles) == _numberCharacters)
            {
                retShanten += 1;
            }
        }

        if (nMentsuKouho > 4)
        {
            retShanten += nMentsuKouho - 4;
        }

        if (retShanten != AgariState && retShanten < _numberJidahai)
        {
            retShanten = _numberJidahai;
        }

        if (retShanten < _minShanten)
        {
            _minShanten = retShanten;
        }
    }

    private void IncreaseSet(int k)
    {
        _tiles[k] -= 3;
        _numberMelds += 1;
    }

    private void DecreaseSet(int k)
    {
        _tiles[k] += 3;
        _numberMelds -= 1;
    }

    private void IncreasePair(int k)
    {
        _tiles[k] -= 2;
        _numberPairs += 1;
    }

    private void DecreasePair(int k)
    {
        _tiles[k] += 2;
        _numberPairs -= 1;
    }

    private void IncreaseSyuntsu(int k)
    {
        _tiles[k] -= 1;
        _tiles[k + 1] -= 1;
        _tiles[k + 2] -= 1;
        _numberMelds += 1;
    }

    private void DecreaseSyuntsu(int k)
    {
        _tiles[k] += 1;
        _tiles[k + 1] += 1;
        _tiles[k + 2] += 1;
        _numberMelds -= 1;
    }

    private void IncreaseTatsuFirst(int k)
    {
        _tiles[k] -= 1;
        _tiles[k + 1] -= 1;
        _numberTatsu += 1;
    }

    private void DecreaseTatsuFirst(int k)
    {
        _tiles[k] += 1;
        _tiles[k + 1] += 1;
        _numberTatsu -= 1;
    }

    private void IncreaseTatsuSecond(int k)
    {
        _tiles[k] -= 1;
        _tiles[k + 2] -= 1;
        _numberTatsu += 1;
    }

    private void DecreaseTatsuSecond(int k)
    {
        _tiles[k] += 1;
        _tiles[k + 2] += 1;
        _numberTatsu -= 1;
    }

    private void IncreaseIsolatedTile(int k)
    {
        _tiles[k] -= 1;
        _numberIsolatedTiles |= 1 << k;
    }

    private void DecreaseIsolatedTile(int k)
    {
        _tiles[k] += 1;
        _numberIsolatedTiles &= ~(1 << k);
    }

    private void RemoveCharacterTiles(int nc)
    {
        var number = 0;
        var isolated = 0;

        for (int i = 27; i < 34; i++)
        {
            if (_tiles[i] == 4)
            {
                _numberMelds += 1;
                _numberJidahai += 1;
                number |= 1 << (i - 27);
                isolated |= 1 << (i - 27);
            }

            if (_tiles[i] == 3)
            {
                _numberMelds += 1;
            }

            if (_tiles[i] == 2)
            {
                _numberPairs += 1;
            }

            if (_tiles[i] == 1)
            {
                isolated |= 1 << (i - 27);
            }
        }

        if (_numberJidahai > 0 && (nc % 3) == 2)
        {
            _numberJidahai -= 1;
        }

        if (isolated > 0)
        {
            _numberIsolatedTiles |= 1 << 27;
            if ((number | isolated) == number)
            {
                _numberCharacters |= 1 << 27;
            }
        }
    }
}