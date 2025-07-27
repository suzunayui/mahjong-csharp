using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mahjong.HandCalculating;

public class HandDivider
{
    private readonly Dictionary<string, List<List<List<int>>>> _cache = new();
    private string? _cacheKey;

    public List<List<List<int>>> DivideHand(int[] tiles34, IEnumerable<Meld>? melds = null, bool useCache = false)
    {
        melds ??= new List<Meld>();
        var meldsList = melds.ToList();

        if (useCache)
        {
            _cacheKey = BuildDividerCacheKey(tiles34, meldsList);
            if (_cache.ContainsKey(_cacheKey))
            {
                return _cache[_cacheKey];
            }
        }

        var closedHandTiles34 = tiles34.ToArray();

        // small optimization, we can't have a pair in open part of the hand,
        // so we don't need to try find pairs in open sets
        var openTileIndices = meldsList.SelectMany(x => x.Tiles34).ToList();
        foreach (var openItem in openTileIndices)
        {
            closedHandTiles34[openItem]--;
        }

        var pairIndices = FindPairs(closedHandTiles34);

        // let's try to find all possible hand options
        var hands = new List<List<List<int>>>();
        foreach (var pairIndex in pairIndices)
        {
            var localTiles34 = tiles34.ToArray();

            // we don't need to combine already open sets
            foreach (var openItem in openTileIndices)
            {
                localTiles34[openItem]--;
            }

            localTiles34[pairIndex] -= 2;

            // 0 - 8 man tiles
            var man = FindValidCombinations(localTiles34, 0, 8);

            // 9 - 17 pin tiles
            var pin = FindValidCombinations(localTiles34, 9, 17);

            // 18 - 26 sou tiles
            var sou = FindValidCombinations(localTiles34, 18, 26);

            var honor = new List<List<int>>();
            foreach (int x in Constants.HonorIndices)
            {
                if (localTiles34[x] == 3)
                {
                    honor.Add(new List<int> { x, x, x });
                }
            }

            var honorArray = honor.Any() ? new List<List<List<int>>> { honor } : null;

            var arrays = new List<List<List<List<int>>>>(); 
            arrays.Add(new List<List<List<int>>> { new List<List<int>> { new List<int> { pairIndex, pairIndex } } });
            if (sou.Any()) arrays.Add(sou);
            if (man.Any()) arrays.Add(man);
            if (pin.Any()) arrays.Add(pin);
            if (honorArray != null) arrays.Add(honorArray);

            foreach (var meld in meldsList)
            {
                arrays.Add(new List<List<List<int>>> { new List<List<int>> { meld.Tiles34.ToList() } });
            }

            // let's find all possible hand from our valid sets
            foreach (var s in GetCartesianProduct(arrays))
            {
                var hand = new List<List<int>>();
                foreach (var item in s)
                {
                    if (item is List<List<int>> listOfLists)
                    {
                        foreach (var x in listOfLists)
                        {
                            hand.Add(x);
                        }
                    }
                    else if (item is List<int> singleList)
                    {
                        hand.Add(singleList);
                    }
                }

                hand.Sort((a, b) => a[0].CompareTo(b[0]));
                if (hand.Count == 5)
                {
                    hands.Add(hand);
                }
            }
        }

        // small optimization, let's remove hand duplicates
        var uniqueHands = new List<List<List<int>>>();
        foreach (var hand in hands)
        {
            var sortedHand = hand.OrderBy(x => x[0]).ThenBy(x => x.Count > 1 ? x[1] : 0).ToList();
            var handExists = uniqueHands.Any(existingHand => 
            {
                if (existingHand.Count != sortedHand.Count) return false;
                for (int i = 0; i < existingHand.Count; i++)
                {
                    if (!existingHand[i].SequenceEqual(sortedHand[i])) return false;
                }
                return true;
            });
            
            if (!handExists)
            {
                uniqueHands.Add(sortedHand);
            }
        }

        hands = uniqueHands;

        if (pairIndices.Count == 7)
        {
            var hand = new List<List<int>>();
            foreach (var index in pairIndices)
            {
                hand.Add(new List<int> { index, index });
            }
            hands.Add(hand);
        }

        var result = hands.OrderBy(h => string.Join(",", h.Select(g => string.Join("", g)))).ToList();

        if (useCache && _cacheKey != null)
        {
            _cache[_cacheKey] = result;
        }

        return result;
    }

    public List<int> FindPairs(int[] tiles34, int firstIndex = 0, int secondIndex = 33)
    {
        var pairIndices = new List<int>();
        for (int x = firstIndex; x <= secondIndex; x++)
        {
            // ignore pon of honor tiles, because it can't be a part of pair
            if (Constants.HonorIndices.Contains(x) && tiles34[x] != 2)
            {
                continue;
            }

            if (tiles34[x] >= 2)
            {
                pairIndices.Add(x);
            }
        }
        return pairIndices;
    }

