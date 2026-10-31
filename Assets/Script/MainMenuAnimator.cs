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

    private class FloatingOrb
    {
        public RectTransform rect;
        public Image img;
        public Vector2 speed;
        public float sinOffset;
        public float baseAlpha;
        public float rotSpeed;
    }

    private List<FloatingOrb> orbs = new List<FloatingOrb>();
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
            titleTransform.anchoredPosition = titleOriginPos + new Vector2(0f, yOffset);
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
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            float radius = size * 0.48f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(1f - (dist / radius));
                    // Smooth falloff untuk efek glowing orb
                    alpha = Mathf.Pow(alpha, 1.5f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }
            tex.Apply();
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

            // Posisi acak di seluruh layar
            rt.anchoredPosition = new Vector2(
                Random.Range(-halfW, halfW),
                Random.Range(-halfH, halfH)
            );

            Image img = orbGo.GetComponent<Image>();
            img.sprite = orbSprite;
            img.raycastTarget = false; // Tidak mengganggu klik tombol UI

            Color32 baseCol = orbColors[Random.Range(0, orbColors.Length)];
            img.color = baseCol;

            FloatingOrb orb = new FloatingOrb
            {
                rect = rt,
                img = img,
                speed = new Vector2(Random.Range(-10f, 10f), Random.Range(20f, 45f)), // Melayang perlahan ke atas
                sinOffset = Random.Range(0f, Mathf.PI * 2f),
                baseAlpha = baseCol.a / 255f,
                rotSpeed = Random.Range(-30f, 30f)
            };

            orbs.Add(orb);
        }
    }

    private void UpdateFloatingOrbs(float time)
    {
        if (particleContainer == null) return;

        float halfW = particleContainer.rect.width * 0.5f + 40f;
        float halfH = particleContainer.rect.height * 0.5f + 40f;

        for (int i = 0; i < orbs.Count; i++)
        {
            FloatingOrb orb = orbs[i];
            if (orb.rect == null) continue;

            // Gerak melayang perlahan ke atas dengan goyangan horizontal
            Vector2 pos = orb.rect.anchoredPosition;
            pos.y += orb.speed.y * Time.unscaledDeltaTime;
            pos.x += Mathf.Sin(time * 1.4f + orb.sinOffset) * 16f * Time.unscaledDeltaTime;

            // Jika keluar dari batas atas layar, reset ke bawah layar
            if (pos.y > halfH)
            {
                pos.y = -halfH;
                pos.x = Random.Range(-halfW + 40f, halfW - 40f);
            }

            orb.rect.anchoredPosition = pos;

            // Rotasi perlahan
            orb.rect.Rotate(0f, 0f, orb.rotSpeed * Time.unscaledDeltaTime);

            // Efek shimmering / kedip lembut
            if (orb.img != null)
            {
                float shimmer = Mathf.Sin(time * 2.2f + orb.sinOffset) * 0.35f + 0.65f;
                Color c = orb.img.color;
                c.a = orb.baseAlpha * shimmer;
                orb.img.color = c;
            }
        }
    }
}
