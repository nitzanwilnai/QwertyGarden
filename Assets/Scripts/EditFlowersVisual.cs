using CommonTools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MPE;

namespace QwertyGarden
{
    public class EditFlowersVisual
    {
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

        float m_flowerCardOffset = 270.0f;
        float m_slideVelocity = 2000.0f;

        GameObject m_UI;

        KeyboardImages m_keyboardImage;

        Transform m_keyboardParent;
        Transform m_cardsParent;

        ReceiptLineGUI[] m_receiptItems;

        FlowerPopupGUI[] m_flowerPopupGUI;

        TextMeshProUGUI m_totalCostText;
        TextMeshProUGUI m_changeText;
        TextMeshProUGUI m_currentCoinText;
        Animation m_changeAnimation;

        float m_currentX;
        float m_targetX;

        int[] m_newFlowerType = new int[26];

        TutorialGUI m_tutorialGUI = new TutorialGUI();

        RectTransform m_line;
        Canvas m_canvas;

        KeyboardData keyboardData;
        MetaData metaData;
        Balance balance;

        public void Init(GameObject UI, MetaData metaData, Balance balance)
        {
            this.metaData = metaData;
            this.balance = balance;

            m_UI = UI;
            m_UI.SetActive(false);
            m_canvas = m_UI.GetComponent<Canvas>();

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_keyboardParent = guiRef.GetGameObject("Keyboard").transform;
            m_line = guiRef.GetGameObject("Line").GetComponent<RectTransform>();

            CommonVisual.InitTutorialGUI(guiRef, m_tutorialGUI);

            GUIRef receeiptGUIRef = guiRef.GetGameObject("Receipt").GetComponent<GUIRef>();
            m_totalCostText = receeiptGUIRef.GetTextGUI("Total");
            m_currentCoinText = receeiptGUIRef.GetTextGUI("Cash");
            m_changeText = receeiptGUIRef.GetTextGUI("Change");
            m_changeAnimation = receeiptGUIRef.GetAnimation("Change");

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
                m_flowerPopupGUI[flowerType].GO.SetActive(true);
                m_flowerPopupGUI[flowerType].GO.transform.localScale = Vector3.one;
                m_flowerPopupGUI[flowerType].GO.transform.localPosition = new Vector3(288.0f * flowerType, 0.0f, 0.0f);

                m_flowerPopupGUI[flowerType].SellValue.text = balance.FlowerSellValue[flowerType].ToString("N0");
                m_flowerPopupGUI[flowerType].SeedCost.text = balance.FlowerSeedCost[flowerType].ToString("N0");
                m_flowerPopupGUI[flowerType].FlowerName.text = balance.FlowerName[flowerType];
                m_flowerPopupGUI[flowerType].Flower.sprite = AssetManager.Instance.LoadFlowerIcon(balance.FlowerName[flowerType], balance.FlowerIcon[flowerType]);
            }
        }

