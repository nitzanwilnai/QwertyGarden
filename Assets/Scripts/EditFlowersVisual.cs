using CommonTools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;
using System.Collections;

namespace QwertyGarden
{
    public class EditFlowersVisual
    {
        readonly int MAX_DEMO_FLOWERS = 4;

        public class FlowerPopupGUI
        {
            public GameObject GO;
            public TextMeshProUGUI FlowerName;
            public TextMeshProUGUI SeedCost;
            public TextMeshProUGUI SellValue;
            public Image Outline;
            public Image Flower;
        }

        public class ReceiptLineGUI
        {
            public GameObject GO;
            public TextMeshProUGUI Item;
            public TextMeshProUGUI Value;
        }

        float m_slideVelocity = 2000.0f;

        GameObject m_UI;

        KeyboardImages m_keyboardImages;

        Transform m_keyboardParent;
        Transform m_cardsParent;
        float m_cardsParentTargetX;

        ReceiptLineGUI[] m_receiptItems;

        FlowerPopupGUI[] m_flowerPopupGUI;

        TextMeshProUGUI m_totalCostText;
        TextMeshProUGUI m_changeText;
        TextMeshProUGUI m_currentCoinText;
        Animation m_changeAnimation;

        GameObject m_selectKeyGO;

        float m_currentX;
        float m_targetX;
        float[] m_currentScale;
        float[] m_targetScale;
        int m_keyIdx;
        int m_flowerType;

        NavButtonGUI m_navButtonGUI = new NavButtonGUI();
        GameObject m_tutorialGO;
        GameObject m_darken;

        public bool m_showSettings = false;

        Canvas m_canvas;

        KeyboardData keyboardData;
        MetaData metaData;
        Balance balance;

        public void Init(GameObject UI, MetaData metaData, KeyboardData keyboardData, Balance balance)
        {
            this.metaData = metaData;
            this.balance = balance;
            this.keyboardData = keyboardData;

            m_UI = UI;
            m_UI.SetActive(false);
            m_canvas = m_UI.GetComponent<Canvas>();

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_keyboardParent = guiRef.GetGameObject("Keyboard").transform;
            m_darken = guiRef.GetGameObject("Darken");

            GUIRef receeiptGUIRef = guiRef.GetGameObject("Receipt").GetComponent<GUIRef>();
            m_totalCostText = receeiptGUIRef.GetTextGUI("Total");
            m_currentCoinText = receeiptGUIRef.GetTextGUI("Cash");
            m_changeText = receeiptGUIRef.GetTextGUI("Change");
            m_changeAnimation = receeiptGUIRef.GetAnimation("Change");
            receeiptGUIRef.GetButton("Play").onClick.AddListener(Game.Instance.BuySelectedSeedsAndPlay);

            m_selectKeyGO = guiRef.GetGameObject("Select");

            int numFlowers = balance.NumFlowers;
            Transform flowerReceipt = receeiptGUIRef.GetGameObject("ReceiptItems").transform;
            m_receiptItems = new ReceiptLineGUI[numFlowers];
            for (int flowerType = 0; flowerType < numFlowers; flowerType++)
            {
                ReceiptLineGUI receiptLine = new ReceiptLineGUI();
                GameObject receiptLineGO = GameObject.Instantiate(AssetManager.Instance.ShopReceiptItem, flowerReceipt);
                GUIRef receiptGUIRef = receiptLineGO.GetComponent<GUIRef>();
                receiptLine.GO = receiptLineGO;
                receiptLine.Item = receiptGUIRef.GetTextGUI("Name");
                receiptLine.Value = receiptGUIRef.GetTextGUI("Value");
                receiptLine.GO.transform.SetSiblingIndex(7 + flowerType);
                m_receiptItems[flowerType] = receiptLine;
                receiptLine.GO.SetActive(false);
            }

            m_cardsParent = guiRef.GetGameObject("Cards").transform;
            m_flowerPopupGUI = new FlowerPopupGUI[balance.NumFlowers];
            for (int flowerType = 0; flowerType < numFlowers; flowerType++)
            {
                m_flowerPopupGUI[flowerType] = new FlowerPopupGUI();
                m_flowerPopupGUI[flowerType].GO = GameObject.Instantiate(AssetManager.Instance.UIFlowerPopup, m_cardsParent);
                GUIRef popupGUIRef = m_flowerPopupGUI[flowerType].GO.GetComponent<GUIRef>();
                m_flowerPopupGUI[flowerType].SellValue = popupGUIRef.GetTextGUI("Sell");
                m_flowerPopupGUI[flowerType].SeedCost = popupGUIRef.GetTextGUI("Cost");
                m_flowerPopupGUI[flowerType].FlowerName = popupGUIRef.GetTextGUI("FlowerName");
                m_flowerPopupGUI[flowerType].Flower = popupGUIRef.GetImage("Flower");
                m_flowerPopupGUI[flowerType].Outline = popupGUIRef.GetImage("Outline");
                m_flowerPopupGUI[flowerType].GO.SetActive(false);
                m_flowerPopupGUI[flowerType].GO.transform.localScale = Vector3.one;
                m_flowerPopupGUI[flowerType].GO.transform.localPosition = new Vector3(288.0f * flowerType, 0.0f, 0.0f);

                m_flowerPopupGUI[flowerType].SellValue.text = MetaLogic.ToShortScale(balance.FlowerSellValue[flowerType]);
                m_flowerPopupGUI[flowerType].SeedCost.text = MetaLogic.ToShortScale(balance.FlowerSeedCost[flowerType]);
                m_flowerPopupGUI[flowerType].FlowerName.text = balance.FlowerName[flowerType];
                m_flowerPopupGUI[flowerType].Flower.sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[flowerType], balance.FlowerIcon[flowerType]);

                int localFlowerType = flowerType;
                popupGUIRef.GetButton("Button").onClick.AddListener(() => { selectFlower(localFlowerType); });
            }

