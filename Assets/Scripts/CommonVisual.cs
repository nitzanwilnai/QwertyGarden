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

        public GameObject SFXOn;
        public GameObject SFXOff;
        public GameObject MusicOn;
        public GameObject MusicOff;
        public GameObject WPMOn;
        public GameObject WPMOff;
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
            if (!MetaLogic.IsFlagSet(metaData.TutorialFlags, (int)metaData.MenuState))
            {
                MetaLogic.SetFlag(ref metaData.TutorialFlags, (int)metaData.MenuState);
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
            buttonGUIRef.GetButton("Settings").onClick.AddListener(Game.Instance.ToggleShowSettings);
            prestigeButton.onClick.AddListener(() => { Game.Instance.SetMenuState(MENU_STATE.PRESTIGE); });

            Button sfxButton = buttonGUIRef.GetButton("SFX");
            Button musicButton = buttonGUIRef.GetButton("Music");
            Button wpmButton = buttonGUIRef.GetButton("WPM");
            Button exitButton = buttonGUIRef.GetButton("Exit");

            sfxButton.onClick.AddListener(() => { Game.Instance.ToggleSFX(navButtonGUI); });
            musicButton.onClick.AddListener(() => { Game.Instance.ToggleMusic(navButtonGUI); });
            wpmButton.onClick.AddListener(() => { Game.Instance.ToggleWPM(navButtonGUI); });
            exitButton.onClick.AddListener(() => { Game.Instance.ExitGame(); });

            navButtonGUI.PlayButton = playButton.gameObject;
            navButtonGUI.GardenButton = gardenButton.gameObject;
            navButtonGUI.PrestigeButton = prestigeButton.gameObject;
            navButtonGUI.SFXButton = sfxButton.gameObject;
            navButtonGUI.WPMButton = wpmButton.gameObject;
            navButtonGUI.MusicButton = musicButton.gameObject;
            navButtonGUI.ExitButton = exitButton.gameObject;

            navButtonGUI.SFXOn = buttonGUIRef.GetGameObject("SFXOn");
            navButtonGUI.SFXOff = buttonGUIRef.GetGameObject("SFXOff");
            navButtonGUI.MusicOn = buttonGUIRef.GetGameObject("MusicOn");
            navButtonGUI.MusicOff = buttonGUIRef.GetGameObject("MusicOff");
            navButtonGUI.WPMOn = buttonGUIRef.GetGameObject("WPMOn");
            navButtonGUI.WPMOff = buttonGUIRef.GetGameObject("WPMOff");
        }

        public static void ShowSettings(NavButtonGUI navButtonGUI, MetaData metaData)
        {
            navButtonGUI.SFXButton.SetActive(true);
            navButtonGUI.MusicButton.SetActive(true);
            navButtonGUI.WPMButton.SetActive(true);
            navButtonGUI.ExitButton.SetActive(true);

            navButtonGUI.SFXOn.SetActive(metaData.SFX);
            navButtonGUI.SFXOff.SetActive(!metaData.SFX);
            navButtonGUI.MusicOn.SetActive(metaData.Music);
            navButtonGUI.MusicOff.SetActive(!metaData.Music);
            navButtonGUI.WPMOn.SetActive(metaData.WPM);
            navButtonGUI.WPMOff.SetActive(!metaData.WPM);
        }

        public static void HideSettings(NavButtonGUI navButtonGUI)
        {
            navButtonGUI.SFXButton.SetActive(false);
            navButtonGUI.MusicButton.SetActive(false);
            navButtonGUI.WPMButton.SetActive(false);
            navButtonGUI.ExitButton.SetActive(false);
        }
    }
}