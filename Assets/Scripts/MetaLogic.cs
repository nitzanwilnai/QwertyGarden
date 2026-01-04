using QwertyGarden;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public static class MetaLogic
    {
        public static void Init(MetaData metaData, Balance balance)
        {
            metaData.Coins = balance.StartingCoins;
        }

        public static void SetMenuState(MetaData metaData, MENU_STATE newMenuState)
        {
            metaData.MenuState = newMenuState;
        }

        public static void SellCollectedFlowers(MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            int totalSellValue = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                totalSellValue += balance.FlowerSeedCost[flowerType] * keyboardData.FlowerCount[flowerType];
            metaData.Coins += totalSellValue;

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                keyboardData.FlowerCount[flowerType] = 0;
        }
    }
}