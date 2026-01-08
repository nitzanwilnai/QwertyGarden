using UnityEngine;

namespace QwertyGarden
{
    public enum MENU_STATE { MAIN_MENU, GARDEN_SELECTION, KEYBOARD_SELECTION, FLOWER_SELECTION, EDIT_GARDEN, IN_GAME, SETTINGS, PRESTIGE };
    public enum GAME_TYPE { LESSON, COZY };
    public class MetaData
    {
        public double Coins;
        public GAME_TYPE GameType;
        public MENU_STATE MenuState;
        public MENU_STATE PrevMenuState;
        public int KeyboardIndex;
        public float LastTimeStamp;
        public float GrowTime;

        public int TutorialFlags;

        public bool ShowPrestige;
        public int Prestige;

        public bool SFX;
        public bool Music;
    }
}