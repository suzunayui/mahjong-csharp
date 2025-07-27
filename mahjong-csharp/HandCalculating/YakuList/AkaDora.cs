using System.Collections.Generic;
using System.Linq;

namespace Mahjong.HandCalculating.YakuList
{
    /// <summary>
    /// 赤ドラ (Red five)
    /// </summary>
    public class AkaDora : Yaku
    {
        public AkaDora(int? yakuId = null) : base(yakuId) { }

        public override void SetAttributes()
        {
            TenhouId = 54;
            Name = "Aka Dora";
            HanOpen = 1;
            HanClosed = 1;
            IsYakuman = false;
        }

        public override bool IsConditionMet(IEnumerable<IEnumerable<int>> hand, params object?[] args)
        {
            // 赤ドラの検出は別のロジックで処理されるため、
            // ここでは常にtrueを返す
            return true;
        }

        public override string ToString()
        {
            return $"Aka Dora {HanClosed}";
        }
    }
}
