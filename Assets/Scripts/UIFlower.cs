using UnityEngine;
using UnityEngine.UI;

namespace QwertyGarden
{
    [DisallowMultipleComponent]
    public class UIFlower : MonoBehaviour
    {
        [Header("References")]
        public Image Image;

        [Header("Randomization")]
        [Tooltip("Random phase in [0, 2π].")]
        public bool RandomizePhase = true;

        [Tooltip("Random speed multiplier range per flower.")]
        public Vector2 SpeedMulRange = new Vector2(0.75f, 1.35f);

        // Shader property IDs
        static readonly int PhaseOffsetID = Shader.PropertyToID("_PhaseOffset");
        static readonly int SpeedMulID    = Shader.PropertyToID("_SpeedMul");

        Material _instanceMat;

        void Awake()
        {
            if (Image == null)
                Image = GetComponent<Image>();

            // Clone the material once for this Image
            // IMPORTANT: use Image.material (not sharedMaterial) and instantiate it.
            var baseMat = Image.material;

            _instanceMat = Instantiate(baseMat);
            _instanceMat.name = baseMat.name + " (UI Instance)";
            Image.material = _instanceMat;

            ApplyRandoms();
        }

        void OnEnable()
        {
            // If something reassigns the material (some UI workflows do),
            // make sure our instance is still in place.
            if (_instanceMat != null && Image != null && Image.material != _instanceMat)
                Image.material = _instanceMat;
        }

        void OnDestroy()
        {
            if (_instanceMat != null)
            {
                Destroy(_instanceMat);
                _instanceMat = null;
            }
        }

        public void ApplyRandoms()
        {
            if (_instanceMat == null) return;

            if (RandomizePhase)
            {
                float phase = Random.Range(0f, Mathf.PI * 2f);
                _instanceMat.SetFloat(PhaseOffsetID, phase);
            }

            float min = SpeedMulRange.x;
            float max = SpeedMulRange.y;
            if (max < min) { var tmp = min; min = max; max = tmp; }

            float speedMul = Random.Range(min, max);
            _instanceMat.SetFloat(SpeedMulID, speedMul);
        }
    }
}
