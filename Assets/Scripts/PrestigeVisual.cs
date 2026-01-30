using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace QwertyGarden
{
    public class PrestigeVisual
    {
        GameObject m_UI;

        TextMeshProUGUI m_prestigeCostText;
        TextMeshProUGUI m_coinsText;
        TextMeshProUGUI m_totalText;

        Button m_prestigeButton;
        Image m_prestigeButtonImage;

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
            m_prestigeCostText = guiRef.GetTextGUI("PrestigeCost");
            m_coinsText = guiRef.GetTextGUI("Coins");
            m_totalText = guiRef.GetTextGUI("Total");

            guiRef.GetButton("Exit").onClick.AddListener(closePrestigePopup);
            guiRef.GetButton("Cancel").onClick.AddListener(closePrestigePopup);
            m_prestigeButton = guiRef.GetButton("Prestige");
            m_prestigeButton.onClick.AddListener(doPrestige);

            m_prestigeButtonImage = guiRef.GetImage("Prestige");

            Hide();
        }

        public void Show()
        {
            m_UI.SetActive(true);

            double prestigeCost = MetaLogic.GetPrestigeCost(metaData, balance);
            m_prestigeCostText.text = MetaLogic.ToShortScale(prestigeCost) + " <sprite name=coin>";

            m_totalText.text = "Total: <b>+" + (metaData.Prestige + 1).ToString("N0") + "</b> flowers";

            string s = "<color=#";
            s += (prestigeCost <= metaData.Coins) ? ColorUtility.ToHtmlStringRGBA(AssetManager.Instance.ReceiptChangePositive) : ColorUtility.ToHtmlStringRGBA(AssetManager.Instance.ReceiptChangeNegative);
            s += ">";
            m_coinsText.text = s + MetaLogic.ToShortScale(metaData.Coins) + "</color> <sprite name=coin>";

            m_prestigeButton.enabled = prestigeCost <= metaData.Coins;
            m_prestigeButtonImage.color = prestigeCost <= metaData.Coins ? AssetManager.Instance.PrestigeEnabled : AssetManager.Instance.PrestigeDisabled;
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick()
        {
        }

        void closePrestigePopup()
        {
            Game.Instance.SetMenuState(metaData.PrevMenuState);
        }

        void doPrestige()
        {
            if (MetaLogic.TryPrestige(metaData, keyboardData, balance))
            {
                Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
            }
        }
    }
}