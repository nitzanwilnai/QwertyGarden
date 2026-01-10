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
        public GameObject UIGardenSelection;
        public GameObject UIKeyboardSelection;
        public GameObject UIEditFlowers;
        public GameObject UISettings;
        public GameObject UIPrestige;

        MainMenuVisual m_mainMenuVisual = new();
        KeyboardSelectionVisual m_keyboardSelectionVisual = new();
        FlowerSelectionVisual m_flowerSelectionVisual = new();
        GardenSelectionVisual m_gardenSelectionVisual = new();
        EditFlowersVisual m_editFlowersVisual = new();
        SettingsVisual m_settingsVisual = new();
        PrestigeVisual m_prestigeVisual = new();

        Balance m_balance = new Balance();
        LessonData m_lessonData = new LessonData();
        MetaData m_metaData = new MetaData();

        KeyboardData m_keyboardData = new KeyboardData();

        protected override void Awake()
        {
            base.Awake();
            AssetManager.Instance.LoadCommonAssetBundle();

            m_balance.LoadBalance();
            Board.Init(m_metaData, m_lessonData, m_balance, Camera);
            MetaLogic.Init(m_metaData, m_balance);
            KeyboardLogic.InitKeyboardData(m_keyboardData);
            LessonLogic.InitLessonData(m_lessonData, m_balance);

            MetaDataIO.LoadMeta(m_metaData);

            SoundManager.Instance.Init(m_metaData);
            MusicManager.Instance.Init(m_metaData);

            // m_metaData.Prestige = 15;

            if (KeyboardDataIO.KeyboardDataExists(0))
                LoadKeyboard(0);
            else
            {
                LoadNewKeyboard(0);
            }
            MetaDataIO.SaveMeta(m_metaData);

            m_mainMenuVisual.Init(UIMainMenu, m_metaData, m_keyboardData, m_balance);
            m_keyboardSelectionVisual.Init(UIKeyboadSelection, m_balance);
            m_gardenSelectionVisual.Init(UIGardenSelection, m_balance, m_metaData);
            m_flowerSelectionVisual.Init(UIFlowerSelection, m_metaData, m_balance);
            m_keyboardSelectionVisual.Init(UIKeyboardSelection, m_balance);
            m_editFlowersVisual.Init(UIEditFlowers, m_metaData, m_balance);
            m_settingsVisual.Init(UISettings, m_metaData);
            m_prestigeVisual.Init(UIPrestige, m_metaData, m_keyboardData, m_balance);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_metaData.GameType = GAME_TYPE.COZY;
            m_metaData.MenuState = MENU_STATE.MAIN_MENU;
            MetaDataIO.SaveMeta(m_metaData);

            SetMenuState(MENU_STATE.MAIN_MENU);

            // if (m_metaData.GameType == GAME_TYPE.LESSON)
            //     Board.StartLesson();
            // else if (m_metaData.GameType == GAME_TYPE.COZY)
            //     Board.StartGame();
        }

        public void LoadKeyboard(int keyboardIndex)
        {
            m_metaData.KeyboardIndex = keyboardIndex;
            MetaDataIO.SaveMeta(m_metaData);
            KeyboardDataIO.LoadKeyboard(m_keyboardData, m_metaData.KeyboardIndex);
        }

        public void LoadNewKeyboard(int keyboardIndex)
        {
            m_metaData.KeyboardIndex = keyboardIndex;
            MetaDataIO.SaveMeta(m_metaData);
            KeyboardLogic.InitKeyboardData(m_keyboardData);
            CozyLogic.StartCozy(m_keyboardData, m_metaData, m_balance);
            KeyboardDataIO.SaveKeyboard(m_keyboardData, m_metaData.KeyboardIndex);
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
                m_flowerSelectionVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
                m_editFlowersVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.Hide();
            else if (m_metaData.MenuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.PRESTIGE)
                m_prestigeVisual.Hide();

            m_metaData.PrevMenuState = m_metaData.MenuState;
            m_metaData.MenuState = newMenuState;

            MetaDataIO.SaveMeta(m_metaData);

            if (m_metaData.MenuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.GARDEN_SELECTION)
                m_gardenSelectionVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.KEYBOARD_SELECTION)
                m_keyboardSelectionVisual.Show(m_keyboardData, m_metaData);
            else if (m_metaData.MenuState == MENU_STATE.FLOWER_SELECTION)
                m_flowerSelectionVisual.Show(m_keyboardData);
            else if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
                m_editFlowersVisual.Show(m_keyboardData);
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.PlayCozy(m_keyboardData);
            else if (m_metaData.MenuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.PRESTIGE)
                m_prestigeVisual.Show();
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
                m_flowerSelectionVisual.Tick(dt);
            else if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
                m_editFlowersVisual.Tick(dt);
            else if (m_metaData.MenuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Tick();
            else if (m_metaData.MenuState == MENU_STATE.PRESTIGE)
                m_prestigeVisual.Tick();
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.Tick(dt);

#if UNITY_EDITOR
            if (Keyboard.current.deleteKey.wasPressedThisFrame)
                Debug.Log("delete key pressed!");
            if (Keyboard.current.backspaceKey.wasPressedThisFrame)
                Debug.Log("back space key pressed!");


            if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                if (!Directory.Exists("Screenshots"))
                    Directory.CreateDirectory("Screenshots");

                DateTimeOffset now = DateTime.UtcNow;
                string name = "Screenshots/" + Screen.width + "x" + Screen.height + "_" + now.ToString("yyyy-MM-dd HH.mm.ss") + ".png";
                Debug.Log("Screenshot " + name + " taken!");
                ScreenCapture.CaptureScreenshot(name);
            }

            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                m_metaData.Coins *= 2.0d;
                SoundManager.Instance.PlaySFXMoney();
            }
            if (Keyboard.current.equalsKey.wasPressedThisFrame)
            {
                m_metaData.Coins += 1000;
                SoundManager.Instance.PlaySFXMoney();
            }
#endif
        }
    }
}