    public List<List<List<int>>> FindValidCombinations(int[] tiles34, int firstIndex, int secondIndex, bool handNotCompleted = false)
    {
        var indices = new List<int>();
        for (int x = firstIndex; x <= secondIndex; x++)
        {
            if (tiles34[x] > 0)
            {
                for (int i = 0; i < tiles34[x]; i++)
                {
                    indices.Add(x);
                }
            }
        }

        if (!indices.Any())
            return new List<List<List<int>>>();

        var allPossibleCombinations = GetPermutations(indices, 3).ToList();

        bool IsValidCombination(List<int> possibleSet)
        {
            return Utils.IsChi(possibleSet) || Utils.IsPon(possibleSet);
        }

        var validCombinations = new List<List<int>>();
        foreach (var combination in allPossibleCombinations)
        {
            if (IsValidCombination(combination))
            {
                validCombinations.Add(combination);
            }
        }

        if (!validCombinations.Any())
            return new List<List<List<int>>>();

        int countOfNeededCombinations = indices.Count / 3;

        // simple case, we have count of sets == count of tiles
        if (countOfNeededCombinations == validCombinations.Count &&
            validCombinations.SelectMany(x => x).OrderBy(x => x).SequenceEqual(indices.OrderBy(x => x)))
        {
            return new List<List<List<int>>> { validCombinations };
        }

        // filter and remove not possible pon sets
        for (int i = validCombinations.Count - 1; i >= 0; i--)
        {
            var item = validCombinations[i];
            if (Utils.IsPon(item))
            {
                int countOfSets = 1;
                double countOfTiles = 0;
                while (countOfSets > countOfTiles)
                {
                    countOfTiles = indices.Count(x => x == item[0]) / 3.0;
                    countOfSets = validCombinations.Count(x => x[0] == item[0] && x[1] == item[1] && x[2] == item[2]);

                    if (countOfSets > countOfTiles)
                    {
                        validCombinations.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // filter and remove not possible chi sets
        for (int i = validCombinations.Count - 1; i >= 0; i--)
        {
            var item = validCombinations[i];
            if (Utils.IsChi(item))
            {
                int countOfSets = 5;
                // TODO calculate real count of possible sets
                int countOfPossibleSets = 4;
                while (countOfSets > countOfPossibleSets)
                {
                    countOfSets = validCombinations.Count(x => x[0] == item[0] && x[1] == item[1] && x[2] == item[2]);

                    if (countOfSets > countOfPossibleSets)
                    {
                        validCombinations.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // list of chi\pon sets for not completed hand
        if (handNotCompleted)
        {
            return new List<List<List<int>>> { validCombinations };
        }

        // hard case - we can build a lot of sets from our tiles
        // for example we have 123456 tiles and we can build sets:
        // [1, 2, 3] [4, 5, 6] [2, 3, 4] [3, 4, 5]
        // and only two of them valid in the same time [1, 2, 3] [4, 5, 6]

        var possibleCombinations = GetPermutations(Enumerable.Range(0, validCombinations.Count), countOfNeededCombinations);

        var combinationsResults = new List<List<List<int>>>();
        foreach (var combination in possibleCombinations)
        {
            var result = new List<int>();
            foreach (var item in combination)
            {
                result.AddRange(validCombinations[item]);
            }
            result.Sort();
            indices.Sort();

            if (result.SequenceEqual(indices))
            {
                var results = new List<List<int>>();
                foreach (var item in combination)
                {
                    results.Add(validCombinations[item]);
                }
                results.Sort((a, b) => a[0].CompareTo(b[0]));
                if (!combinationsResults.Any(r => SetsEqual(r, results)))
                {
                    combinationsResults.Add(results);
                }
            }
        }

        return combinationsResults;
    }

    private IEnumerable<List<T>> GetPermutations<T>(IEnumerable<T> items, int length)
    {
        var itemsList = items.ToList();
        if (length == 1) 
        {
            return itemsList.Select(t => new List<T> { t });
        }
        
        var result = new List<List<T>>();
        for (int i = 0; i < itemsList.Count; i++)
        {
            var remaining = new List<T>(itemsList);
            remaining.RemoveAt(i);
            
            foreach (var perm in GetPermutations(remaining, length - 1))
            {
                perm.Insert(0, itemsList[i]);
                result.Add(new List<T>(perm));
            }
        }
        
        return result;
    }

    private IEnumerable<IEnumerable<object>> GetCartesianProduct(List<List<List<List<int>>>> arrays)
    {
        IEnumerable<IEnumerable<object>> result = new[] { Enumerable.Empty<object>() };
        
        foreach (var array in arrays)
        {
            result = result.SelectMany(r => array, 
                (r, item) => r.Concat(new object[] { item }));
        }
        
        return result;
    }
    
    private class ListEqualityComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T>? x, List<T>? y)
        {
            if (x == null || y == null) return x == y;
            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            return obj.Aggregate(0, (acc, item) => acc ^ (item?.GetHashCode() ?? 0));
        }
    }

    private bool HandsEqual(List<List<int>> hand1, List<List<int>> hand2)
    {
        if (hand1.Count != hand2.Count)
            return false;

        for (int i = 0; i < hand1.Count; i++)
        {
            if (!hand1[i].SequenceEqual(hand2[i]))
                return false;
        }
        return true;
    }

    private bool SetsEqual(List<List<int>> sets1, List<List<int>> sets2)
    {
        if (sets1.Count != sets2.Count)
            return false;

        for (int i = 0; i < sets1.Count; i++)
        {
            if (!sets1[i].SequenceEqual(sets2[i]))
                return false;
        }
        return true;
    }

    public void ClearCache()
    {
        _cache.Clear();
        _cacheKey = null;
    }

    private string BuildDividerCacheKey(int[] tiles34, List<Meld> melds)
    {
        var preparedArray = tiles34.ToList();
        if (melds.Any())
        {
            preparedArray.AddRange(melds.SelectMany(x => x.Tiles34));
        }
        
        var bytes = Encoding.UTF8.GetBytes(string.Join(",", preparedArray));
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
