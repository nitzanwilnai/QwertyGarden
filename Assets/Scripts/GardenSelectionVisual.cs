using CommonTools;
using QwertyGarden;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GardenSelectionVisual
{
    float m_keyboardOffset = 1088.0f;
    float m_slideVelocity = 10000.0f;
    GameObject m_UI;

    Transform m_keyboardParent;

    KeyboardImages[] m_keyboardImages;
    KeyboardData[] m_keyboardDatas;
    GameObject[] m_keyboardSelectionBox;
    GameObject m_newKeyboardGO;
    int m_keyboardCount;
    int m_keyboardIndex = 0;

    public float m_currentX;
    public float m_targetX;

    TextMeshProUGUI m_coinsText;
    TextMeshProUGUI m_newKeyboardCostText;

    Balance balance;
    MetaData metaData;

    public void Init(GameObject UI, Balance balance, MetaData metaData)
    {
        this.balance = balance;
        this.metaData = metaData;

        m_UI = UI;
        m_UI.SetActive(false);

        GUIRef guiRef = m_UI.GetComponent<GUIRef>();
        m_newKeyboardGO = guiRef.GetGameObject("NewKeyboard");
        m_keyboardParent = guiRef.GetGameObject("KeyboardParent").transform;

        GameObject topBarGO = guiRef.GetGameObject("TopBar");
        m_coinsText = topBarGO.GetComponent<GUIRef>().GetTextGUI("Coins");
        m_newKeyboardCostText = guiRef.GetTextGUI("NewKeyboardCost");

        m_keyboardSelectionBox = new GameObject[balance.MaxKeyboards];
        m_keyboardImages = new KeyboardImages[balance.MaxKeyboards];
        m_keyboardDatas = new KeyboardData[balance.MaxKeyboards];
        for (int i = 0; i < balance.MaxKeyboards; i++)
            m_keyboardDatas[i] = new KeyboardData();
    }

    public void Show()
    {
        m_UI.SetActive(true);

        m_keyboardCount = 0;
        for (int i = 0; i < balance.MaxKeyboards; i++)
            if (KeyboardDataIO.KeyboardDataExists(i))
            {
                KeyboardLogic.InitKeyboardData(m_keyboardDatas[m_keyboardCount]);
                KeyboardDataIO.LoadKeyboard(m_keyboardDatas[m_keyboardCount], i);

                GameObject keyboardSelectionBox = GameObject.Instantiate(AssetManager.Instance.KeyboardSelectionBox, m_keyboardParent);
                keyboardSelectionBox.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                keyboardSelectionBox.transform.localPosition = new Vector3(m_keyboardCount * m_keyboardOffset, 0.0f, 0.0f);
                m_keyboardSelectionBox[m_keyboardCount] = keyboardSelectionBox;

                m_keyboardImages[m_keyboardCount] = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[m_keyboardDatas[m_keyboardCount].KeyboardType], keyboardSelectionBox.transform);
                m_keyboardImages[m_keyboardCount].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_keyboardImages[m_keyboardCount].transform.localPosition = Vector3.zero;
                m_keyboardImages[m_keyboardCount].transform.SetAsFirstSibling();

                for (int keyIndex = 0; keyIndex < 26; keyIndex++)
                {
                    int flowerType = m_keyboardDatas[m_keyboardCount].FlowerType[keyIndex];
                    int progress = m_keyboardDatas[m_keyboardCount].FlowerProgress[keyIndex];
                    m_keyboardImages[m_keyboardCount].KeyImages[keyIndex].sprite = AssetManager.Instance.LoadFlowerProgress(balance.FlowerName[flowerType], balance.FlowerFrames[flowerType][progress]);
                }

                m_keyboardCount++;
            }

        m_newKeyboardGO.transform.localPosition = new Vector3(m_keyboardCount * m_keyboardOffset, 0.0f, 0.0f);

        m_coinsText.text = metaData.Coins.ToString("N0");

        m_keyboardIndex = 0;

    }

    public void Hide()
    {
        m_UI.SetActive(false);

        for (int i = 0; i < m_keyboardCount; i++)
            GameObject.Destroy(m_keyboardImages[i]);
        m_keyboardCount = 0;
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
        m_keyboardParent.transform.localPosition = new Vector3(m_currentX, 0.0f, 0.0f);

        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasReleasedThisFrame)
            {
                if (m_keyboardIndex < m_keyboardCount)
                {
                    Game.Instance.LoadKeyboard(m_keyboardIndex);
                    Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
                }
                else
                {
                    Game.Instance.LoadNewKeyboard(m_keyboardCount);
                    Game.Instance.SetMenuState(MENU_STATE.KEYBOARD_SELECTION);
                }
            }
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
            }
            if (Keyboard.current.leftArrowKey.wasReleasedThisFrame)
            {
                if (m_keyboardIndex > 0)
                {
                    m_keyboardIndex--;
                    m_targetX = m_keyboardIndex * -m_keyboardOffset;
                }
            }
            if (Keyboard.current.rightArrowKey.wasReleasedThisFrame)
            {
                if (m_keyboardIndex < m_keyboardCount)
                {
                    m_keyboardIndex++;
                    m_targetX = m_keyboardIndex * -m_keyboardOffset;
                }
            }

        }
    }
}