            CommonVisual.InitButtons(guiRef, m_navButtonGUI);
            m_tutorialGO = guiRef.GetGameObject("Tutorial");
            m_tutorialGO.SetActive(false);
            guiRef.GetButton("TutorialExit").onClick.AddListener(hideTutorial);

            m_currentScale = new float[balance.NumFlowers];
            m_targetScale = new float[balance.NumFlowers];
        }

        public void Show(KeyboardData keyboardData)
        {
            CommonVisual.AutoCanvasScaler(m_UI);

            this.keyboardData = keyboardData;
            m_UI.SetActive(true);
            m_keyboardImages = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardData.KeyboardType], m_keyboardParent);
            m_keyboardImages.transform.localScale = new Vector3(1.35f, 1.35f, 1.0f);
            m_keyboardImages.transform.localPosition = Vector3.zero;
            m_keyboardImages.transform.SetAsFirstSibling();

            int totalCharCount = 0;
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                totalCharCount += keyboardData.CharacterCount[keyIdx];

            m_keyboardImages.KeyFlowerIcon = new Image[26];
            m_keyboardImages.KeyPctText = new TextMeshProUGUI[26];
            m_keyboardImages.KeyButtonImages = new Image[26];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                GUIRef keyGUIRef = m_keyboardImages.KeysGO[keyIdx].GetComponent<GUIRef>();
                m_keyboardImages.KeyFlowerIcon[keyIdx] = keyGUIRef.GetImage("Flower");
                m_keyboardImages.KeyPctText[keyIdx] = keyGUIRef.GetTextGUI("Text");
                m_keyboardImages.KeyButtonImages[keyIdx] = keyGUIRef.GetImage("Button");

                int flowerType = keyboardData.NewFlowerType[keyIdx];
                m_keyboardImages.KeyFlowerIcon[keyIdx].sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[flowerType], balance.FlowerIcon[flowerType]);

                float pct = Mathf.FloorToInt((float)keyboardData.CharacterCount[keyIdx] / (float)totalCharCount * 100.0f);
                m_keyboardImages.KeyPctText[keyIdx].text = (char)(keyIdx + 65) + " " + pct.ToString("N0") + "%";

                int localIdx = keyIdx;
                keyGUIRef.GetButton("Button").onClick.AddListener(() => { selectKey(localIdx); });
            }

            m_keyboardImages.MoneyText.text = MetaLogic.ToShortScale(metaData.Coins);

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
                m_flowerPopupGUI[flowerType].GO.SetActive(false);

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                m_currentScale[flowerType] = 1.0f;
                m_targetScale[flowerType] = 1.0f;
            }

            updateReceiptItems();

            m_navButtonGUI.PlayButton.SetActive(true);
            m_navButtonGUI.GardenButton.SetActive(false);

            MetaLogic.TryShowPrestige(metaData);
            m_navButtonGUI.PrestigeButton.SetActive(metaData.ShowPrestige);

            CommonVisual.HideSettings(m_navButtonGUI);

            m_showSettings = false;

            m_currentX = 0.0f;
            m_targetX = 0.0f;

            m_tutorialGO.SetActive(CommonVisual.AutoShowTutorial(metaData));
            m_darken.SetActive(CommonVisual.AutoShowTutorial(metaData));

            m_selectKeyGO.SetActive(true);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            GameObject.Destroy(m_keyboardImages.gameObject);
        }

        void updateReceiptItems()
        {
            for (int i = 0; i < m_receiptItems.Length; i++)
                m_receiptItems[i].GO.SetActive(false);

            Span<int> flowerCount = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < balance.NumFlowers; keyIdx++)
                flowerCount[keyIdx] = 0;

            Span<int> flowerCost = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                int newFlowerType = keyboardData.NewFlowerType[keyIdx];
                int oldFlowerType = keyboardData.FlowerType[keyIdx];
                if (newFlowerType != oldFlowerType)
                {
                    flowerCost[newFlowerType] += balance.FlowerSeedCost[newFlowerType];
                    flowerCount[newFlowerType]++;
                }
            }

            int itemCount = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                int itemCost = flowerCost[flowerType];
                m_receiptItems[itemCount].Item.text = balance.FlowerName[flowerType] + " x " + flowerCount[flowerType];
                m_receiptItems[itemCount].Value.text = MetaLogic.ToShortScale(itemCost) + " <sprite name=\"coin\">";
                m_receiptItems[itemCount].GO.SetActive(flowerCount[flowerType] > 0);
                itemCount++;
            }

            double totalCost = CozyLogic.CalculateCurrentGardenCost(keyboardData, balance);
            double change = metaData.Coins - totalCost;

            m_totalCostText.text = MetaLogic.ToShortScale(totalCost) + " <sprite name=\"coin\">";
            m_currentCoinText.text = MetaLogic.ToShortScale(metaData.Coins) + " <sprite name=\"coin\">";
            string symbol = change < 0 ? "-" : "";
            double changeAbs = change < 0 ? -change : change;
            m_changeText.text = symbol + MetaLogic.ToShortScale(changeAbs) + " <sprite name=\"coin\">";
            m_changeText.color = change >= 0 ? AssetManager.Instance.ReceiptChangePositive : AssetManager.Instance.ReceiptChangeNegative;
        }

        public void Tick(float dt)
        {
            if (m_currentX < m_targetX)
            {
                m_currentX += dt * m_slideVelocity;
                if (m_currentX >= m_targetX)
                    m_currentX = m_targetX;
            }
            if (m_currentX > m_targetX)
            {
                m_currentX -= dt * m_slideVelocity;
                if (m_currentX <= m_targetX)
                    m_currentX = m_targetX;
            }

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                if (m_currentScale[flowerType] < m_targetScale[flowerType])
                {
                    m_currentScale[flowerType] += dt;
                    if (m_currentScale[flowerType] > m_targetScale[flowerType])
                        m_currentScale[flowerType] = m_targetScale[flowerType];
                }
                if (m_currentScale[flowerType] > m_targetScale[flowerType])
                {
                    m_currentScale[flowerType] -= dt;
                    if (m_currentScale[flowerType] < m_targetScale[flowerType])
                        m_currentScale[flowerType] = m_targetScale[flowerType];
                }
                m_flowerPopupGUI[flowerType].GO.transform.localScale = new Vector3(m_currentScale[flowerType], m_currentScale[flowerType], 1.0f);
            }

            // if (moveCards)
            m_cardsParent.transform.localPosition = new Vector3(m_currentX, -192.0f, 0.0f);


            void SetLine(RectTransform line, Vector2 start, Vector2 end)
            {
                Vector2 dir = end - start;
                line.sizeDelta = new Vector2(dir.magnitude, line.sizeDelta.y);
                line.anchoredPosition = start + dir * 0.5f;
                line.localRotation = Quaternion.Euler(0, 0,
                    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            }

            Vector2 WorldToCanvas(RectTransform canvas, Vector3 worldPos)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas,
                    RectTransformUtility.WorldToScreenPoint(null, worldPos),
                    null,
                    out Vector2 localPos
                );
                return localPos;
            }
        }

        public void ToggleShowSettings()
        {
            if (!m_showSettings)
            {
                m_showSettings = true;
                CommonVisual.ShowSettings(m_navButtonGUI, metaData);
            }
            else
            {
                m_showSettings = false;
                CommonVisual.HideSettings(m_navButtonGUI);
            }
        }

        public void ShowTutorial()
        {
            m_tutorialGO.SetActive(true);
            m_darken.SetActive(true);
        }

        void hideTutorial()
        {
            m_tutorialGO.SetActive(false);
            m_darken.SetActive(false);
        }

        public void BuySelectedSeedsAndPlay()
        {
            if (CozyLogic.TryEditCozy(keyboardData, metaData, balance))
            {
                SoundManager.Instance.PlaySFXMoney();
                MetaDataIO.SaveMeta(metaData);
                KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);
                Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
            }
        }

        void selectFlower(int newFlowerType)
        {
            double currentGardenCost = CozyLogic.CalculateCurrentGardenCost(keyboardData, balance);
            double moneyLeftOver = metaData.Coins - currentGardenCost;

            int oldFlowerType = keyboardData.FlowerType[m_keyIdx];

#if DEMO
            if (newFlowerType >= MAX_DEMO_FLOWERS)
                return;
#endif

            if (balance.FlowerSeedCost[newFlowerType] <= moneyLeftOver || oldFlowerType == newFlowerType)
            {
                m_keyboardImages.KeyFlowerIcon[m_keyIdx].sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[newFlowerType], balance.FlowerIcon[newFlowerType]);

                keyboardData.NewFlowerType[m_keyIdx] = newFlowerType;

                selectFlowerCommon(oldFlowerType, newFlowerType, moneyLeftOver);
            }
        }

        void selectFlowerCommon(int oldFlowerType, int newFlowerType, double moneyLeftOver)
        {
            SoundManager.Instance.PlaySFXKeyClick();

            updateReceiptItems();

            m_currentX = m_cardsParent.transform.localPosition.x;
            m_targetX = -288.0f * newFlowerType;
            m_flowerType = newFlowerType;

            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                if (!m_flowerPopupGUI[flowerType].GO.activeSelf)
                    m_flowerPopupGUI[flowerType].GO.SetActive(true);

                bool flowerChangeOK = (balance.FlowerSeedCost[flowerType] <= moneyLeftOver) || flowerType == oldFlowerType;

#if DEMO
                if (flowerType >= MAX_DEMO_FLOWERS)
                    flowerChangeOK = false;
#endif

                m_flowerPopupGUI[flowerType].Outline.color = flowerChangeOK ? AssetManager.Instance.FlowerPopupGreen : AssetManager.Instance.FlowerPopupRed;

                m_targetScale[flowerType] = flowerType == newFlowerType ? 1.0f : 0.8f;
            }

            KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);
        }

        void selectKey(int keyIndex)
        {
            m_selectKeyGO.SetActive(false);

            m_keyIdx = keyIndex;

            Span<int> tempGardenFlowers = stackalloc int[26];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                tempGardenFlowers[keyIdx] = keyboardData.NewFlowerType[keyIdx];
            double currentGardenCost = CozyLogic.CalculateGardenCost(keyboardData, balance, tempGardenFlowers);
            double moneyLeftOver = metaData.Coins - currentGardenCost;

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                m_keyboardImages.KeyButtonImages[keyIdx].color = m_keyIdx == keyIdx ? AssetManager.Instance.KeySelected : AssetManager.Instance.KeyNotSelected;

            selectFlowerCommon(keyboardData.FlowerType[keyIndex], keyboardData.FlowerType[keyIndex], moneyLeftOver);
        }
    }
}