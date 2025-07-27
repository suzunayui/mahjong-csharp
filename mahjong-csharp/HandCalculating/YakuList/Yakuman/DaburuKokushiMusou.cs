using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating.YakuList.Yakuman
{
    /// <summary>
    /// 国士無双十三面待ち (Double Kokushi Musou)
    /// </summary>
    public class DaburuKokushiMusou : Yaku
    {
        public DaburuKokushiMusou(int? yakuId = null) : base(yakuId) { }

        public override void SetAttributes()
        {
            TenhouId = 48;
            Name = "Kokushi Musou Juusanmen Matchi";
            HanOpen = null; // 門前限定
            HanClosed = 26; // ダブル役満
            IsYakuman = true;
        }

        public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
        {
            // ダブル国士無双の判定は上位のコードで制御される
            return true;
        }

        public override string ToString()
        {
            return Name ?? "DaburuKokushiMusou";
        }
    }
}
