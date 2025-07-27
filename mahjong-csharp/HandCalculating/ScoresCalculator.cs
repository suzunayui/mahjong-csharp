namespace Mahjong.HandCalculating;

public class ScoresCalculator
{
    public virtual Dictionary<string, object> CalculateScores(int han, int fu, HandConfig config, bool isYakuman = false)
    {
        var yakuLevel = "";
        var rounded = 0;
        var doubleRounded = 0;
        var fourRounded = 0;
        var sixRounded = 0;

        // Kazoe hand
        if (han >= 13 && !isYakuman)
        {
            if (config.Options.KazoeLimit == HandConstants.KazoeLimit)
            {
                han = 13;
                yakuLevel = "kazoe yakuman";
            }
            else if (config.Options.KazoeLimit == HandConstants.KazoeSanbaiman)
            {
                han = 12;
                yakuLevel = "kazoe sanbaiman";
            }
        }

        if (han >= 5)
        {
            if (han >= 78)
            {
                yakuLevel = "6x yakuman";
                if (config.Options.LimitToSextupleYakuman)
                {
                    rounded = 48000;
                }
                else
                {
                    var extraHan = (han - 78) / 13;
                    rounded = 48000 + (extraHan * 8000);
                }
            }
            else if (han >= 65)
            {
                yakuLevel = "5x yakuman";
                rounded = 40000;
            }
            else if (han >= 52)
            {
                yakuLevel = "4x yakuman";
                rounded = 32000;
            }
            else if (han >= 39)
            {
                yakuLevel = "3x yakuman";
                rounded = 24000;
            }
            else if (han >= 26)
            {
                yakuLevel = "2x yakuman";
                rounded = 16000;
            }
            else if (han >= 13)
            {
                yakuLevel = "yakuman";
                rounded = 8000;
            }
            else if (han >= 11)
            {
                yakuLevel = "sanbaiman";
                rounded = 6000;
            }
            else if (han >= 8)
            {
                yakuLevel = "baiman";
                rounded = 4000;
            }
            else if (han >= 6)
            {
                yakuLevel = "haneman";
                rounded = 3000;
            }
            else
            {
                yakuLevel = "mangan";
                rounded = 2000;
            }

            doubleRounded = rounded * 2;
            fourRounded = doubleRounded * 2;
            sixRounded = doubleRounded * 3;
        }
        else // han < 5
        {
            var basePoints = fu * (int)Math.Pow(2, 2 + han);
            rounded = ((basePoints + 99) / 100) * 100;
            doubleRounded = ((2 * basePoints + 99) / 100) * 100;
            fourRounded = ((4 * basePoints + 99) / 100) * 100;
            sixRounded = ((6 * basePoints + 99) / 100) * 100;

            var isKiriage = false;
            if (config.Options.Kiriage)
            {
                if ((han == 4 && fu == 30) || (han == 3 && fu == 60))
                {
                    yakuLevel = "kiriage mangan";
                    isKiriage = true;
                }
            }
            else
            {
                if (rounded > 2000)
                {
                    yakuLevel = "mangan";
                }
            }

            if (rounded > 2000 || isKiriage)
            {
                rounded = 2000;
                doubleRounded = rounded * 2;
                fourRounded = doubleRounded * 2;
                sixRounded = doubleRounded * 3;
            }
        }

        int main, additional, mainBonus, additionalBonus;

        if (config.IsTsumo)
        {
            main = doubleRounded;
            mainBonus = 100 * config.TsumiNumber;
            additionalBonus = mainBonus;

            if (config.IsDealer)
            {
                additional = main;
            }
            else
            {
                additional = rounded;
            }
        }
        else // Ron
        {
            additional = 0;
            additionalBonus = 0;
            mainBonus = 300 * config.TsumiNumber;

            if (config.IsDealer)
            {
                main = sixRounded;
            }
            else
            {
                main = fourRounded;
            }
        }

        var kyoutakuBonus = 1000 * config.KyoutakuNumber;
        var total = (main + mainBonus) + 2 * (additional + additionalBonus) + kyoutakuBonus;

        if (config.IsNagashiMangan)
        {
            yakuLevel = "nagashi mangan";
        }

        return new Dictionary<string, object>
        {
            ["main"] = main,
            ["main_bonus"] = mainBonus,
            ["additional"] = additional,
            ["additional_bonus"] = additionalBonus,
            ["kyoutaku_bonus"] = kyoutakuBonus,
            ["total"] = total,
            ["yaku_level"] = yakuLevel
        };
    }
}