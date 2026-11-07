using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuAnimator : MonoBehaviour
{
    [Header("1. Animasi Floating Judul")]
    public RectTransform titleTransform;
    public float titleFloatDistance = 8f;
    public float titleFloatSpeed = 1.6f;
    private Vector2 titleOriginPos;

    [Header("2. Animasi Denyut Tombol Play")]
    public RectTransform playBtnTransform;
    public float pulseMinScale = 0.98f;
    public float pulseMaxScale = 1.04f;
    public float pulseSpeed = 2.2f;

    [Header("3. Partikel Kilau / Floating Currency Orbs")]
    public RectTransform particleContainer;
    public Sprite particleSprite;
    public int particleCount = 20;

    private struct FloatingOrb
    {
        public RectTransform rect;
        public CanvasRenderer canvasRenderer;
        public float speedY;
        public float sinOffset;
        public float baseAlpha;
    }

    private readonly List<FloatingOrb> orbs = new List<FloatingOrb>(24);
    private static Sprite proceduralCircleSprite;

    void Start()
    {
        // 1. Simpan posisi awal Title
        if (titleTransform != null)
        {
            titleOriginPos = titleTransform.anchoredPosition;
        }

        // 2. Setup Partikel mengambang di background
        InitFloatingOrbs();
    }

    void Update()
    {
        float time = Time.unscaledTime;

        // 1. Floating Judul (Naik turun lembut sinusoidal)
        if (titleTransform != null)
        {
            float yOffset = Mathf.Sin(time * titleFloatSpeed) * titleFloatDistance;
            titleTransform.anchoredPosition = new Vector2(titleOriginPos.x, titleOriginPos.y + yOffset);
        }

        // 2. Pulsing Tombol Play (Denyut lembut breathing effect)
        if (playBtnTransform != null)
        {
            float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, (Mathf.Sin(time * pulseSpeed) + 1f) * 0.5f);
            playBtnTransform.localScale = new Vector3(scale, scale, 1f);
        }

        // 3. Update Partikel Melayang
        UpdateFloatingOrbs(time);
    }

    private Sprite GetCircleSprite()
    {
        if (particleSprite != null) return particleSprite;

        if (proceduralCircleSprite == null)
        {
            const int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            Color32[] pixels = new Color32[size * size];

            float center = size * 0.5f;
            float invRadius = 1f / (size * 0.48f);

            for (int y = 0; y < size; y++)
            {
                float dy = y - center;
                int rowOffset = y * size;
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(1f - dist * invRadius);
                    byte a = (byte)Mathf.RoundToInt(Mathf.Pow(alpha, 1.5f) * 255f);
                    pixels[rowOffset + x] = new Color32(255, 255, 255, a);
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            proceduralCircleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        return proceduralCircleSprite;
    }

    private void InitFloatingOrbs()
    {
        if (particleContainer == null) return;

        Sprite orbSprite = GetCircleSprite();

        // Warna-warna bernuansa investasi (Warm Gold, Golden Amber, Cyan Finansial, Emerald Mint)
        Color32[] orbColors = new Color32[]
        {
            new Color32(0xFE, 0xF0, 0x8A, 0x80), // Warm Gold
            new Color32(0xFA, 0xCC, 0x15, 0x75), // Yellow Cuan
            new Color32(0x38, 0xBD, 0xF8, 0x70), // Sky Blue
            new Color32(0x34, 0xD3, 0x99, 0x80)  // Mint Emerald
        };

        float halfW = particleContainer.rect.width * 0.5f;
        float halfH = particleContainer.rect.height * 0.5f;

        for (int i = 0; i < particleCount; i++)
        {
            GameObject orbGo = new GameObject("Orb_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            orbGo.transform.SetParent(particleContainer, false);

            RectTransform rt = orbGo.GetComponent<RectTransform>();
            float sz = Random.Range(10f, 26f);
            rt.sizeDelta = new Vector2(sz, sz);

            rt.anchoredPosition = new Vector2(
                Random.Range(-halfW, halfW),
                Random.Range(-halfH, halfH)
            );

            Image img = orbGo.GetComponent<Image>();
            img.sprite = orbSprite;
            img.raycastTarget = false;

            Color32 baseCol = orbColors[Random.Range(0, orbColors.Length)];
            img.color = new Color32(baseCol.r, baseCol.g, baseCol.b, 255);

            CanvasRenderer cr = orbGo.GetComponent<CanvasRenderer>();
            float baseAlpha = baseCol.a / 255f;
            cr.SetAlpha(baseAlpha);

            orbs.Add(new FloatingOrb
            {
                rect = rt,
                canvasRenderer = cr,
                speedY = Random.Range(20f, 45f),
                sinOffset = Random.Range(0f, Mathf.PI * 2f),
                baseAlpha = baseAlpha
            });
        }
    }

    private void UpdateFloatingOrbs(float time)
    {
        if (particleContainer == null) return;

        float halfW = particleContainer.rect.width * 0.5f + 40f;
        float halfH = particleContainer.rect.height * 0.5f + 40f;
        float dt = Time.unscaledDeltaTime;

        for (int i = 0; i < orbs.Count; i++)
        {
            FloatingOrb orb = orbs[i];
            if (orb.rect == null) continue;

            // Gerak melayang perlahan ke atas dengan goyangan horizontal
            Vector2 pos = orb.rect.anchoredPosition;
            pos.y += orb.speedY * dt;
            pos.x += Mathf.Sin(time * 1.4f + orb.sinOffset) * 16f * dt;

            if (pos.y > halfH)
            {
                pos.y = -halfH;
                pos.x = Random.Range(-halfW + 40f, halfW - 40f);
            }

            orb.rect.anchoredPosition = pos;

            // Update alpha via CanvasRenderer tanpa memicu rebuild mesh Canvas
            if (orb.canvasRenderer != null)
            {
                float shimmer = Mathf.Sin(time * 2.2f + orb.sinOffset) * 0.35f + 0.65f;
                orb.canvasRenderer.SetAlpha(orb.baseAlpha * shimmer);
            }
        }
    }
}
