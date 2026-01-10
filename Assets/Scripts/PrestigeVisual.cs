using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public class PrestigeVisual
    {
        GameObject m_UI;

        TutorialGUI m_tutorialGUI = new TutorialGUI();

        Animation m_prestigeCostAnim;
        TextMeshProUGUI m_prestigeCostText;
        TextMeshProUGUI m_coinsText;
        TextMeshProUGUI m_totalText;

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
            m_prestigeCostText = guiRef.GetTextGUI("PrestigeCost");
            m_coinsText = guiRef.GetTextGUI("Coins");
            m_totalText = guiRef.GetTextGUI("Total");
            m_prestigeCostAnim = guiRef.GetAnimation("PrestigeCost");

            Hide();
        }

        public void Show()
        {
            m_UI.SetActive(true);

            CommonVisual.CheckTutorialFlag(metaData, m_tutorialGUI);

            double prestigeCost = MetaLogic.GetPrestigeCost(metaData, balance);
            m_prestigeCostText.text = "cost: " + MetaLogic.ToShortScale(prestigeCost) + " <sprite name=coin>";

            m_totalText.text = "Total: <b>+" + (metaData.Prestige+1).ToString("N0") + "</b> flowers";

            m_coinsText.text = MetaLogic.ToShortScale(metaData.Coins) + " <sprite name=coin>";
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

                if (Keyboard.current.mKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
                }
                else if (Keyboard.current.backspaceKey.wasReleasedThisFrame)
                {
                    if (MetaLogic.TryPrestige(metaData, keyboardData, balance))
                    {
                        Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
                    }
                    else
                    {
                        m_prestigeCostAnim.Play("Grow");
                    }
                }
            }
        }
    }
}