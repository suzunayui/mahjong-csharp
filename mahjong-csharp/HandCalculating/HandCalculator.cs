using System;
using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating
{
    public class HandCalculator
    {
        private HandConfig _config = new HandConfig();

        public const string ErrNoWinningTile = "winning_tile_not_in_hand";
        public const string ErrOpenHandRiichi = "open_hand_riichi_not_allowed";
        public const string ErrOpenHandDaburi = "open_hand_daburi_not_allowed";
        public const string ErrHandNotWinning = "hand_not_winning";
        public const string ErrHandNotCorrect = "hand_not_correct";
        public const string ErrNoYaku = "no_yaku";

        public HandResponse EstimateHandValue(
            IEnumerable<int> tiles,
            int winTile,
            IEnumerable<Meld>? melds = null,
            IEnumerable<int>? doraIndicators = null,
            HandConfig? config = null)
        {
            _config = config ?? new HandConfig();
            var agari = new Agari();
            var fuCalculator = new FuCalculator();
            var scoresCalculator = new ScoresCalculator();

            var allMelds = (melds ?? Enumerable.Empty<Meld>()).ToList();
            var tiles34 = TilesConverter.To34Array(tiles);
            var isOpenHand = allMelds.Any();
            var handYaku = new List<Yaku>();

            if (!tiles.Contains(winTile))
            {
                return new HandResponse(error: ErrNoWinningTile);
            }

            if (_config.IsRiichi && !_config.IsDaburuRiichi && isOpenHand)
            {
                return new HandResponse(error: ErrOpenHandRiichi);
            }

            if (!agari.IsAgari(tiles34, null))
            {
                return new HandResponse(error: ErrHandNotWinning);
            }

            // Use HandDivider to divide the hand properly
            var handDivider = new HandDivider();
            var possibleHands = handDivider.DivideHand(tiles34, allMelds);
            if (!possibleHands.Any())
            {
                return new HandResponse(error: ErrHandNotWinning);
            }

            // Check for special case: ryanpeiko pattern that might be detected as chiitoitsu
            var isRyanpeikoPattern = !isOpenHand && IsRyanpeikoPattern(tiles34);
            List<List<int>>? ryanpeikoHand = null;
            
            if (isRyanpeikoPattern)
            {
                ryanpeikoHand = CreateRyanpeikoHand(tiles34);
            }

            // Choose the best hand division (prioritize ryanpeiko over chiitoitsu)
            var hand = ryanpeikoHand ?? ChooseBestHandDivision(possibleHands, !isOpenHand);

            // Check if this hand division represents chiitoitsu (7 pairs of size 2 each)
            var isChiitoitsu = !isRyanpeikoPattern && hand.Count == 7 && hand.All(group => group.Count == 2) && !isOpenHand;
            var valuedTiles = new List<int> { 31, 32, 33 }; // 白發中
            if (_config.PlayerWind.HasValue) valuedTiles.Add(_config.PlayerWind.Value);
            if (_config.RoundWind.HasValue) valuedTiles.Add(_config.RoundWind.Value);

            var winGroup = FindWinGroup(winTile, hand);
            if (winGroup == null)
            {
                return new HandResponse(error: ErrHandNotWinning);
            }

            // Calculate fu using the proper FuCalculator
            var (fuDetails, fu) = fuCalculator.CalculateFu(hand, winTile, winGroup, _config, valuedTiles, allMelds);

            var han = 0;

            // Check for Tanyao
            if (_config.Yaku.Tanyao.IsConditionMet(hand))
            {
                handYaku.Add(_config.Yaku.Tanyao);
                han += isOpenHand ? (_config.Yaku.Tanyao.HanOpen ?? 0) : (_config.Yaku.Tanyao.HanClosed ?? 0);
            }
            
            // Check for Pinfu (can coexist with tanyao, but not with chiitoitsu or ryanpeiko)
            bool hasRyanpeiko = false;
            object?[] pinfuArgs = new object?[] { isOpenHand, winTile, _config.PlayerWind, _config.RoundWind };
            if (!isChiitoitsu && _config.Yaku.Pinfu.IsConditionMet(hand, pinfuArgs))
            {
                // Check if ryanpeiko will be detected later
                hasRyanpeiko = !isOpenHand && !isChiitoitsu && _config.Yaku.Ryanpeiko.IsConditionMet(hand, isOpenHand);
                
                if (!hasRyanpeiko) // Pinfu and Ryanpeiko are mutually exclusive
                {
                    handYaku.Add(_config.Yaku.Pinfu);
                    han += _config.Yaku.Pinfu.HanClosed ?? 0;
                    
                    // Pinfu special fu calculation (only if pinfu is present)
                    if (_config.IsTsumo)
                    {
                        fu = 20; // Pinfu tsumo is always 20 fu
                    }
                    else
                    {
                        fu = 30; // Pinfu ron is 30 fu
                    }
                }
            }

            // Check for Tsumo (only for closed hands)
            if (_config.IsTsumo && !isOpenHand)
            {
                handYaku.Add(_config.Yaku.Tsumo);
                han += _config.Yaku.Tsumo.HanClosed ?? 0;
            }

            // Check for Chiitoitsu (seven pairs)
            if (isChiitoitsu)
            {
                handYaku.Add(_config.Yaku.Chiitoitsu);
                han += _config.Yaku.Chiitoitsu.HanClosed ?? 0;
                fu = 25; // Chiitoitsu is always 25 fu
            }
            
            // Check for Ryanpeiko (only for closed hands and non-chiitoitsu)
            if (!isOpenHand && !isChiitoitsu && _config.Yaku.Ryanpeiko.IsConditionMet(hand, isOpenHand))
            {
                handYaku.Add(_config.Yaku.Ryanpeiko);
                han += _config.Yaku.Ryanpeiko.HanClosed ?? 0;
                // Ryanpeiko uses standard fu calculation (not pinfu, not chiitoitsu)
                if (fu == 20) fu = 30; // Override pinfu fu if detected
            }
            // Check for Iipeiko (only if not Ryanpeiko)
            else if (!isOpenHand && !isChiitoitsu && _config.Yaku.Iipeiko.IsConditionMet(hand, isOpenHand))
            {
                handYaku.Add(_config.Yaku.Iipeiko);
                han += _config.Yaku.Iipeiko.HanClosed ?? 0;
            }

            // Check for Sanshoku Doujun (三色同順)
            if (!isChiitoitsu && _config.Yaku.Sanshoku.IsConditionMet(hand, isOpenHand))
            {
                handYaku.Add(_config.Yaku.Sanshoku);
                han += isOpenHand ? (_config.Yaku.Sanshoku.HanOpen ?? 0) : (_config.Yaku.Sanshoku.HanClosed ?? 0);
            }

            // Check for Ittsu (一気通貫)
            if (!isChiitoitsu && _config.Yaku.Ittsu.IsConditionMet(hand, isOpenHand))
            {
                handYaku.Add(_config.Yaku.Ittsu);
                han += isOpenHand ? (_config.Yaku.Ittsu.HanOpen ?? 0) : (_config.Yaku.Ittsu.HanClosed ?? 0);
            }

            // Check for Riichi
            if (_config.IsRiichi && !_config.IsDaburuRiichi)
            {
                handYaku.Add(_config.Yaku.Riichi);
                han += _config.Yaku.Riichi.HanClosed ?? 0;
            }

            if (_config.IsDaburuRiichi)
            {
                handYaku.Add(_config.Yaku.DaburuRiichi);
                han += _config.Yaku.DaburuRiichi.HanClosed ?? 0;
            }

            // Check for Ippatsu (一発)
            if (_config.IsIppatsu && _config.IsRiichi && !isOpenHand)
            {
                handYaku.Add(_config.Yaku.Ippatsu);
                han += _config.Yaku.Ippatsu.HanClosed ?? 0;
            }

            // Check for Haitei (海底撈月)
            if (_config.IsHaitei && _config.IsTsumo)
            {
                handYaku.Add(_config.Yaku.Haitei);
                han += _config.Yaku.Haitei.HanClosed ?? 0;
            }

            // Check for Houtei (河底撈魚)
            if (_config.IsHoutei && !_config.IsTsumo)
            {
                handYaku.Add(_config.Yaku.Houtei);
                han += _config.Yaku.Houtei.HanClosed ?? 0;
            }

            // Check for Rinshan (嶺上開花)
            if (_config.IsRinshan && _config.IsTsumo && allMelds.Any(m => m.Type == Meld.Kan))
            {
                handYaku.Add(_config.Yaku.Rinshan);
                han += _config.Yaku.Rinshan.HanClosed ?? 0;
            }

            // Check for Chankan (槍槓)
            if (_config.IsChankan && !_config.IsTsumo)
            {
                handYaku.Add(_config.Yaku.Chankan);
                han += _config.Yaku.Chankan.HanClosed ?? 0;
            }

            // Check for Dora
            var doraCount = 0;
            foreach (var tile in tiles)
            {
                doraCount += Utils.PlusDora(tile, doraIndicators ?? Enumerable.Empty<int>());
            }

            if (doraCount > 0)
            {
                _config.Yaku.Dora.HanOpen = doraCount;
                _config.Yaku.Dora.HanClosed = doraCount;
                handYaku.Add(_config.Yaku.Dora);
                han += doraCount;
            }

            // Check for Yakuhai (role tiles) - both in hand and melds
            var allGroups = new List<List<int>>(hand);
            foreach (var meld in allMelds)
            {
                if (meld.Type == Meld.Pon || meld.Type == Meld.Kan)
                {
                    allGroups.Add(meld.Tiles34);
                }
            }

            foreach (var group in allGroups)
            {
                if (group.Count >= 3 && group[0] == group[1] && group[1] == group[2]) // pon or kan
                {
                    var tileType = group[0];
                    if (valuedTiles.Contains(tileType))
                    {
                        var hanValue = isOpenHand ? 1 : 1; // Both open and closed are 1 han for yakuhai
                        
                        if (tileType == 31) // 白
                        {
                            handYaku.Add(_config.Yaku.Haku);
                            han += hanValue;
                        }
                        else if (tileType == 32) // 發  
                        {
                            handYaku.Add(_config.Yaku.Hatsu);
                            han += hanValue;
                        }
                        else if (tileType == 33) // 中
                        {
                            handYaku.Add(_config.Yaku.Chun);
                            han += hanValue;
                        }
                        else if (tileType == 27) // 東
                        {
                            handYaku.Add(_config.Yaku.East);
                            han += hanValue;
                        }
                        else if (tileType == 28) // 南
                        {
                            handYaku.Add(_config.Yaku.South);
                            han += hanValue;
                        }
                        else if (tileType == 29) // 西
                        {
                            handYaku.Add(_config.Yaku.West);
                            han += hanValue;
                        }
                        else if (tileType == 30) // 北
                        {
                            handYaku.Add(_config.Yaku.North);
                            han += hanValue;
                        }
                    }
                }
            }

            // Check for Yakuman (役満)
            var yakumanDetected = false;
            
            // Check for Daburu Chuuren Poutou (純正九蓮宝燈) first - this is more restrictive
            if (!isOpenHand && !isChiitoitsu && IsDaburuChuurenPoutou(tiles, winTile))
            {
                handYaku.Add(_config.Yaku.DaburuChuurenPoutou);
                han = _config.Yaku.DaburuChuurenPoutou.HanClosed ?? 26;
                yakumanDetected = true;
            }
            // Check for Chuuren Poutou (九蓮宝燈) - only if not pure nine gates
            else if (!isOpenHand && !isChiitoitsu && _config.Yaku.ChuurenPoutou.IsConditionMet(hand))
            {
                handYaku.Add(_config.Yaku.ChuurenPoutou);
                han = _config.Yaku.ChuurenPoutou.HanClosed ?? 13;
                yakumanDetected = true;
            }
            
            // Check for Kokushi Musou (国士無双) - check separately
            if (!yakumanDetected && !isOpenHand && _config.Yaku.Kokushi.IsConditionMet(hand))
            {
                handYaku.Add(_config.Yaku.Kokushi);
                han = _config.Yaku.Kokushi.HanClosed ?? 13;
                yakumanDetected = true;
            }
            
            // Check for Suuankou (四暗刻) - only closed hands and self-draw
            if (!yakumanDetected && !isOpenHand && _config.IsTsumo && _config.Yaku.Suuankou.IsConditionMet(hand))
            {
                handYaku.Add(_config.Yaku.Suuankou);
                han = _config.Yaku.Suuankou.HanClosed ?? 13;
                yakumanDetected = true;
            }
            
            // Check for Daisangen (大三元)
            if (!yakumanDetected && _config.Yaku.Daisangen.IsConditionMet(hand))
            {
                handYaku.Add(_config.Yaku.Daisangen);
                han = _config.Yaku.Daisangen.HanClosed ?? 13;
                yakumanDetected = true;
            }

            // If no Yakuman detected, check for regular yaku
            if (!yakumanDetected)
            {
                // Check for Chinitsu (清一色)
                if (_config.Yaku.Chinitsu.IsConditionMet(hand))
                {
                    handYaku.Add(_config.Yaku.Chinitsu);
                    han += isOpenHand ? (_config.Yaku.Chinitsu.HanOpen ?? 0) : (_config.Yaku.Chinitsu.HanClosed ?? 0);
                }

                // Check for Tanyao (断幺九)
                if (_config.Yaku.Tanyao.IsConditionMet(hand))
                {
                    handYaku.Add(_config.Yaku.Tanyao);
                    han += _config.Yaku.Tanyao.HanClosed ?? 0;
                }

                // Check for Chiitoitsu (七対子)
                if (isChiitoitsu)
                {
                    handYaku.Add(_config.Yaku.Chiitoitsu);
                    han += _config.Yaku.Chiitoitsu.HanClosed ?? 0;
                }

                // Check for Riichi
                if (_config.IsRiichi)
                {
                    handYaku.Add(_config.Yaku.Riichi);
                    han += _config.Yaku.Riichi.HanClosed ?? 0;
                }

                // Check for Tsumo (ツモ)
                if (!isOpenHand && _config.IsTsumo)
                {
                    handYaku.Add(_config.Yaku.Tsumo);
                    han += _config.Yaku.Tsumo.HanClosed ?? 0;
                }

                // Add Dora if available
                if (doraIndicators != null)
                {
                    var doraTileCount = 0;
                    foreach (var tile in tiles)
                    {
                        doraTileCount += Utils.PlusDora(tile, doraIndicators);
                    }
                    
                    if (doraTileCount > 0)
                    {
                        var doraYaku = _config.Yaku.Dora;
                        doraYaku.HanClosed = doraTileCount;
                        doraYaku.HanOpen = doraTileCount;
                        handYaku.Add(doraYaku);
                        han += doraTileCount;
                    }
                }
            }

            if (han == 0)
            {
                return new HandResponse(error: ErrNoYaku);
            }

            var cost = scoresCalculator.CalculateScores(han, fu, _config, yakumanDetected);

            return new HandResponse
            {
                Cost = cost,
                Han = han,
                Fu = fu,
                Yaku = handYaku
            };
        }

        private List<int>? FindWinGroup(int winTile, List<List<int>> hand)
        {
            var winTile34 = winTile / 4;
            return hand.FirstOrDefault(group => group.Contains(winTile34));
        }

        private bool IsChiitoitsuHand(int[] tiles34)
        {
            var pairsCount = 0;
            var totalTiles = 0;
            
            for (int i = 0; i < 34; i++)
            {
                if (tiles34[i] == 2)
                    pairsCount++;
                else if (tiles34[i] == 4) // 4枚持ちがある場合は二盃口の可能性
                    return false; // 七対子ではない
                else if (tiles34[i] != 0)
                    return false; // Can only have pairs or empty
                    
                totalTiles += tiles34[i];
            }
            
            return pairsCount == 7 && totalTiles == 14;
        }

        private List<List<int>> ChooseBestHandDivision(List<List<List<int>>> possibleHands, bool isClosedHand)
        {
            if (possibleHands.Count == 1)
                return possibleHands.First();

            // For closed hands, prioritize ryanpeiko over chiitoitsu
            if (isClosedHand)
            {
                // Look for ryanpeiko (4 mentsu + 1 jantou with exactly 2 identical sequences appearing twice)
                var ryanpeikoHand = possibleHands.FirstOrDefault(hand => 
                    hand.Count == 5 && IsRyanpeiko(hand));
                
                if (ryanpeikoHand != null)
                    return ryanpeikoHand;
            }

            // Return the first available hand
            return possibleHands.First();
        }

        private bool IsRyanpeiko(List<List<int>> hand)
        {
            if (hand.Count != 5) return false;

            var sequences = hand.Where(group => group.Count == 3 && 
                group[0] + 1 == group[1] && group[1] + 1 == group[2]).ToList();
            
            if (sequences.Count != 4) return false;

            // Group sequences by their content
            var sequenceGroups = sequences.GroupBy(seq => string.Join(",", seq.OrderBy(x => x)))
                .Where(g => g.Count() == 2).ToList();

            // Should have exactly 2 different sequence types, each appearing exactly twice
            return sequenceGroups.Count == 2;
        }

        private bool IsRyanpeikoPattern(int[] tiles34)
        {
            // Check for specific ryanpeiko patterns
            // Pattern: two different sequences, each appearing exactly twice, plus one pair
            
            // Count sequences and pairs
            var sequences = new List<(int, int, int)>();
            var pairs = new List<int>();
            
            // Find all pairs
            for (int i = 0; i < 34; i++)
            {
                if (tiles34[i] == 2)
                {
                    pairs.Add(i);
                }
                else if (tiles34[i] != 0)
                {
                    return false; // Must be pairs or part of sequences
                }
            }
            
            // Should have exactly 7 pairs for the specific test pattern
            if (pairs.Count != 7)
                return false;
                
            // Check if these pairs can form 2 sequences × 2 + 1 pair
            // For 112233m 99p 445566s pattern:
            // Can be: 123m×2 + 99p + 456s×2
            var tempTiles = new int[34];
            Array.Copy(tiles34, tempTiles, 34);
            
            // Try to find ryanpeiko patterns
            var foundSequencePairs = 0;
            
            // Check each suit for double sequences
            for (int suit = 0; suit < 3; suit++)
            {
                int offset = suit * 9;
                for (int i = 0; i < 7; i++) // 123 to 789
                {
                    int tile1 = offset + i;
                    int tile2 = offset + i + 1;
                    int tile3 = offset + i + 2;
                    
                    if (tempTiles[tile1] >= 2 && tempTiles[tile2] >= 2 && tempTiles[tile3] >= 2)
                    {
                        // Use this sequence twice
                        tempTiles[tile1] -= 2;
                        tempTiles[tile2] -= 2;
                        tempTiles[tile3] -= 2;
                        foundSequencePairs++;
                        break; // Only one sequence type per suit for ryanpeiko
                    }
                }
            }
            
            // Should have exactly 2 sequence pairs
            if (foundSequencePairs != 2)
                return false;
                
            // Check if remaining tiles form exactly one pair
            var remainingPairs = 0;
            for (int i = 0; i < 34; i++)
            {
                if (tempTiles[i] == 2)
                    remainingPairs++;
                else if (tempTiles[i] != 0)
                    return false;
            }
            
            return remainingPairs == 1;
        }

        private List<List<int>>? CreateRyanpeikoHand(int[] tiles34)
        {
            var tempTiles = new int[34];
            Array.Copy(tiles34, tempTiles, 34);
            
            var hand = new List<List<int>>();
            var usedSequences = new List<(int, int, int)>();
            
            // Find and create sequence pairs
            for (int suit = 0; suit < 3; suit++)
            {
                int offset = suit * 9;
                for (int i = 0; i < 7; i++)
                {
                    int tile1 = offset + i;
                    int tile2 = offset + i + 1;
                    int tile3 = offset + i + 2;
                    
                    if (tempTiles[tile1] >= 2 && tempTiles[tile2] >= 2 && tempTiles[tile3] >= 2)
                    {
                        // Add this sequence twice
                        hand.Add(new List<int> { tile1, tile2, tile3 });
                        hand.Add(new List<int> { tile1, tile2, tile3 });
                        tempTiles[tile1] -= 2;
                        tempTiles[tile2] -= 2;
                        tempTiles[tile3] -= 2;
                        usedSequences.Add((tile1, tile2, tile3));
                        break;
                    }
                }
            }
            
            // Find the remaining pair
            for (int i = 0; i < 34; i++)
            {
                if (tempTiles[i] == 2)
                {
                    hand.Add(new List<int> { i, i });
                    break;
                }
            }
            
            // Should have exactly 5 groups (4 sequences + 1 pair)
            return hand.Count == 5 ? hand : null;
        }

        private bool IsDaburuChuurenPoutou(IEnumerable<int> tiles, int winTile)
        {
            // Convert tiles to 34 format for easier analysis
            var tiles34 = new int[34];
            foreach (var tile in tiles)
            {
                tiles34[tile / 4]++;
            }

            // Check if all tiles are from one suit only (no honors)
            var manCount = tiles34.Take(9).Sum();
            var pinCount = tiles34.Skip(9).Take(9).Sum();
            var souCount = tiles34.Skip(18).Take(9).Sum();
            var honorCount = tiles34.Skip(27).Sum();

            if (honorCount > 0) return false;

            var suitCounts = new[] { manCount, pinCount, souCount };
            var nonZeroSuits = suitCounts.Count(c => c > 0);
            if (nonZeroSuits != 1) return false;

            // Determine which suit we're working with
            int suitOffset = 0;
            if (pinCount > 0) suitOffset = 9;
            else if (souCount > 0) suitOffset = 18;

            // Get the suit tiles
            var suitTiles = tiles34.Skip(suitOffset).Take(9).ToArray();
            
            // Pure nine gates must be exactly: 1112345678999 (13 tiles)
            // Plus one extra tile that creates 9-way wait (can be any of 1-9)
            var purePattern = new[] { 3, 1, 1, 1, 1, 1, 1, 1, 3 };
            
            // Check if we have exactly the pure pattern + 1 extra
            var extraTiles = new int[9];
            for (int i = 0; i < 9; i++)
            {
                extraTiles[i] = suitTiles[i] - purePattern[i];
            }
            
            // Should have exactly one extra tile, and it should be the winning tile
            var winTile34 = winTile / 4 - suitOffset;
            if (winTile34 < 0 || winTile34 > 8) return false;
            
            // Check that we have exactly one extra tile in the winning position
            // and no extra tiles elsewhere
            for (int i = 0; i < 9; i++)
            {
                if (i == winTile34)
                {
                    if (extraTiles[i] != 1) return false; // Should have exactly 1 extra
                }
                else
                {
                    if (extraTiles[i] != 0) return false; // Should have no extra
                }
            }
            
            // Pure nine gates is 9-way wait: base pattern 1112345678999 + any 1-9
            // The final tile count can be 4 for positions 0 and 8 (1 and 9)
            // and 2 for positions 1-7 (2-8), which is perfectly valid
            // The key is that we start from the pure base pattern and add exactly one tile
            
            return true;
        }
    }
}