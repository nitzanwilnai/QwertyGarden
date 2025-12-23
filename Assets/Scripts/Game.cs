using System;
using UnityEngine;
using CommonTools;
using UnityEngine.InputSystem;
using System.IO;

namespace QwertyGarden
{
    public class Game : Singleton<Game>
    {
        public Camera Camera;
        public Board Board;
        public GameObject UIMainMenu;
        public GameObject UIKeyboadSelection;
        public GameObject UIFlowerSelection;

        MainMenuVisual m_mainMenuVisual = new();
        KeyboardSelectionVisual m_keyboardSelectionVisual = new();
        FlowerSelectionVisual m_flowerSlipSelectionVisual = new();
        GardenSelectionVisual m_gardenSelectionVisual = new();

        Balance m_balance = new Balance();
        LessonData m_lessonData = new LessonData();
        GameData m_gameData = new GameData();
        MetaData m_metaData = new MetaData();

        KeyboardData m_keyboardData = new KeyboardData();
        int m_keyboardIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            Board.Init(m_metaData, m_lessonData, m_gameData, m_balance, Camera);
            m_balance.LoadBalance();
            KeyboardLogic.InitKeyboardData(m_keyboardData);
            LessonLogic.InitLessonData(m_lessonData, m_balance);

            m_mainMenuVisual.Init(UIMainMenu);
            m_keyboardSelectionVisual.Init(UIKeyboadSelection, m_balance);
            m_gardenSelectionVisual.Init(UIKeyboadSelection, m_balance);
            m_flowerSlipSelectionVisual.Init(UIFlowerSelection);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_metaData.GameType = GAME_TYPE.COZY;

            SetMenuState(MENU_STATE.MAIN_MENU);

            // if (m_metaData.GameType == GAME_TYPE.LESSON)
            //     Board.StartLesson();
            // else if (m_metaData.GameType == GAME_TYPE.COZY)
            //     Board.StartGame();
        }

        public void LoadKeyboard(int keyboardIndex)
        {
            m_keyboardIndex = keyboardIndex;
            KeyboardDataIO.LoadKeyboard(m_keyboardData, m_keyboardIndex);
        }

        public void SetMenuState(MENU_STATE newMenuState)
        {
            if (m_metaData.MenuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.GARDEN_SELECTION)
                m_gardenSelectionVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.KEYBOARD_SELECTION)
                m_keyboardSelectionVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.FLOWER_SELECTION)
                m_flowerSlipSelectionVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.Hide();

            m_metaData.MenuState = newMenuState;

            if (m_metaData.MenuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.GARDEN_SELECTION)
                m_gardenSelectionVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.KEYBOARD_SELECTION)
                m_keyboardSelectionVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.FLOWER_SELECTION)
                m_flowerSlipSelectionVisual.Show(m_keyboardData, m_keyboardIndex);
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.StartGame(m_keyboardData);
        }

        // Update is called once per frame
        void Update()
        {
            float dt = Time.deltaTime;

            if (m_metaData.MenuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Tick();
            else if (m_metaData.MenuState == MENU_STATE.GARDEN_SELECTION)
                m_gardenSelectionVisual.Tick(dt);
            else if (m_metaData.MenuState == MENU_STATE.KEYBOARD_SELECTION)
                m_keyboardSelectionVisual.Tick(dt);
            else if (m_metaData.MenuState == MENU_STATE.FLOWER_SELECTION)
                m_flowerSlipSelectionVisual.Tick();
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.Tick(dt);


#if UNITY_EDITOR
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (!Directory.Exists("Screenshots"))
                    Directory.CreateDirectory("Screenshots");

                DateTimeOffset now = DateTime.UtcNow;
                string name = "Screenshots/" + Screen.width + "x" + Screen.height + "_" + now.ToString("yyyy-MM-dd HH.mm.ss") + ".png";
                ScreenCapture.CaptureScreenshot(name);
            }
#endif
        }
    }
}