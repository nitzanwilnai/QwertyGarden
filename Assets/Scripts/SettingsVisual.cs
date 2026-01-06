using CommonTools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public class SettingsVisual
    {
        GameObject m_UI;

        MetaData metaData;

        GameObject m_musicOn;
        GameObject m_musicOff;
        GameObject m_sfxOn;
        GameObject m_sfxOff;

        public void Init(GameObject UI, MetaData metaData)
        {
            this.metaData = metaData;
            m_UI = UI;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_musicOn = guiRef.GetGameObject("MusicOn");
            m_musicOff = guiRef.GetGameObject("MusicOff");
            m_sfxOn = guiRef.GetGameObject("SFXOn");
            m_sfxOff = guiRef.GetGameObject("SFXOff");

            Hide();
        }

        public void Show()
        {
            m_UI.SetActive(true);

            updateUI();
        }

        void updateUI()
        {
            m_musicOn.SetActive(metaData.Music);
            m_musicOff.SetActive(!metaData.Music);

            m_sfxOn.SetActive(metaData.SFX);
            m_sfxOff.SetActive(!metaData.SFX);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.sKey.wasReleasedThisFrame)
                {
                    metaData.SFX = !metaData.SFX;
                    updateUI();
                }
                else if (Keyboard.current.mKey.wasReleasedThisFrame)
                {
                    metaData.Music = !metaData.Music;
                    updateUI();
                }
                else if (Keyboard.current.escapeKey.wasReleasedThisFrame || Keyboard.current.leftShiftKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(metaData.PrevMenuState);
                }
            }
        }
    }
}