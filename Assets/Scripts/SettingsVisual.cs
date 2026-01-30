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
        GameObject m_wpmOn;
        GameObject m_wpmOff;

        public void Init(GameObject UI, MetaData metaData)
        {
            this.metaData = metaData;
            m_UI = UI;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_musicOn = guiRef.GetGameObject("MusicOn");
            m_musicOff = guiRef.GetGameObject("MusicOff");
            m_sfxOn = guiRef.GetGameObject("SFXOn");
            m_sfxOff = guiRef.GetGameObject("SFXOff");
            m_wpmOn = guiRef.GetGameObject("WPMOn");
            m_wpmOff = guiRef.GetGameObject("WPMOff");

            Hide();
        }

        public void Show()
        {
            CommonVisual.AutoCanvasScaler(m_UI);

            m_UI.SetActive(true);

            updateUI();
        }

        void updateUI()
        {
            m_musicOn.SetActive(metaData.Music);
            m_musicOff.SetActive(!metaData.Music);

            m_sfxOn.SetActive(metaData.SFX);
            m_sfxOff.SetActive(!metaData.SFX);

            m_wpmOn.SetActive(metaData.WPM);
            m_wpmOff.SetActive(!metaData.WPM);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.anyKey.wasReleasedThisFrame)
                {
                    Game.Instance.SetMenuState(metaData.PrevMenuState);
                }
            }
        }
    }
}