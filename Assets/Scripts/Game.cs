using System;
using UnityEngine;
using CommonTools;
using UnityEngine.InputSystem;
using System.IO;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
using Steamworks;
#endif

namespace QwertyGarden
{
    public class Game : Singleton<Game>
    {
        public Camera Camera;
        public Board Board;
        public GameObject UIEditFlowers;
        public GameObject UISettings;
        public GameObject UIPrestige;
        public GameObject UINoKeyboard;
        public GameObject UIDemo;
        public GameObject UITitle;

        EditFlowersVisual m_editFlowersVisual = new();
        SettingsVisual m_settingsVisual = new();
        PrestigeVisual m_prestigeVisual = new();
        TitleVisual m_titleVisual = new();

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

            m_metaData.MenuState = MENU_STATE.TITLE;
            MetaDataIO.TryLoadMeta(m_metaData);

            SoundManager.Instance.Init(m_metaData);
            MusicManager.Instance.Init(m_metaData);

            // m_metaData.Prestige = 15;

            if (KeyboardDataIO.KeyboardDataExists(0))
                LoadKeyboard(0);
            else
            {
                LoadNewKeyboard(0);
            }
            // MetaDataIO.SaveMeta(m_metaData);

            m_editFlowersVisual.Init(UIEditFlowers, m_metaData, m_keyboardData, m_balance);
            m_settingsVisual.Init(UISettings, m_metaData);
            m_prestigeVisual.Init(UIPrestige, m_metaData, m_keyboardData, m_balance);
            m_titleVisual.Init(m_metaData, UITitle, m_balance);

            UIDemo.SetActive(false);
#if DEMO
            UIDemo.SetActive(true);
#endif

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
            bool steamInit = SteamAPI.Init();
            if (!steamInit)
            {
                Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            }
            else
                Debug.Log("Steam Init Success");

#endif

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_metaData.GameType = GAME_TYPE.COZY;
            // m_metaData.MenuState = MENU_STATE.TITLE;
            MetaDataIO.SaveMeta(m_metaData);

            m_metaData.PrevMenuState = m_metaData.MenuState;
            m_metaData.MenuState = MENU_STATE.TITLE;
            showNewMenu();

            MusicManager.Instance.PlayMusic();

            // if (m_metaData.GameType == GAME_TYPE.LESSON)
            //     Board.StartLesson();
            // else if (m_metaData.GameType == GAME_TYPE.COZY)
            //     Board.StartGame();
        }

        public void LoadKeyboard(int keyboardIndex)
        {
            m_metaData.KeyboardIndex = keyboardIndex;
            MetaDataIO.SaveMeta(m_metaData);
            if (KeyboardDataIO.LoadKeyboard(m_keyboardData, m_metaData.KeyboardIndex))
            {
                // v3 loaded
            }
            else if (KeyboardDataIO.LoadKeyboardV2(m_keyboardData, m_metaData.KeyboardIndex))
            {
                // v2 loaded
            }
            // else if (KeyboardDataIO.LoadKeyboardV1(m_keyboardData, m_metaData.KeyboardIndex))
            // {
            //     // v1 loaded
            // }

#if UNITY_EDITOR
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                m_keyboardData.FlowerType[keyIdx] = Mathf.FloorToInt(UnityEngine.Random.value * 4.0f + 11.0f);
                m_keyboardData.FlowerProgress[keyIdx] = Mathf.RoundToInt(UnityEngine.Random.value * 1.0f + 8.0f);
            }
#endif
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
            hidePreviousMenu();

            m_metaData.PrevMenuState = m_metaData.MenuState;
            m_metaData.MenuState = newMenuState;

            MetaDataIO.SaveMeta(m_metaData);

            showNewMenu();
        }

        private void showNewMenu()
        {
            if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
                m_editFlowersVisual.Show(m_keyboardData);
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.PlayCozy(m_keyboardData);
            else if (m_metaData.MenuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.PRESTIGE)
                m_prestigeVisual.Show();
            else if (m_metaData.MenuState == MENU_STATE.TITLE)
                m_titleVisual.Show();
        }

        private void hidePreviousMenu()
        {
            if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
                m_editFlowersVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.Hide();
            else if (m_metaData.MenuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.PRESTIGE)
                m_prestigeVisual.Hide();
            else if (m_metaData.MenuState == MENU_STATE.TITLE)
                m_titleVisual.Hide();
        }

        // Update is called once per frame
        void Update()
        {
            float dt = Time.deltaTime;

            bool noKeyboard = Keyboard.current == null;
            if (UINoKeyboard.activeSelf != noKeyboard)
                UINoKeyboard.SetActive(noKeyboard);

            if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
                m_editFlowersVisual.Tick(dt);
            else if (m_metaData.MenuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Tick();
            else if (m_metaData.MenuState == MENU_STATE.PRESTIGE)
                m_prestigeVisual.Tick();
            else if (m_metaData.MenuState == MENU_STATE.IN_GAME)
                Board.Tick(dt);
            else if (m_metaData.MenuState == MENU_STATE.TITLE)
                m_titleVisual.Tick();

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

#endif
#if VIDEO || UNITY_EDITOR
            if (Keyboard.current.digit0Key.wasPressedThisFrame)
            {
                m_metaData.Coins *= 2.0d;
                SoundManager.Instance.PlaySFXMoney();
                Board.UpdateUI();
            }
            if (Keyboard.current.equalsKey.wasPressedThisFrame)
            {
                m_metaData.Coins += 1000;
                SoundManager.Instance.PlaySFXMoney();
                Board.UpdateUI();
            }
            if (Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                Board.IncAllFlowerProgress();
                SoundManager.Instance.PlaySFXMoney();
                Board.UpdateUI();
            }
#endif
        }

        public void ToggleSFX(NavButtonGUI navButtonGUI)
        {
            m_metaData.SFX = !m_metaData.SFX;

            CommonVisual.ShowSettings(navButtonGUI, m_metaData);
        }

        public void ToggleMusic(NavButtonGUI navButtonGUI)
        {
            m_metaData.Music = !m_metaData.Music;
            MusicManager.Instance.Mute();

            CommonVisual.ShowSettings(navButtonGUI, m_metaData);
        }

        public void ToggleWPM(NavButtonGUI navButtonGUI)
        {
            m_metaData.WPM = !m_metaData.WPM;
            if (m_metaData.MenuState == MENU_STATE.IN_GAME)
            {
                Board.UpdateUI();
            }

            CommonVisual.ShowSettings(navButtonGUI, m_metaData);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ToggleShowSettings()
        {
            if (m_metaData.MenuState == MENU_STATE.IN_GAME)
            {
                Board.ToggleShowSettings();
            }
            else if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
            {
                m_editFlowersVisual.ToggleShowSettings();
            }
        }

        public void ShowTutorial()
        {
            if (m_metaData.MenuState == MENU_STATE.IN_GAME)
            {
                Board.ShowTutorial();
            }
            else if (m_metaData.MenuState == MENU_STATE.EDIT_GARDEN)
            {
                m_editFlowersVisual.ShowTutorial();
            }
        }

        public void ShowInvoice()
        {
            Board.ShowInvoice();
        }

        public void BuySelectedSeedsAndPlay()
        {
            m_editFlowersVisual.BuySelectedSeedsAndPlay();
        }

        public void GoToWishlist()
        {
            Application.OpenURL("steam://openurl/https://store.steampowered.com/app/4255650/Qwerty_Garden/");
        }
    }
}