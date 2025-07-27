namespace Mahjong.HandCalculating;

public static class HandConstants
{
    // Hands over 26+ han don't count as double yakuman
    public const int KazoeLimit = 0;
    // Hands over 13+ is a sanbaiman
    public const int KazoeSanbaiman = 1;
    // 26+ han as double yakuman, 39+ han as triple yakuman, etc.
    public const int KazoeNoLimit = 2;
}

public class OptionalRules
{
    public bool HasOpenTanyao { get; set; } = false;
    public bool HasAkaDora { get; set; } = false;
    public bool HasDoubleYakuman { get; set; } = true;
    public int KazoeLimit { get; set; } = HandConstants.KazoeLimit;
    public bool Kiriage { get; set; } = false;
    public bool FuForOpenPinfu { get; set; } = true;
    public bool FuForPinfuTsumo { get; set; } = false;
    public bool RenhouAsYakuman { get; set; } = false;
    public bool HasDaisharin { get; set; } = false;
    public bool HasDaisharinOtherSuits { get; set; } = false;
    public bool HasDaichisei { get; set; } = false;
    public bool HasSashikomiYakuman { get; set; } = false;
    public bool LimitToSextupleYakuman { get; set; } = true;
    public bool PaarenchanNeedsYaku { get; set; } = true;

    public OptionalRules(
        bool hasOpenTanyao = false,
        bool hasAkaDora = false,
        bool hasDoubleYakuman = true,
        int kazoeLimit = HandConstants.KazoeLimit,
        bool kiriage = false,
        bool fuForOpenPinfu = true,
        bool fuForPinfuTsumo = false,
        bool renhouAsYakuman = false,
        bool hasDaisharin = false,
        bool hasDaisharinOtherSuits = false,
        bool hasSashikomiYakuman = false,
        bool limitToSextupleYakuman = true,
        bool paarenchanNeedsYaku = true,
        bool hasDaichisei = false)
    {
        HasOpenTanyao = hasOpenTanyao;
        HasAkaDora = hasAkaDora;
        HasDoubleYakuman = hasDoubleYakuman;
        KazoeLimit = kazoeLimit;
        Kiriage = kiriage;
        FuForOpenPinfu = fuForOpenPinfu;
        FuForPinfuTsumo = fuForPinfuTsumo;
        RenhouAsYakuman = renhouAsYakuman;
        HasDaisharin = hasDaisharin || hasDaisharinOtherSuits;
        HasDaisharinOtherSuits = hasDaisharinOtherSuits;
        HasSashikomiYakuman = hasSashikomiYakuman;
        LimitToSextupleYakuman = limitToSextupleYakuman;
        HasDaichisei = hasDaichisei;
        PaarenchanNeedsYaku = paarenchanNeedsYaku;
    }
}

public class HandConfig
{
    public YakuConfig Yaku { get; set; }
    public OptionalRules Options { get; set; }

    public bool IsTsumo { get; set; } = false;
    public bool IsRiichi { get; set; } = false;
    public bool IsIppatsu { get; set; } = false;
    public bool IsRinshan { get; set; } = false;
    public bool IsChankan { get; set; } = false;
    public bool IsHaitei { get; set; } = false;
    public bool IsHoutei { get; set; } = false;
    public bool IsDaburuRiichi { get; set; } = false;
    public bool IsNagashiMangan { get; set; } = false;
    public bool IsTenhou { get; set; } = false;
    public bool IsRenhou { get; set; } = false;
    public bool IsChiihou { get; set; } = false;
    public bool IsOpenRiichi { get; set; } = false;

    public bool IsDealer { get; set; } = false;
    public int? PlayerWind { get; set; } = null;
    public int? RoundWind { get; set; } = null;
    public int Paarenchan { get; set; } = 0;

    public int KyoutakuNumber { get; set; } = 0;
    public int TsumiNumber { get; set; } = 0;

    public HandConfig(
        bool isTsumo = false,
        bool isRiichi = false,
        bool isIppatsu = false,
        bool isRinshan = false,
        bool isChankan = false,
        bool isHaitei = false,
        bool isHoutei = false,
        bool isDaburuRiichi = false,
        bool isNagashiMangan = false,
        bool isTenhou = false,
        bool isRenhou = false,
        bool isChiihou = false,
        bool isOpenRiichi = false,
        int? playerWind = null,
        int? roundWind = null,
        int kyoutakuNumber = 0,
        int tsumiNumber = 0,
        int paarenchan = 0,
        OptionalRules? options = null)
    {
        Yaku = new YakuConfig();
        Options = options ?? new OptionalRules();

        IsTsumo = isTsumo;
        IsRiichi = isRiichi;
        IsIppatsu = isIppatsu;
        IsRinshan = isRinshan;
        IsChankan = isChankan;
        IsHaitei = isHaitei;
        IsHoutei = isHoutei;
        IsDaburuRiichi = isDaburuRiichi;
        IsNagashiMangan = isNagashiMangan;
        IsTenhou = isTenhou;
        IsRenhou = isRenhou;
        IsChiihou = isChiihou;
        IsOpenRiichi = isOpenRiichi;

        PlayerWind = playerWind;
        RoundWind = roundWind;
        IsDealer = playerWind == Constants.East;
        Paarenchan = paarenchan;

        KyoutakuNumber = kyoutakuNumber;
        TsumiNumber = tsumiNumber;
    }
}