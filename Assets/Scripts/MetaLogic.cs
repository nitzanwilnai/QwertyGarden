namespace QwertyGarden
{
    public static class MetaLogic
    {
        public static void Init(MetaData metaData, Balance balance)
        {
            metaData.Coins = balance.StartingCoins;
            metaData.SFX = true;
            metaData.Music = true;
            metaData.WPM = true;
        }

        public static void SetMenuState(MetaData metaData, MENU_STATE newMenuState)
        {
            metaData.MenuState = newMenuState;
        }

        public static double GetSellValue(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            double totalSellValue = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                totalSellValue += balance.FlowerSellValue[flowerType] * keyboardData.FlowerCount[flowerType];

            float accuracyBonus = KeyboardLogic.GetAccuracyBonus(metaData, keyboardData);
            if (accuracyBonus == 0.0f)
                accuracyBonus = 2.0f;
            float wpmBonus = KeyboardLogic.GetWPMBonus(metaData, keyboardData);
            return System.Math.Floor(totalSellValue * accuracyBonus * wpmBonus);
        }

        public static void SellCollectedFlowers(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            metaData.Coins += GetSellValue(metaData, keyboardData, balance);

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                keyboardData.FlowerCount[flowerType] = 0;
        }

        public static bool IsFlagSet(int flags, int index)
        {
            return (flags & (1 << index)) > 0;
        }

        public static void SetFlag(ref int flags, int index)
        {
            flags |= 1 << index;
        }

        public static int CountDigits(double value)
        {
            if (value == 0.0) return 1;
            value = System.Math.Abs(value);

            return (int)System.Math.Floor(System.Math.Log10(value)) + 1;
        }

        public static double PowerOf10(double value)
        {
            if (value < 1.0)
                return 0.0;

            double result = 1.0;

            while (value >= 10.0)
            {
                value *= 0.1;
                result *= 10.0;
            }

            return result;
        }

        private static readonly (double value, string name)[] Scales =
        {
                (1e18, "quintillion"),
                (1e15, "quadrillion"),
                (1e12, "trillion"),
                (1e9,  "billion"),
                (1e6,  "million"),
                // (1e3,  "thousand"),
            };

        public static string ToShortScale(double number)
        {
            double abs = System.Math.Abs(number);

            foreach (var (value, name) in Scales)
            {
                if (abs >= value)
                {
                    double scaled = number / value;
                    return $"{formatThreeDigits(scaled)} {name}";
                }
            }

            return number.ToString("N0");
        }

        private static string formatThreeDigits(double value)
        {
            double abs = System.Math.Abs(value);

            if (abs >= 100.0)
                return value.ToString("0");      // 3 digits, no decimals
            if (abs >= 10.0)
                return value.ToString("0.0");    // 2 digits + 1 decimal
            return value.ToString("0.00");       // 1 digit + 2 decimals
        }

        public static void TryShowPrestige(MetaData metaData)
        {
            if (metaData.Coins >= 1000)
                metaData.ShowPrestige = true;
        }

        public static double GetPrestigeCost(MetaData metaData, Balance balance)
        {
            double prestigeCost = balance.PrestigeCost;

            for (int i = 0; i < metaData.Prestige; i++)
                prestigeCost *= balance.PrestigeMultiplier;

            return prestigeCost;
        }

        public static bool TryPrestige(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            double prestigeCost = GetPrestigeCost(metaData, balance);
            double totalSellValue = MetaLogic.GetSellValue(metaData, keyboardData, balance);

            if (prestigeCost < metaData.Coins + totalSellValue)
            {
                metaData.Coins -= prestigeCost;
                metaData.Prestige++;

                for (int i = 0; i < 26; i++)
                    keyboardData.FlowerType[i] = 0;

                for (int i = 0; i < 26; i++)
                    keyboardData.NewFlowerType[i] = 0;

                for (int i = 0; i < 26; i++)
                    keyboardData.FlowerProgress[i] = 0;

                for (int i = 0; i < 26; i++)
                    keyboardData.FlowerCount[i] = 0;

                for (int i = 0; i < 26; i++)
                    keyboardData.GrowTime[i] = 0.0f;

                return true;
            }
            return false;
        }
    }
}