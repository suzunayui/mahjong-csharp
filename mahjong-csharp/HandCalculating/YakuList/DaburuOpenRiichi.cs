using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating.YakuList
{
    /// <summary>
    /// ダブルオープンリーチ (Double Open Riichi)
    /// </summary>
    public class DaburuOpenRiichi : Yaku
    {
        public DaburuOpenRiichi(int? yakuId = null) : base(yakuId) { }

        public override void SetAttributes()
        {
            Name = "Double Open Riichi";
            HanOpen = null; // 門前限定
            HanClosed = 3;
            IsYakuman = false;
        }

        public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
        {
            // ダブルオープンリーチの判定は上位のコードで制御される
            return true;
        }

        public override string ToString()
        {
            return Name ?? "DaburuOpenRiichi";
        }
    }
}
