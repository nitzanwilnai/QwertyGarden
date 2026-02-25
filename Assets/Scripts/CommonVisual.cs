using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace QwertyGarden
{
    public class TutorialGUI
    {
        public GameObject GO;
        public GameObject Instruction;
        public GameObject Bubble;
        public GameObject Darken;
        public bool TutorialShown;
    }

    public class NavButtonGUI
    {
        public GameObject PlayButton;
        public GameObject GardenButton;
        public GameObject PrestigeButton;
        public GameObject SFXButton;
        public GameObject MusicButton;
        public GameObject WPMButton;
        public GameObject ExitButton;
        public GameObject FontButton;

        public GameObject SFXOn;
        public GameObject SFXOff;
        public GameObject MusicOn;
        public GameObject MusicOff;
        public GameObject WPMOn;
        public GameObject WPMOff;
        public GameObject Font1;
        public GameObject Font2;
    }

    public static class CommonVisual
    {
        public static void InitTutorialGUI(GUIRef guiRef, TutorialGUI tutorialGUI)
        {
            tutorialGUI.GO = guiRef.GetGameObject("Tutorial");
            tutorialGUI.Darken = guiRef.GetGameObject("Darken");
            GUIRef tutorialGUIRef = tutorialGUI.GO.GetComponent<GUIRef>();
            tutorialGUI.Bubble = tutorialGUIRef.GetGameObject("Bubble");
            tutorialGUI.Instruction = tutorialGUIRef.GetGameObject("Instruction");
        }

        public static void ShowTutorial(TutorialGUI tutorialGUI)
        {
            tutorialGUI.Bubble.SetActive(false);
            tutorialGUI.Bubble.SetActive(true);
            tutorialGUI.Instruction.SetActive(false);
            tutorialGUI.Darken.SetActive(true);
            tutorialGUI.TutorialShown = true;
            SoundManager.Instance.PlaySFXHitBubble();
        }

        public static void HideTutorial(TutorialGUI tutorialGUI)
        {
            tutorialGUI.Bubble.SetActive(false);
            tutorialGUI.Instruction.SetActive(true);
            tutorialGUI.Darken.SetActive(false);
            tutorialGUI.TutorialShown = false;
            SoundManager.Instance.PlaySFXHitBubble();
        }

        public static bool AutoShowTutorial(MetaData metaData)
        {
            if (!Logic.IsFlagSet(metaData.TutorialFlags, (int)metaData.MenuState))
            {
                Logic.SetFlag(ref metaData.TutorialFlags, (int)metaData.MenuState);
                MetaDataIO.SaveMeta(metaData);
                return true;
            }
            return false;
        }

        public static bool CheckTutorialKey(TutorialGUI tutorialGUI)
        {
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                if (!tutorialGUI.TutorialShown)
                {
                    ShowTutorial(tutorialGUI);
                    return true;
                }
                else
                {
                    HideTutorial(tutorialGUI);
                    return true;
                }
            }
            return false;
        }

        public static void UpdateSettingsText(MetaData metaData, TextMeshProUGUI SettingsText)
        {
            string sfxOn = metaData.SFX ? "on" : "off";
            string musicOn = metaData.Music ? "on" : "off";
            SettingsText.text = "<b>1</b> sfx " + sfxOn + "\n<b>2</b> music " + musicOn;
        }

        public static void AutoCanvasScaler(GameObject UI)
        {
            float ratio = (float)Screen.width / (float)Screen.height;

            CanvasScaler canvasScaler = UI.GetComponent<CanvasScaler>();

            if (ratio >= 16.0f / 9.0f)
                canvasScaler.matchWidthOrHeight = 1.0f;
            else
                canvasScaler.matchWidthOrHeight = 0.0f;
        }

        public static void InitButtons(GUIRef guiRef, NavButtonGUI navButtonGUI)
        {
            GUIRef buttonGUIRef = guiRef.GetGameObject("Buttons").GetComponent<GUIRef>();

            Button playButton = buttonGUIRef.GetButton("Play");
            Button gardenButton = buttonGUIRef.GetButton("Garden");
            Button prestigeButton = buttonGUIRef.GetButton("Prestige");

            playButton.onClick.AddListener(Game.Instance.BuySelectedSeedsAndPlay);
            gardenButton.onClick.AddListener(Game.Instance.ShowInvoice);
            buttonGUIRef.GetButton("Tutorial").onClick.AddListener(Game.Instance.ShowTutorial);
            buttonGUIRef
                .GetButton("Settings")
                .onClick.AddListener(Game.Instance.ToggleShowSettings);
            prestigeButton.onClick.AddListener(() =>
            {
                Game.Instance.SetMenuState(MENU_STATE.PRESTIGE);
            });

            buttonGUIRef.GetButton("Wishlist").onClick.AddListener(Game.Instance.GoToWishlist);
            buttonGUIRef.GetButton("Wishlist").gameObject.SetActive(false);
#if DEMO
            buttonGUIRef.GetButton("Wishlist").gameObject.SetActive(true);
#endif

            Button sfxButton = buttonGUIRef.GetButton("SFX");
            Button musicButton = buttonGUIRef.GetButton("Music");
            Button wpmButton = buttonGUIRef.GetButton("WPM");
            Button exitButton = buttonGUIRef.GetButton("Exit");
            Button fontButton = buttonGUIRef.GetButton("Font");

            sfxButton.onClick.AddListener(() =>
            {
                Game.Instance.ToggleSFX(navButtonGUI);
            });
            musicButton.onClick.AddListener(() =>
            {
                Game.Instance.ToggleMusic(navButtonGUI);
            });
            wpmButton.onClick.AddListener(() =>
            {
                Game.Instance.ToggleWPM(navButtonGUI);
            });
            exitButton.onClick.AddListener(Game.Instance.ExitGame);
            fontButton.onClick.AddListener(() =>
            {
                Game.Instance.ToggleFont(navButtonGUI);
            });

            navButtonGUI.PlayButton = playButton.gameObject;
            navButtonGUI.GardenButton = gardenButton.gameObject;
            navButtonGUI.PrestigeButton = prestigeButton.gameObject;
            navButtonGUI.SFXButton = sfxButton.gameObject;
            navButtonGUI.WPMButton = wpmButton.gameObject;
            navButtonGUI.MusicButton = musicButton.gameObject;
            navButtonGUI.ExitButton = exitButton.gameObject;
            navButtonGUI.FontButton = fontButton.gameObject;

            navButtonGUI.SFXOn = buttonGUIRef.GetGameObject("SFXOn");
            navButtonGUI.SFXOff = buttonGUIRef.GetGameObject("SFXOff");
            navButtonGUI.MusicOn = buttonGUIRef.GetGameObject("MusicOn");
            navButtonGUI.MusicOff = buttonGUIRef.GetGameObject("MusicOff");
            navButtonGUI.WPMOn = buttonGUIRef.GetGameObject("WPMOn");
            navButtonGUI.WPMOff = buttonGUIRef.GetGameObject("WPMOff");
            navButtonGUI.Font1 = buttonGUIRef.GetGameObject("Font1");
            navButtonGUI.Font2 = buttonGUIRef.GetGameObject("Font2");
        }

        public static void ShowSettings(NavButtonGUI navButtonGUI, MetaData metaData)
        {
            navButtonGUI.SFXButton.SetActive(true);
            navButtonGUI.MusicButton.SetActive(true);
            navButtonGUI.WPMButton.SetActive(true);
            navButtonGUI.ExitButton.SetActive(true);
            navButtonGUI.FontButton.SetActive(true);

            navButtonGUI.SFXOn.SetActive(metaData.SFX);
            navButtonGUI.SFXOff.SetActive(!metaData.SFX);
            navButtonGUI.MusicOn.SetActive(metaData.Music);
            navButtonGUI.MusicOff.SetActive(!metaData.Music);
            navButtonGUI.WPMOn.SetActive(metaData.WPM);
            navButtonGUI.WPMOff.SetActive(!metaData.WPM);
            navButtonGUI.Font1.SetActive(metaData.Font == 0);
            navButtonGUI.Font2.SetActive(metaData.Font == 1);
        }

        public static void HideSettings(NavButtonGUI navButtonGUI)
        {
            navButtonGUI.SFXButton.SetActive(false);
            navButtonGUI.MusicButton.SetActive(false);
            navButtonGUI.WPMButton.SetActive(false);
            navButtonGUI.ExitButton.SetActive(false);
            navButtonGUI.FontButton.SetActive(false);
        }

        private static readonly (double value, string name)[] Scales =
        {
            (1e18, "quintillion"),
            (1e15, "quadrillion"),
            (1e12, "trillion"),
            (1e9, "billion"),
            (1e6, "million"),
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
                return value.ToString("0"); // 3 digits, no decimals
            if (abs >= 10.0)
                return value.ToString("0.0"); // 2 digits + 1 decimal
            return value.ToString("0.00"); // 1 digit + 2 decimals
        }
    }
}
