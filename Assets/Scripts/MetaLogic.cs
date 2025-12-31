using QwertyGarden;
using UnityEngine;

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
    }
}