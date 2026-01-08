using CommonTools;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

namespace QwertyGarden
{
    public class MainMenuVisual
    {
        GameObject m_UI;

        Transform m_keyboardParent;
        KeyboardImages m_keyboardImages;

        TextMeshProUGUI m_coinsText;

        TutorialGUI m_tutorialGUI = new TutorialGUI();

        GameObject m_prestigeGO;

        MetaData metaData;
        KeyboardData keyboardData;
        Balance balance;

        public void Init(GameObject UI, MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            this.metaData = metaData;
            this.keyboardData = keyboardData;
            this.balance = balance;

            m_UI = UI;
            m_UI.SetActive(false);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_keyboardParent = guiRef.GetGameObject("KeyboardParent").transform;
            CommonVisual.InitTutorialGUI(guiRef, m_tutorialGUI);
            m_prestigeGO = guiRef.GetGameObject("Prestige");

            GameObject topBarGO = guiRef.GetGameObject("TopBar");
            m_coinsText = topBarGO.GetComponent<GUIRef>().GetTextGUI("Coins");
        }

        public void Show()
        {
            m_UI.SetActive(true);

            m_keyboardImages = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardData.KeyboardType], m_keyboardParent);
            m_keyboardImages.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            m_keyboardImages.transform.localPosition = new Vector3(0.0f, 384.0f, 0.0f);
            m_keyboardImages.transform.SetAsFirstSibling();

            for (int keyIndex = 0; keyIndex < 26; keyIndex++)
            {
                int flowerType = keyboardData.FlowerType[keyIndex];
                int progress = keyboardData.FlowerProgress[keyIndex];
                m_keyboardImages.KeyImages[keyIndex].sprite = AssetManager.Instance.LoadFlowerProgress(balance.FlowerName[flowerType], balance.FlowerFrames[flowerType][progress]);
                m_keyboardImages.KeyPct[keyIndex].SetActive(false);
            }

            m_coinsText.text = MetaLogic.ToShortScale(metaData.Coins) + " <sprite name=\"coin\">";
            // m_coinsText.text = Decimal.MaxValue.ToString("N0");

            m_prestigeGO.SetActive(metaData.ShowPrestige);

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
                    if (Keyboard.current.enterKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
                    }
                    else if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.EDIT_GARDEN);
                    }
                    else if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.SETTINGS);
                    }
                    else if (metaData.ShowPrestige && Keyboard.current.backspaceKey.wasReleasedThisFrame)
                    {
                        Game.Instance.SetMenuState(MENU_STATE.PRESTIGE);
                    }
                }
            }
        }
    }
}