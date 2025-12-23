using UnityEngine;

namespace QwertyGarden
{
    public class PlantKey : MonoBehaviour
    {
        public GameObject DirtGO;
        public SpriteRenderer DirtSR;
        public Sprite[] Dirt;

        public GameObject GrassGO;
        public SpriteRenderer GrassSR;
        public Sprite[] Grass;

        public void Randomize()
        {
            int dirtIndex = Mathf.FloorToInt(Random.value * Dirt.Length);
            DirtSR.sprite = Dirt[dirtIndex];
            DirtGO.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.value * 360.0f));

            int grassIndex = Mathf.FloorToInt(Random.value * Grass.Length);
            GrassSR.sprite = Grass[grassIndex];
            GrassSR.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.value * 10.0f - 5.0f));
            Vector3 grassPos = new Vector3(Random.value * 0.3f - 0.15f, Random.value * 0.1f - 0.05f + 0.25f, -4.0f);
            GrassGO.transform.localPosition = grassPos;
            // GrassGO.SetActive(Random.value < 0.5f);
            GrassGO.SetActive(false);
        }
    }
}