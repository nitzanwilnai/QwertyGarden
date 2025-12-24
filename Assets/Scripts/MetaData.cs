using UnityEngine;

namespace QwertyGarden
{
    public enum MENU_STATE { MAIN_MENU, GARDEN_SELECTION, KEYBOARD_SELECTION, FLOWER_SELECTION, IN_GAME };
    public enum GAME_TYPE { LESSON, COZY };
    public class MetaData
    {
        public int Coins;
        public GAME_TYPE GameType;
        public MENU_STATE MenuState;
        public int KeyboardIndex;
    }
}