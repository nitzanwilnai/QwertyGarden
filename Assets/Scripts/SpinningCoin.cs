using UnityEngine;

namespace QwertyGarden
{
    public class SpinningCoin : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;
        public Sprite[] Sprites;
        public float Speed;
        float m_time;
        int m_index;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_time = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            m_time += Time.deltaTime;
            if (m_time > Speed)
            {
                m_time -= Speed;
                m_index = (m_index + 1) % Sprites.Length;
                SpriteRenderer.sprite = Sprites[m_index];
            }
        }
    }
}