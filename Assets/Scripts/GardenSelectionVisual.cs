using CommonTools;
using QwertyGarden;
using UnityEngine;
using UnityEngine.InputSystem;

public class GardenSelectionVisual
{
    float m_keyboardOffset = 1088.0f;
    float m_slideVelocity = 2000.0f;
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

    Balance balance;

    public void Init(GameObject UI, Balance balance)
    {
        this.balance = balance;

        m_UI = UI;
        m_UI.SetActive(false);

        GUIRef guiRef = m_UI.GetComponent<GUIRef>();
        m_newKeyboardGO = guiRef.GetGameObject("NewKeyboard");
        m_keyboardParent = guiRef.GetGameObject("KeyboardParent").transform;

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
                    int flowerIndex = m_keyboardDatas[m_keyboardCount].FlowerIndex[keyIndex];
                    m_keyboardImages[m_keyboardCount].KeyImages[keyIndex].sprite = AssetManager.Instance.Flowers[flowerIndex].FlowerCard;

                }

                m_keyboardCount++;
            }

        m_newKeyboardGO.transform.localPosition = new Vector3(m_keyboardCount * m_keyboardOffset, 0.0f, 0.0f);

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
                Game.Instance.LoadKeyboard(m_keyboardIndex);
                Game.Instance.SetMenuState(MENU_STATE.FLOWER_SELECTION);
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
