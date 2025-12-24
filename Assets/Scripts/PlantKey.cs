using UnityEngine;

namespace QwertyGarden
{
    public class PlantKey : MonoBehaviour
    {
        public GameObject DirtGO;
        public SpriteRenderer DirtSR;
        public Sprite[] Dirt;

        public void Randomize()
        {
            int dirtIndex = Mathf.FloorToInt(Random.value * Dirt.Length);
            DirtSR.sprite = Dirt[dirtIndex];
            DirtGO.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.value * 360.0f));
        }
    }
}