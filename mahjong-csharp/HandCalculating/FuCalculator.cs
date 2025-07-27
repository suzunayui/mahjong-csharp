using System;
using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating
{
    /// <summary>
    /// 符計算クラス
    /// </summary>
    public class FuCalculator
    {
        // 符計算の種類定数
        public const string BASE = "base";
        public const string PENCHAN = "penchan";
        public const string KANCHAN = "kanchan";
        public const string VALUED_PAIR = "valued_pair";
        public const string DOUBLE_VALUED_PAIR = "double_valued_pair";
        public const string PAIR_WAIT = "pair_wait";
        public const string TSUMO = "tsumo";
        public const string HAND_WITHOUT_FU = "hand_without_fu";
        public const string CLOSED_PON = "closed_pon";
        public const string OPEN_PON = "open_pon";
        public const string CLOSED_TERMINAL_PON = "closed_terminal_pon";
        public const string OPEN_TERMINAL_PON = "open_terminal_pon";
        public const string CLOSED_KAN = "closed_kan";
        public const string OPEN_KAN = "open_kan";
        public const string CLOSED_TERMINAL_KAN = "closed_terminal_kan";
        public const string OPEN_TERMINAL_KAN = "open_terminal_kan";

        /// <summary>
        /// 符詳細情報
        /// </summary>
        public class FuDetail
        {
            public int Fu { get; set; }
            public string Reason { get; set; } = "";
        }

        /// <summary>
        /// 符を計算する
        /// </summary>
        /// <param name="hand">手牌の面子リスト</param>
        /// <param name="winTile">上がり牌（136形式）</param>
        /// <param name="winGroup">上がり牌が含まれる面子</param>
        /// <param name="config">設定</param>
        /// <param name="valuedTiles">役牌リスト</param>
        /// <param name="melds">明刻・明槓のリスト</param>
        /// <returns>符詳細リストと総符数</returns>
        public (List<FuDetail> Details, int TotalFu) CalculateFu(
            IEnumerable<IEnumerable<int>> hand,
            int winTile,
            IEnumerable<int> winGroup,
            HandConfig config,
            IEnumerable<int>? valuedTiles = null,
            IEnumerable<Meld>? melds = null)
        {
            var handList = hand.ToList();
            var winGroupList = winGroup.ToList();
            var winTile34 = winTile / 4;
            var fuDetails = new List<FuDetail>();

            valuedTiles ??= new List<int>();
            melds ??= new List<Meld>();
            var meldsList = melds.ToList();

            // 七対子の場合
            if (handList.Count == 7)
            {
                return (new List<FuDetail> { new FuDetail { Fu = 25, Reason = BASE } }, 25);
            }

            var isOpenHand = meldsList.Any(m => m.Opened);

            // 雀頭を取得
            var pair = handList.FirstOrDefault(x => IsPair(x));
            
            // 刻子・槓子のリスト
            var ponSets = handList.Where(IsPoNOrKan).ToList();

            // 順子のリスト（鳴いていない順子のみ）
            var openChiMelds = meldsList.Where(m => m.Type == Meld.Chi).Select(m => m.Tiles34).ToList();
            var closedChiSets = handList.Where(x => !IsPair(x) && !IsPoNOrKan(x) && 
                !openChiMelds.Any(open => open.SequenceEqual(x))).ToList();

            // 待ちの符（辺張・嵌張・単騎）
            if (closedChiSets.Any(chi => chi.SequenceEqual(winGroupList)))
            {
                var sortedWinGroup = winGroupList.OrderBy(x => x).ToList();
                var winIndex = sortedWinGroup.IndexOf(winTile34);
                var simplifiedTile = Simplify(winTile34);

                // 辺張（123の3待ち、789の7待ち）
                if (ContainsTerminals(sortedWinGroup))
                {
                    if ((simplifiedTile == 2 && winIndex == 2) || // 123の3待ち
                        (simplifiedTile == 6 && winIndex == 0))   // 789の7待ち
                    {
                        fuDetails.Add(new FuDetail { Fu = 2, Reason = PENCHAN });
                    }
                }

                // 嵌張（135の2待ち）
                if (winIndex == 1)
                {
                    fuDetails.Add(new FuDetail { Fu = 2, Reason = KANCHAN });
                }
            }

            // 役牌雀頭の符
            if (pair != null)
            {
                var pairTile = pair.First();
                var valuedPairCount = valuedTiles.Count(vt => vt == pairTile);
                
                if (valuedPairCount == 1)
                {
                    fuDetails.Add(new FuDetail { Fu = 2, Reason = VALUED_PAIR });
                }
                else if (valuedPairCount == 2)
                {
                    fuDetails.Add(new FuDetail { Fu = 4, Reason = DOUBLE_VALUED_PAIR });
                }
            }

            // 単騎待ち（雀頭待ち）
            if (IsPair(winGroupList))
            {
                fuDetails.Add(new FuDetail { Fu = 2, Reason = PAIR_WAIT });
            }

            // 刻子・槓子の符
            foreach (var ponSet in ponSets)
            {
                var tile = ponSet.First();
                var isTerminalOrHonor = IsTerminalOrHonor(tile);
                var openMelds = meldsList.Where(m => m.Tiles34.SequenceEqual(ponSet)).ToList();
                var openMeld = openMelds.FirstOrDefault();
                
                var setWasOpen = openMeld?.Opened ?? false;
                var isKanSet = openMeld != null && (openMeld.Type == Meld.Kan || openMeld.Type == Meld.Shouminkan);

                // ロンでポンを完成させた場合は、明刻として扱う
                if (!config.IsTsumo && ponSet.SequenceEqual(winGroupList))
                {
                    setWasOpen = true;
                }

                if (isKanSet)
                {
                    if (isTerminalOrHonor)
                    {
                        fuDetails.Add(new FuDetail 
                        { 
                            Fu = setWasOpen ? 16 : 32, 
                            Reason = setWasOpen ? OPEN_TERMINAL_KAN : CLOSED_TERMINAL_KAN 
                        });
                    }
                    else
                    {
                        fuDetails.Add(new FuDetail 
                        { 
                            Fu = setWasOpen ? 8 : 16, 
                            Reason = setWasOpen ? OPEN_KAN : CLOSED_KAN 
                        });
                    }
                }
                else // 刻子
                {
                    if (isTerminalOrHonor)
                    {
                        fuDetails.Add(new FuDetail 
                        { 
                            Fu = setWasOpen ? 4 : 8, 
                            Reason = setWasOpen ? OPEN_TERMINAL_PON : CLOSED_TERMINAL_PON 
                        });
                    }
                    else
                    {
                        fuDetails.Add(new FuDetail 
                        { 
                            Fu = setWasOpen ? 2 : 4, 
                            Reason = setWasOpen ? OPEN_PON : CLOSED_PON 
                        });
                    }
                }
            }

            // ツモの符（他の符がある場合または設定によりピンフツモにも符がある場合）
            var addTsumoFu = fuDetails.Count > 0 || config.Options.FuForPinfuTsumo;
            if (config.IsTsumo && addTsumoFu)
            {
                fuDetails.Add(new FuDetail { Fu = 2, Reason = TSUMO });
            }

            // 開いた手で符がない場合の符（設定により）
            if (isOpenHand && fuDetails.Count == 0 && config.Options.FuForOpenPinfu)
            {
                fuDetails.Add(new FuDetail { Fu = 2, Reason = HAND_WITHOUT_FU });
            }

            // 基本符（開いた手またはツモの場合20符、それ以外30符）
            if (isOpenHand || config.IsTsumo)
            {
                fuDetails.Add(new FuDetail { Fu = 20, Reason = BASE });
            }
            else
            {
                fuDetails.Add(new FuDetail { Fu = 30, Reason = BASE });
            }

            var totalFu = RoundFu(fuDetails);
            return (fuDetails, totalFu);
        }

        private int RoundFu(List<FuDetail> fuDetails)
        {
            var fu = fuDetails.Sum(f => f.Fu);
            return (fu + 9) / 10 * 10; // 10の位に切り上げ
        }

        private bool IsPair(IEnumerable<int> group)
        {
            var list = group.ToList();
            return list.Count == 2 && list[0] == list[1];
        }

        private bool IsPoNOrKan(IEnumerable<int> group)
        {
            var list = group.ToList();
            return list.Count >= 3 && list.All(x => x == list[0]);
        }

        private bool IsTerminalOrHonor(int tile)
        {
            // 么九牌（1,9）と字牌（27-33）
            return Constants.TerminalIndices.Contains(tile) || Constants.HonorIndices.Contains(tile);
        }

        private bool ContainsTerminals(IEnumerable<int> group)
        {
            return group.Any(IsTerminalOrHonor);
        }

        private int Simplify(int tile)
        {
            return tile % 9;
        }
    }
}
