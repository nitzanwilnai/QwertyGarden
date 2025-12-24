using CommonTools;
using QwertyGarden;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardSelectionVisual
{
    float m_keyboardOffset = 1088.0f;
    float m_slideVelocity = 2000.0f;
    GameObject m_UI;

    Transform m_keyboardParent;

    KeyboardImages[] m_keyboardImages;
    KeyboardData[] m_keyboardDatas;
    GameObject[] m_keyboardSelectionBox;
    int m_keyboardType = 0;

    public float m_currentX;
    public float m_targetX;

    KeyboardData keyboardData;
    MetaData metaData;
    Balance balance;

    public void Init(GameObject UI, Balance balance)
    {
        this.balance = balance;

        m_UI = UI;
        m_UI.SetActive(false);

        GUIRef guiRef = m_UI.GetComponent<GUIRef>();
        m_keyboardParent = guiRef.GetGameObject("KeyboardParent").transform;

        m_keyboardSelectionBox = new GameObject[balance.MaxKeyboards];
        m_keyboardImages = new KeyboardImages[balance.MaxKeyboards];
        m_keyboardDatas = new KeyboardData[balance.MaxKeyboards];
        for (int i = 0; i < balance.MaxKeyboards; i++)
            m_keyboardDatas[i] = new KeyboardData();
    }

    public void Show(KeyboardData keyboardData, MetaData metaData)
    {
        this.keyboardData = keyboardData;
        this.metaData = metaData;

        m_UI.SetActive(true);

        for (int keyboardType = 0; keyboardType < AssetManager.Instance.KeyboardImages.Length; keyboardType++)
        {
            GameObject keyboardSelectionBox = GameObject.Instantiate(AssetManager.Instance.KeyboardSelectionBox, m_keyboardParent);
            keyboardSelectionBox.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            keyboardSelectionBox.transform.localPosition = new Vector3(keyboardType * m_keyboardOffset, 0.0f, 0.0f);
            m_keyboardSelectionBox[keyboardType] = keyboardSelectionBox;

            m_keyboardImages[keyboardType] = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardType], keyboardSelectionBox.transform);
            m_keyboardImages[keyboardType].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            m_keyboardImages[keyboardType].transform.localPosition = Vector3.zero;
            m_keyboardImages[keyboardType].transform.SetAsFirstSibling();
        }

        m_keyboardType = 0;

    }

    public void Hide()
    {
        m_UI.SetActive(false);

        for (int keyboardIdx = 0; keyboardIdx < AssetManager.Instance.KeyboardImages.Length; keyboardIdx++)
            GameObject.Destroy(m_keyboardImages[keyboardIdx]);
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
                keyboardData.KeyboardType = m_keyboardType;
                Game.Instance.SetMenuState(MENU_STATE.FLOWER_SELECTION);
            }
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Game.Instance.SetMenuState(MENU_STATE.GARDEN_SELECTION);
            }
            if (Keyboard.current.leftArrowKey.wasReleasedThisFrame)
            {
                if (m_keyboardType > 0)
                {
                    m_keyboardType--;
                    m_targetX = m_keyboardType * -m_keyboardOffset;
                }
            }
            if (Keyboard.current.rightArrowKey.wasReleasedThisFrame)
            {
                if (m_keyboardType < AssetManager.Instance.KeyboardImages.Length - 1)
                {
                    m_keyboardType++;
                    m_targetX = m_keyboardType * -m_keyboardOffset;
                }
            }

        }
    }
}
