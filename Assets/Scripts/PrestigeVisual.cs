using CommonTools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public class PrestigeVisual
    {
        GameObject m_UI;

        TutorialGUI m_tutorialGUI = new TutorialGUI();

        MetaData metaData;
        KeyboardData keyboardData;
        Balance balance;

        public void Init(GameObject UI, MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            this.metaData = metaData;
            this.keyboardData = keyboardData;
            this.balance = balance;

            m_UI = UI;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            CommonVisual.InitTutorialGUI(guiRef, m_tutorialGUI);

            Hide();
        }

        public void Show()
        {
            m_UI.SetActive(true);

            CommonVisual.CheckTutorialFlag(metaData, m_tutorialGUI);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick()
        {
            if (Keyboard.current != null)
            {
                if (CommonVisual.CheckTutorialKey(m_tutorialGUI))
                {

                }
                else if (!m_tutorialGUI.TutorialShown)
                {
                    if (Keyboard.current.mKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
                    }
                    else if (Keyboard.current.backspaceKey.wasReleasedThisFrame)
                    {
                        
                        Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
                    }
                }
            }
        }
    }
}