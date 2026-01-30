using CommonTools;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QwertyGarden
{
    public class TitleVisual
    {
        GameObject m_UI;

        Transform m_flowerParent;

        UIFlower[] m_UIFlowers;

        int m_numFlowers;

        public void Init(GameObject UI, Balance balance)
        {
            m_UI = UI;

            m_numFlowers = Mathf.FloorToInt(Screen.width / 12.8f);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_flowerParent = guiRef.GetGameObject("Flowers").transform;

            m_UIFlowers = new UIFlower[m_numFlowers];
            for (int i = 0; i < m_UIFlowers.Length; i++)
            {
                UIFlower uiFlower = GameObject.Instantiate(AssetManager.Instance.TitleFlowerPrefab, m_flowerParent);

                m_UIFlowers[i] = uiFlower;

                int flowerType = Mathf.FloorToInt(Random.value * balance.NumFlowers);
                uiFlower.Image.sprite = AssetManager.Instance.LoadFlowerProgress(balance.FlowerName[flowerType], balance.FlowerFrames[flowerType][9]);
            }

            float screenWidth = Screen.width;
            float halfScreenWidth = screenWidth / 2.0f;
            for (int i = 0; i < m_UIFlowers.Length; i++)
            {
                float scale = Random.value * 0.1f + 0.9f;
                m_UIFlowers[i].transform.localScale = new Vector3(scale, scale, 1.0f);
            }

            int oneThird = m_numFlowers / 3;
            for (int i = 0; i < m_UIFlowers.Length; i++)
            {
                float startY = (i / oneThird) * (float)oneThird;
                m_UIFlowers[i].transform.localPosition = new Vector3(Random.value * screenWidth - halfScreenWidth, Random.value * oneThird - startY, 0.0f);
            }

            guiRef.GetButton("Start").onClick.AddListener(startGame);
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
            if (Keyboard.current.anyKey.wasReleasedThisFrame)
            {
                startGame();
            }
        }

        void startGame()
        {
            Game.Instance.SetMenuState(MENU_STATE.IN_GAME);
        }
    }
}