        public void Show(KeyboardData keyboardData)
        {
            m_line.gameObject.SetActive(false);

            this.keyboardData = keyboardData;
            m_UI.SetActive(true);
            m_keyboardImage = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardData.KeyboardType], m_keyboardParent);
            m_keyboardImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            m_keyboardImage.transform.localPosition = Vector3.zero;
            m_keyboardImage.transform.SetAsFirstSibling();

            m_keyboardImage.KeyPctText = new TextMeshProUGUI[26];
            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                int flowerType = keyboardData.FlowerType[keyIdx];
                m_newFlowerType[keyIdx] = flowerType;
                m_keyboardImage.KeyImages[keyIdx].sprite = AssetManager.Instance.LoadFlowerCard(balance.FlowerName[flowerType], balance.FlowerCard[flowerType]);
                m_keyboardImage.KeyPct[keyIdx].SetActive(false);
                m_keyboardImage.KeyPctText[keyIdx] = m_keyboardImage.KeyPct[keyIdx].GetComponentInChildren<TextMeshProUGUI>();
            }

            updateReceiptItems();

            m_currentX = 0.0f;
            m_targetX = 0.0f;

            CommonVisual.CheckTutorialFlag(metaData, m_tutorialGUI);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            GameObject.Destroy(m_keyboardImage.gameObject);
        }

        double calculateChange()
        {
            Span<int> flowerCount = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < balance.NumFlowers; keyIdx++)
                flowerCount[keyIdx] = 0;

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                if (m_newFlowerType[keyIdx] != keyboardData.FlowerType[keyIdx])
                    flowerCount[m_newFlowerType[keyIdx]]++;

            double totalCost = 0;
            int itemCount = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                int itemCost = balance.FlowerSeedCost[flowerType] * flowerCount[flowerType];
                totalCost += itemCost;
                m_receiptItems[itemCount].Item.text = balance.FlowerName[flowerType] + " x " + flowerCount[flowerType];
                m_receiptItems[itemCount].Value.text = MetaLogic.ToShortScale(itemCost) + " <sprite name=\"coin\">";
                m_receiptItems[itemCount].GO.SetActive(flowerCount[flowerType] > 0);
                itemCount++;
            }
            return metaData.Coins - totalCost;
        }

        void updateReceiptItems()
        {
            for (int i = 0; i < m_receiptItems.Length; i++)
                m_receiptItems[i].GO.SetActive(false);

            Span<int> flowerCount = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < balance.NumFlowers; keyIdx++)
                flowerCount[keyIdx] = 0;

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                if (m_newFlowerType[keyIdx] != keyboardData.FlowerType[keyIdx])
                    flowerCount[m_newFlowerType[keyIdx]]++;

            double totalCost = 0;
            int itemCount = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                int itemCost = balance.FlowerSeedCost[flowerType] * flowerCount[flowerType];
                totalCost += itemCost;
                m_receiptItems[itemCount].Item.text = balance.FlowerName[flowerType] + " x " + flowerCount[flowerType];
                m_receiptItems[itemCount].Value.text = MetaLogic.ToShortScale(itemCost) + " <sprite name=\"coin\">";
                m_receiptItems[itemCount].GO.SetActive(flowerCount[flowerType] > 0);
                itemCount++;
            }
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
            if (Keyboard.current != null)
            {
                if (CommonVisual.CheckTutorialKey(m_tutorialGUI))
                {

                }
                else if (Keyboard.current.enterKey.wasReleasedThisFrame)
                {
                    if (CozyLogic.TryEditCozy(keyboardData, metaData, balance, m_newFlowerType))
                    {
                        MetaDataIO.SaveMeta(metaData);
                        KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);
                        Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
                    }
                    else
                    {
                        m_changeAnimation.Play("Grow");
                    }
                }
                else if (Keyboard.current.leftShiftKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.SETTINGS);
                }
                else if (Keyboard.current.tabKey.wasPressedThisFrame)
                {
                    int totalCharCount = 0;
                    for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                        totalCharCount += keyboardData.CharacterCount[keyIdx];

                    for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                    {
                        float pct = Mathf.FloorToInt((float)keyboardData.CharacterCount[keyIdx] / (float)totalCharCount * 100.0f);
                        m_keyboardImage.KeyPctText[keyIdx].text = (char)(keyIdx+65) + "\n" + pct.ToString("N0") + "%";
                        m_keyboardImage.KeyPct[keyIdx].SetActive(true);
                    }
                }
                else if (Keyboard.current.tabKey.wasReleasedThisFrame)
                {
                    for (int keyIdx = 0; keyIdx < 26; keyIdx++)
                    {
                        m_keyboardImage.KeyPct[keyIdx].SetActive(false);
                    }
                }


                double change = calculateChange();
                char c;
                int keyIndex = KeyboardLogic.GetTypedKeyIndex(out c);
                if (keyIndex > -1)
                {
                    int flowerType = m_newFlowerType[keyIndex];
                    int newFlowerType = (flowerType + 1) % balance.NumFlowers;
                    m_newFlowerType[keyIndex] = newFlowerType;
                    if (change < balance.FlowerSeedCost[newFlowerType])
                    {
                        newFlowerType = 0;
                        m_changeAnimation.Play("Grow");
                    }

                    m_newFlowerType[keyIndex] = newFlowerType;

                    updateReceiptItems();
                    for (int i = 0; i < balance.NumFlowers; i++)
                    {
                        m_flowerPopupGUI[i].Outline.color = change >= balance.FlowerSeedCost[i] ? AssetManager.Instance.FlowerPopupGreen : AssetManager.Instance.FlowerPopupRed;
                    }

                    m_keyboardImage.KeyImages[keyIndex].sprite = AssetManager.Instance.LoadFlowerCard(balance.FlowerName[newFlowerType], balance.FlowerCard[newFlowerType]);

                    m_cardsParent.transform.localPosition = new Vector3(-288.0f * newFlowerType, -192.0f, 0.0f);

                    Vector3 cardPosition = m_flowerPopupGUI[newFlowerType].GO.transform.position;
                    cardPosition.y -= m_flowerPopupGUI[newFlowerType].GO.GetComponent<RectTransform>().rect.height / 2.0f;
                    SetLine(newFlowerType, m_line, WorldToCanvas(m_canvas.GetComponent<RectTransform>(), cardPosition), WorldToCanvas(m_canvas.GetComponent<RectTransform>(), m_keyboardImage.KeyImages[keyIndex].transform.position));
                }

            }

            void SetLine(int flowerType, RectTransform line, Vector2 start, Vector2 end)
            {
                m_flowerPopupGUI[flowerType].GO.SetActive(true);

                Vector2 dir = end - start;
                line.sizeDelta = new Vector2(dir.magnitude, line.sizeDelta.y);
                line.anchoredPosition = start + dir * 0.5f;
                line.localRotation = Quaternion.Euler(0, 0,
                    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

                m_line.gameObject.SetActive(true);
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
    }
}