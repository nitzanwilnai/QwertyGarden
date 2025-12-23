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
    int m_keyboardIndex;

    public void Init(GameObject UI)
    {
        m_UI = UI;
        m_UI.SetActive(false);

        GUIRef guiRef = m_UI.GetComponent<GUIRef>();
        m_keyboardParent = guiRef.GetGameObject("Keyboard").transform;
    }

    public void Show(KeyboardData keyboardData, int keyboardIndex)
    {
        this.keyboardData = keyboardData;
        m_keyboardIndex = keyboardIndex;
        m_UI.SetActive(true);
        m_keyboardImage = GameObject.Instantiate(AssetManager.Instance.KeyboardImages[keyboardData.KeyboardType], m_keyboardParent);
        m_keyboardImage.transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
        m_keyboardImage.transform.localPosition = Vector3.zero;
        m_keyboardImage.transform.SetAsFirstSibling();
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
                KeyboardDataIO.SaveKeyboard(keyboardData, m_keyboardIndex);
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
                Debug.Log("keyboardData.FlowerIndex["+keyIndex+"] " + keyboardData.FlowerIndex[keyIndex] + " m_keyboardImages.KeyImages["+keyIndex+"].name " + m_keyboardImage.KeyImages[keyIndex] + " changes to sprite AssetManager.Instance.Flowers["+flowerIndex+"].FlowerCard " + AssetManager.Instance.Flowers[flowerIndex].FlowerCard);
                m_keyboardImage.KeyImages[keyIndex].sprite = AssetManager.Instance.Flowers[flowerIndex].FlowerCard;
            }
        }
    }

}
