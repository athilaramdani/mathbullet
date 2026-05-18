using UnityEngine;

/// <summary>
/// Buat sprite lingkaran putih secara procedural saat runtime.
/// Attach ke Player jika SpriteRenderer tidak punya sprite.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisual : MonoBehaviour
{
    public Color playerColor = Color.white;
    public int textureSize = 128;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr.sprite == null)
        {
            sr.sprite = CreateCircleSprite(textureSize, playerColor);
            sr.sortingOrder = 10;
        }
    }

    public static Sprite CreateCircleSprite(int size, Color color)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        float cx = size / 2f;
        float cy = size / 2f;
        float r  = size / 2f - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= r)
                {
                    // Soft edge anti-aliasing
                    float alpha = Mathf.Clamp01((r - dist) / 1.5f);
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        tex.Apply();

        return Sprite.Create(tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size // pixels per unit = size so sprite is 1 unit wide
        );
    }
}
