using Mahjong.HandCalculating.YakuList;
using Mahjong.HandCalculating.YakuList.Yakuman;

namespace Mahjong.HandCalculating;

public class YakuConfig
{
    // Yaku situations
    public Tsumo Tsumo { get; set; }
    public Riichi Riichi { get; set; }
    public OpenRiichi OpenRiichi { get; set; }
    public DaburuRiichi DaburuRiichi { get; set; }
    public DaburuOpenRiichi DaburuOpenRiichi { get; set; }
    public Ippatsu Ippatsu { get; set; }
    public Chankan Chankan { get; set; }
    public Rinshan Rinshan { get; set; }
    public Haitei Haitei { get; set; }
    public Houtei Houtei { get; set; }
    public NagashiMangan NagashiMangan { get; set; }
    public Renhou Renhou { get; set; }

    // Yaku 1-2 Han
    public Pinfu Pinfu { get; set; }
    public Tanyao Tanyao { get; set; }
    public Iipeiko Iipeiko { get; set; }
    public Chiitoitsu Chiitoitsu { get; set; }
    public Sanshoku Sanshoku { get; set; }
    public Ittsu Ittsu { get; set; }
    public Chantai Chantai { get; set; }
    public Toitoi Toitoi { get; set; }
    public Sanankou Sanankou { get; set; }
    public Sankantsu Sankantsu { get; set; }
    public SanshokuDouko SanshokuDouko { get; set; }
    public Honroto Honroto { get; set; }
    public Shosangen Shosangen { get; set; }
    public Haku Haku { get; set; }
    public Hatsu Hatsu { get; set; }
    public Chun Chun { get; set; }
    public East East { get; set; }
    public South South { get; set; }
    public West West { get; set; }
    public North North { get; set; }
    public YakuhaiPlace YakuhaiPlace { get; set; }
    public YakuhaiRound YakuhaiRound { get; set; }

    // Yaku 3+ Han
    public Ryanpeiko Ryanpeiko { get; set; }
    public Honitsu Honitsu { get; set; }
    public Junchan Junchan { get; set; }
    public Chinitsu Chinitsu { get; set; }

    // Yakuman
    public KokushiMusou Kokushi { get; set; }
    public DaburuKokushiMusou DaburuKokushi { get; set; }
    public Daisangen Daisangen { get; set; }
    public Suuankou Suuankou { get; set; }
    public Suukantsu Suukantsu { get; set; }
    public Tsuisou Tsuisou { get; set; }
    public Ryuisou Ryuisou { get; set; }
    public Chinroto Chinroto { get; set; }
    public ChuurenPoutou ChuurenPoutou { get; set; }
    public Shousuushii Shousuushii { get; set; }
    public DaiSuushii DaiSuushii { get; set; }
    public Tenhou Tenhou { get; set; }
    public Chiihou Chiihou { get; set; }
    public SuuankouTanki SuuankouTanki { get; set; }
    public DaburuChuurenPoutou DaburuChuurenPoutou { get; set; }
    public Daichisei Daichisei { get; set; }
    public Daisharin Daisharin { get; set; }
    public RenhouYakuman RenhouYakuman { get; set; }
    public Paarenchan Paarenchan { get; set; }
    public Sashikomi Sashikomi { get; set; }

    // Dora
    public Dora Dora { get; set; }
    public AkaDora AkaDora { get; set; }

    public YakuConfig()
    {
        var id = 0;

        // Yaku situations
        Tsumo = new Tsumo(id++);
        Riichi = new Riichi(id++);
        OpenRiichi = new OpenRiichi(id++);
        DaburuRiichi = new DaburuRiichi(id++);
        DaburuOpenRiichi = new DaburuOpenRiichi(id++);
        Ippatsu = new Ippatsu(id++);
        Chankan = new Chankan(id++);
        Rinshan = new Rinshan(id++);
        Haitei = new Haitei(id++);
        Houtei = new Houtei(id++);
        NagashiMangan = new NagashiMangan(id++);
        Renhou = new Renhou(id++);

        // Yaku 1-2 Han
        Pinfu = new Pinfu(id++);
        Tanyao = new Tanyao(id++);
        Iipeiko = new Iipeiko(id++);
        Chiitoitsu = new Chiitoitsu(id++);
        Sanshoku = new Sanshoku(id++);
        Ittsu = new Ittsu(id++);
        Chantai = new Chantai(id++);
        Toitoi = new Toitoi(id++);
        Sanankou = new Sanankou(id++);
        Sankantsu = new Sankantsu(id++);
        SanshokuDouko = new SanshokuDouko(id++);
        Honroto = new Honroto(id++);
        Shosangen = new Shosangen(id++);
        Haku = new Haku(id++);
        Hatsu = new Hatsu(id++);
        Chun = new Chun(id++);
        East = new East(id++);
        South = new South(id++);
        West = new West(id++);
        North = new North(id++);
        YakuhaiPlace = new YakuhaiPlace(id++);
        YakuhaiRound = new YakuhaiRound(id++);

        // Yaku 3+ Han
        Ryanpeiko = new Ryanpeiko(id++);
        Honitsu = new Honitsu(id++);
        Junchan = new Junchan(id++);
        Chinitsu = new Chinitsu(id++);

        // Yakuman
        Kokushi = new KokushiMusou(id++);
        DaburuKokushi = new DaburuKokushiMusou(id++);
        Daisangen = new Daisangen(id++);
        Suuankou = new Suuankou(id++);
        Suukantsu = new Suukantsu(id++);
        Tsuisou = new Tsuisou(id++);
        Ryuisou = new Ryuisou(id++);
        Chinroto = new Chinroto(id++);
        ChuurenPoutou = new ChuurenPoutou(id++);
        Shousuushii = new Shousuushii(id++);
        DaiSuushii = new DaiSuushii(id++);
        Tenhou = new Tenhou(id++);
        Chiihou = new Chiihou(id++);
        SuuankouTanki = new SuuankouTanki(id++);
        DaburuChuurenPoutou = new DaburuChuurenPoutou(id++);
        Daichisei = new Daichisei(id++);
        Daisharin = new Daisharin(id++);
        RenhouYakuman = new RenhouYakuman(id++);
        Paarenchan = new Paarenchan(id++);
        Sashikomi = new Sashikomi(id++);

        // Dora
        Dora = new Dora(id++);
        AkaDora = new AkaDora(id++);
    }
}