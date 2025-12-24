using UnityEngine;

namespace QwertyGarden
{
    public class Flower : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;
        public Sprite[] Sprites;
        public Animation Animation;
        int m_flowerVariationIndex;

        static readonly int PhaseOffsetID = Shader.PropertyToID("_PhaseOffset");

        void Awake()
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            SpriteRenderer.GetPropertyBlock(mpb);

            // Random phase in [0, 2π]
            float randomPhase = Random.Range(0f, Mathf.PI * 2f);
            mpb.SetFloat(PhaseOffsetID, randomPhase);

            SpriteRenderer.SetPropertyBlock(mpb);
        }

        public void ResetFlower(int numFrames)
        {
            m_flowerVariationIndex = Mathf.FloorToInt(Random.value * (Sprites.Length / numFrames)) * numFrames;
            SpriteRenderer.sprite = Sprites[m_flowerVariationIndex];
        }

        public void GrowFlower(int index)
        {
            SpriteRenderer.sprite = Sprites[m_flowerVariationIndex + index];
            Animation.Play("FlowerGrow");
        }
    }
}