using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
        }

        public static void HideTutorial(TutorialGUI tutorialGUI)
        {
            tutorialGUI.Bubble.SetActive(false);
            tutorialGUI.Instruction.SetActive(true);
            tutorialGUI.Darken.SetActive(false);
        }

        public static void CheckTutorialFlag(MetaData metaData, TutorialGUI tutorialGUI)
        {
            if (!MetaLogic.IsFlagSet(metaData.TutorialFlags, (int)metaData.MenuState))
            {
                ShowTutorial(tutorialGUI);
                tutorialGUI.TutorialShown = true;
                MetaLogic.SetFlag(ref metaData.TutorialFlags, (int)metaData.MenuState);
                MetaDataIO.SaveMeta(metaData);
            }
            else
                HideTutorial(tutorialGUI);
        }

        public static bool CheckTutorialKey(TutorialGUI tutorialGUI)
        {
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                if (!tutorialGUI.TutorialShown)
                {
                    tutorialGUI.TutorialShown = true;
                    ShowTutorial(tutorialGUI);
                    return true;
                }
                else
                {
                    tutorialGUI.TutorialShown = false;
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
    }
}