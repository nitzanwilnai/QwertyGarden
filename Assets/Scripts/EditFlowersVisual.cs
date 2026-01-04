using CommonTools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using TMPro;
using Unity.VisualScripting;

namespace QwertyGarden
{
    public class EditFlowersVisual
    {

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

        GameObject[] m_flowerCards;
        Image[] m_flowerCardOutlines;
        ReceiptLineGUI[] m_receiptItems;

        Transform m_flowerCardParent;

        TextMeshProUGUI m_totalCostText;
        TextMeshProUGUI m_changeText;
        TextMeshProUGUI m_currentCoinText;
        Animation m_changeAnimation;

        float m_currentX;
        float m_targetX;
        int m_flowerType = 0;

        int[] newFlowerType = new int[26];

        KeyboardData keyboardData;
        MetaData metaData;
        Balance balance;

        public void Init(GameObject UI, MetaData metaData, Balance balance)
        {
            this.metaData = metaData;
            this.balance = balance;

            m_UI = UI;
            m_UI.SetActive(false);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_keyboardParent = guiRef.GetGameObject("Keyboard").transform;

            GUIRef receeiptGUIRef = guiRef.GetGameObject("Receipt").GetComponent<GUIRef>();
            m_totalCostText = receeiptGUIRef.GetTextGUI("Total");
            m_currentCoinText = receeiptGUIRef.GetTextGUI("Cash");
            m_changeText = receeiptGUIRef.GetTextGUI("Change");
            m_changeAnimation = receeiptGUIRef.GetAnimation("Change");

            int numFlowers = balance.NumFlowers;
            m_flowerCardParent = guiRef.GetGameObject("CardsParent").transform;
            Transform flowerReceipt = receeiptGUIRef.GetGameObject("ReceiptItems").transform;
            m_flowerCards = new GameObject[numFlowers];
            m_flowerCardOutlines = new Image[numFlowers];
            m_receiptItems = new ReceiptLineGUI[numFlowers];
            for (int flowerType = 0; flowerType < numFlowers; flowerType++)
            {
                GameObject flowerCard = GameObject.Instantiate(AssetManager.Instance.ShopCardPrefab, m_flowerCardParent);
                CommonFlowerVisual.AddFlowerCard(balance, flowerType, flowerCard, m_flowerCardOutlines);
                m_flowerCards[flowerType] = flowerCard;

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
        }

        public void Show(KeyboardData keyboardData)
        {
            this.keyboardData = keyboardData;
            m_UI.SetActive(true);
            m_keyboardImage = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardData.KeyboardType], m_keyboardParent);
            m_keyboardImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            m_keyboardImage.transform.localPosition = Vector3.zero;
            m_keyboardImage.transform.SetAsFirstSibling();

            for (int keyIndex = 0; keyIndex < 26; keyIndex++)
            {
                int flowerType = keyboardData.FlowerType[keyIndex];
                newFlowerType[keyIndex] = flowerType;
                m_keyboardImage.KeyImages[keyIndex].sprite = AssetManager.Instance.LoadFlowerCard(balance.FlowerName[flowerType], balance.FlowerCard[flowerType]);
            }

            updateReceiptItems();

            m_flowerType = 0;
            m_currentX = 0.0f;
            m_targetX = 0.0f;
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            GameObject.Destroy(m_keyboardImage.gameObject);
        }

        void updateReceiptItems()
        {
            for (int i = 0; i < m_receiptItems.Length; i++)
                m_receiptItems[i].GO.SetActive(false);

            Span<int> flowerCount = stackalloc int[balance.NumFlowers];
            for (int keyIdx = 0; keyIdx < balance.NumFlowers; keyIdx++)
                flowerCount[keyIdx] = 0;

            for (int keyIdx = 0; keyIdx < 26; keyIdx++)
            {
                if (newFlowerType[keyIdx] != keyboardData.FlowerType[keyIdx])
                {
                    int flowerType = newFlowerType[keyIdx];
                    flowerCount[flowerType]++;
                }
            }

            decimal totalCost = 0;
            int itemCount = 0;
            for (int flowerType = 0; flowerType < balance.NumFlowers; flowerType++)
            {
                int itemCost = balance.FlowerSeedCost[flowerType] * flowerCount[flowerType];
                totalCost += itemCost;
                m_receiptItems[itemCount].Item.text = balance.FlowerName[flowerType] + " x " + flowerCount[flowerType];
                m_receiptItems[itemCount].Value.text = itemCost.ToString() + " <sprite name=\"coin\">";
                m_receiptItems[itemCount].GO.SetActive(flowerCount[flowerType] > 0);
                itemCount++;
            }

            m_totalCostText.text = totalCost.ToString("N0") + " <sprite name=\"coin\">";
            m_currentCoinText.text = metaData.Coins.ToString("N0") + " <sprite name=\"coin\">";
            decimal change = metaData.Coins - totalCost;
            string symbol = change < 0 ? "-" : "";
            decimal changeAbs = change < 0 ? -change : change;
            m_changeText.text = symbol + changeAbs.ToString("N0") + " <sprite name=\"coin\">";
            m_changeText.color = change >= 0 ? AssetManager.Instance.ReceiptChangePositive : AssetManager.Instance.ReceiptChangeNegative;
        }

        public void Tick(float dt)
        {
            CommonFlowerVisual.PositionKeyboardForResolution(m_keyboardImage);

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
            m_flowerCardParent.transform.localPosition = new Vector3(m_currentX, 0.0f, 0.0f);

            if (Keyboard.current != null)
            {
                if (Keyboard.current.enterKey.wasReleasedThisFrame)
                {
                    if (CozyLogic.TryEditCozy(keyboardData, metaData, balance, newFlowerType))
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
                if (Keyboard.current.escapeKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(MENU_STATE.GARDEN_SELECTION);
                }
                if (Keyboard.current.leftArrowKey.wasReleasedThisFrame)
                {
                    if (m_flowerType > 0)
                    {
                        m_flowerType--;
                        m_targetX = m_flowerType * -m_flowerCardOffset;
                        CommonFlowerVisual.UpdateCardOutline(m_flowerCardOutlines, m_flowerType);
                    }
                }
                if (Keyboard.current.rightArrowKey.wasReleasedThisFrame)
                {
                    if (m_flowerType < balance.NumFlowers - 1)
                    {
                        m_flowerType++;
                        m_targetX = m_flowerType * -m_flowerCardOffset;
                        CommonFlowerVisual.UpdateCardOutline(m_flowerCardOutlines, m_flowerType);
                    }
                }

                char c;
                int keyIndex = KeyboardLogic.GetTypedKeyIndex(out c);
                if (keyIndex > -1)
                {
                    newFlowerType[keyIndex] = m_flowerType;
                    m_keyboardImage.KeyImages[keyIndex].sprite = AssetManager.Instance.LoadFlowerCard(balance.FlowerName[m_flowerType], balance.FlowerCard[m_flowerType]);

                    updateReceiptItems();
                }
            }
        }
    }
}