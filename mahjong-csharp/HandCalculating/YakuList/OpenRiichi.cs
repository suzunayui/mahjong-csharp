using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating.YakuList
{
    /// <summary>
    /// オープンリーチ (Open Riichi)
    /// </summary>
    public class OpenRiichi : Yaku
    {
        public OpenRiichi(int? yakuId = null) : base(yakuId) { }

        public override void SetAttributes()
        {
            Name = "Open Riichi";
            HanOpen = null; // 門前限定
            HanClosed = 2;
            IsYakuman = false;
        }

        public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
        {
            // オープンリーチの判定は上位のコードで制御される
            return true;
        }

        public override string ToString()
        {
            return Name ?? "OpenRiichi";
        }
    }
}
