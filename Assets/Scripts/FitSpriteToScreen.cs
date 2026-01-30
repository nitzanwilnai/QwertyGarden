using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToScreen : MonoBehaviour
{
    void Start()
    {
        Fit();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            Fit();
    }

    void Fit()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr.sprite == null)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        // Screen size in world units
        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        // Sprite size in world units
        Vector2 spriteSize = sr.sprite.bounds.size;

        // Scale to fit while maintaining aspect ratio
        float scale = Mathf.Max(
            screenWidth / spriteSize.x,
            screenHeight / spriteSize.y
        );

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}