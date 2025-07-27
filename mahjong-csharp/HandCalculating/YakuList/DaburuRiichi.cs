using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating.YakuList
{
    /// <summary>
    /// ダブルリーチ (Double Riichi)
    /// </summary>
    public class DaburuRiichi : Yaku
    {
        public DaburuRiichi(int? yakuId = null) : base(yakuId) { }

        public override void SetAttributes()
        {
            TenhouId = 21;
            Name = "Double Riichi";
            HanOpen = null; // 門前限定
            HanClosed = 2;
            IsYakuman = false;
        }

        public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
        {
            // ダブルリーチの判定は上位のコードで制御される
            return true;
        }

        public override string ToString()
        {
            return Name ?? "DaburuRiichi";
        }
    }
}
