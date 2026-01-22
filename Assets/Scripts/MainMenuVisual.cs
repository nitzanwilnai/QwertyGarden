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
        TextMeshProUGUI m_settingsText;
        TextMeshProUGUI m_prestigeTotal;
        TextMeshProUGUI m_versionText;
        GameObject m_spaceGO;

        TutorialGUI m_tutorialGUI = new TutorialGUI();

        GameObject m_prestigeGO;
        TextMeshProUGUI m_poststigeText;

        GameObject m_wishlistGO;

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
            m_poststigeText = guiRef.GetTextGUI("Prestige");
            m_settingsText = guiRef.GetTextGUI("Settings");
            m_spaceGO = guiRef.GetGameObject("Space");
            m_prestigeTotal = guiRef.GetTextGUI("PrestigeTotal");
            m_wishlistGO = guiRef.GetGameObject("Wishlist");
            m_versionText = guiRef.GetTextGUI("Version");

            GameObject topBarGO = guiRef.GetGameObject("TopBar");
            m_coinsText = topBarGO.GetComponent<GUIRef>().GetTextGUI("Coins");
        }

        public void Show()
        {
            CommonVisual.AutoCanvasScaler(m_UI);

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
                m_keyboardImages.KeyPct[keyIndex].SetActive(false);
            }

            m_coinsText.text = MetaLogic.ToShortScale(metaData.Coins) + " <sprite name=\"coin\">";
            // m_coinsText.text = Decimal.MaxValue.ToString("N0");

            m_poststigeText.text = "<color=#C0392B><b>backspace</b> prestige! " + MetaLogic.ToShortScale(MetaLogic.GetPrestigeCost(metaData, balance)) + " </color><sprite name=coin>";

            m_prestigeTotal.text = metaData.Prestige > 0 ? ("prestige +" + metaData.Prestige) : "";

            m_spaceGO.SetActive(metaData.Coins > 0);

            MetaLogic.TryShowPrestige(metaData);
            m_prestigeGO.SetActive(metaData.ShowPrestige);

            CommonVisual.CheckTutorialFlag(metaData, m_tutorialGUI);

            m_wishlistGO.SetActive(false);
#if DEMO
            m_wishlistGO.SetActive(true);
#endif            

            TextAsset versionText = (TextAsset)Resources.Load("Version");
            m_versionText.text = "VERSION: " + versionText.text.ToUpper();

            // string fileName = Application.persistentDataPath + "/";
            // m_versionText.text = fileName;
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            if (m_keyboardImages != null && m_keyboardImages.gameObject != null)
                GameObject.Destroy(m_keyboardImages.gameObject);
        }

        public void Tick()
        {
            if (Keyboard.current != null)
            {
                if (CommonVisual.CheckTutorialKey(m_tutorialGUI))
                {

                }

                if (Keyboard.current.enterKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
                    SoundManager.Instance.PlaySFXKeyClick();
                }
                else if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.EDIT_GARDEN);
                    SoundManager.Instance.PlaySFXKeyClick();
                }
#if DEMO
                else if (Keyboard.current.wKey.wasReleasedThisFrame)
                {
                    Application.OpenURL("steam://openurl/https://store.steampowered.com/app/4255650/QwertyGarden/");
                }
#endif
                else if (Keyboard.current.digit1Key.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.SETTINGS);
                    // metaData.SFX = !metaData.SFX;
                    // CommonVisual.UpdateSettingsText(metaData, m_settingsText);
                }
                // else if (Keyboard.current.digit2Key.wasReleasedThisFrame)
                // {
                //     metaData.Music = !metaData.Music;
                //     CommonVisual.UpdateSettingsText(metaData, m_settingsText);
                // }
                else if (Keyboard.current.qKey.wasReleasedThisFrame)
                {
                    Application.Quit();
                }
                else if (metaData.ShowPrestige && Keyboard.current.backspaceKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.PRESTIGE);
                }
            }
        }
    }
}