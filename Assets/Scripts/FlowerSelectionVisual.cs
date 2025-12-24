using CommonTools;
using QwertyGarden;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerSelectionVisual
{
    GameObject m_UI;

    KeyboardImages m_keyboardImage;

    Transform m_keyboardParent;

    KeyboardData keyboardData;
    GameData gameData;
    MetaData metaData;
    Balance balance;

    public void Init(GameObject UI, MetaData metaData, GameData gameData, Balance balance)
    {
        this.metaData = metaData;
        this.gameData = gameData;
        this.balance = balance;

        m_UI = UI;
        m_UI.SetActive(false);

        GUIRef guiRef = m_UI.GetComponent<GUIRef>();
        m_keyboardParent = guiRef.GetGameObject("Keyboard").transform;
    }

    public void Show(KeyboardData keyboardData)
    {
        this.keyboardData = keyboardData;
        m_UI.SetActive(true);
        m_keyboardImage = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardData.KeyboardType], m_keyboardParent);
        m_keyboardImage.transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
        m_keyboardImage.transform.localPosition = Vector3.zero;
        m_keyboardImage.transform.SetAsFirstSibling();

        for (int keyIndex = 0; keyIndex < 26; keyIndex++)
        {
            int flowerIndex = keyboardData.FlowerIndex[keyIndex];
            m_keyboardImage.KeyImages[keyIndex].sprite = AssetManager.Instance.Flowers[flowerIndex].FlowerCard;

        }
    }

    public void Hide()
    {
        m_UI.SetActive(false);
        GameObject.Destroy(m_keyboardImage);
    }

    public void Tick()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasReleasedThisFrame)
            {
                CozyLogic.StartCozy(keyboardData, gameData, balance);
                KeyboardDataIO.SaveKeyboard(keyboardData, metaData.KeyboardIndex);
                Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
            }
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Game.Instance.SetMenuState(MENU_STATE.KEYBOARD_SELECTION);
            }

            char c;
            int keyIndex = KeyboardLogic.GetTypedKeyIndex(out c);
            if (keyIndex > -1)
            {
                keyboardData.FlowerIndex[keyIndex] = (keyboardData.FlowerIndex[keyIndex] + 1) % AssetManager.Instance.Flowers.Length;
                int flowerIndex = keyboardData.FlowerIndex[keyIndex];
                Debug.Log("keyboardData.FlowerIndex[" + keyIndex + "] " + keyboardData.FlowerIndex[keyIndex] + " m_keyboardImages.KeyImages[" + keyIndex + "].name " + m_keyboardImage.KeyImages[keyIndex] + " changes to sprite AssetManager.Instance.Flowers[" + flowerIndex + "].FlowerCard " + AssetManager.Instance.Flowers[flowerIndex].FlowerCard);
                m_keyboardImage.KeyImages[keyIndex].sprite = AssetManager.Instance.Flowers[flowerIndex].FlowerCard;
            }
        }
    }

}
