using QwertyGarden;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuVisual
{
    GameObject m_UI;

    public void Init(GameObject UI)
    {
        m_UI = UI;
        m_UI.SetActive(false);
    }

    public void Show()
    {
        m_UI.SetActive(true);
    }

    public void Hide()
    {
        m_UI.SetActive(false);
    }

    public void Tick()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasReleasedThisFrame)
            {
                Game.Instance.SetMenuState(MENU_STATE.GARDEN_SELECTION);
            }
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);
            }
        }
    }
}
