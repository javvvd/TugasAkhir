using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class ToastTransaksiManager : MonoBehaviour
{
    public static ToastTransaksiManager Instance { get; private set; }

    [Header("Referensi UI Banner")]
    [Tooltip("Panel root untuk toast banner")]
    public GameObject bannerToast;
    public RectTransform rectBanner;
    public CanvasGroup canvasGroup;
    public Image imgBackground;
    public Outline outlineBanner;
    public Image imgIconBadge;
    public TextMeshProUGUI teksIcon;
    public TextMeshProUGUI teksJudul;
    public TextMeshProUGUI teksSub;

    [Header("Pengaturan Animasi")]
    [SerializeField] private float durasiTampil = 2.0f; // Muncul selama 2 detik
    [SerializeField] private float durasiSlideMasuk = 0.3f;
    [SerializeField] private float durasiFadeOut = 1.0f; // Fade out pelan-pelan
    [SerializeField] private float posisiYMasuk = -42f;
    [SerializeField] private float posisiYSembunyi = 95f;

    [Header("Tema Warna")]
    [SerializeField] private Color warnaBgBeli = new Color(0.06f, 0.10f, 0.18f, 0.96f);
    [SerializeField] private Color warnaOutlineBeli = new Color(0.06f, 0.78f, 0.50f, 0.80f);
    [SerializeField] private Color warnaBadgeBeli = new Color(0.06f, 0.25f, 0.16f, 0.90f);

    [SerializeField] private Color warnaBgJual = new Color(0.12f, 0.08f, 0.10f, 0.96f);
    [SerializeField] private Color warnaOutlineJual = new Color(0.96f, 0.62f, 0.15f, 0.85f);
    [SerializeField] private Color warnaBadgeJual = new Color(0.30f, 0.14f, 0.08f, 0.90f);

    private Coroutine coroutineToast;

    void Awake()
    {
        if (Instance == null) Instance = this;

        if (bannerToast == null)
        {
            var hp = UnityEngine.Object.FindFirstObjectByType<HPManager>();
            if (hp != null && hp.panelHandphone != null)
            {
                var t = hp.panelHandphone.transform.Find("ToastNotifikasi");
                if (t != null) bannerToast = t.gameObject;
            }
        }

        if (rectBanner == null && bannerToast != null)
        {
            rectBanner = bannerToast.GetComponent<RectTransform>();
        }

        if (canvasGroup == null && bannerToast != null)
        {
            canvasGroup = bannerToast.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = bannerToast.AddComponent<CanvasGroup>();
            }
        }

        // Pastikan tersembunyi di awal
        if (bannerToast != null)
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            bannerToast.SetActive(false);
        }
    }


    public void Tampilkan(string judul, string subjudul, string tipe = "beli")
    {
        if (bannerToast != null && !bannerToast.activeSelf)
        {
            bannerToast.SetActive(true);
        }
        if (coroutineToast != null)
        {
            StopCoroutine(coroutineToast);
        }
        coroutineToast = StartCoroutine(ProsesToast(judul, subjudul, tipe.ToLower().Trim()));
    }

    private IEnumerator ProsesToast(string judul, string subjudul, string tipe)
    {
        if (bannerToast == null || rectBanner == null || canvasGroup == null) yield break;

        // Bawa ke layer paling depan dalam PanelHandphone agar selalu terlihat di atas app/chat
        if (bannerToast != null) bannerToast.transform.SetAsLastSibling();

        bool isBeli = (tipe != "jual" && tipe != "cutloss" && tipe != "cut_loss");

        // Set Warna & Icon sesuai tipe
        if (isBeli)
        {
            if (imgBackground != null) imgBackground.color = warnaBgBeli;
            if (outlineBanner != null) outlineBanner.effectColor = warnaOutlineBeli;
            if (imgIconBadge != null) imgIconBadge.color = warnaBadgeBeli;
            if (teksIcon != null) teksIcon.text = "<color=#10B981><b>$</b></color>";
        }
        else
        {
            if (imgBackground != null) imgBackground.color = warnaBgJual;
            if (outlineBanner != null) outlineBanner.effectColor = warnaOutlineJual;
            if (imgIconBadge != null) imgIconBadge.color = warnaBadgeJual;
            if (teksIcon != null) teksIcon.text = "<color=#F59E0B><b>!</b></color>";
        }

        // Set Isi Teks
        if (teksJudul != null) teksJudul.text = judul;
        if (teksSub != null) teksSub.text = subjudul;

        // Reset state awal animasi
        bannerToast.SetActive(true);
        canvasGroup.alpha = 0f;
        rectBanner.anchoredPosition = new Vector2(0f, posisiYSembunyi);

        // Putar suara SFX transaksi
        if (isBeli)
        {
            AudioManager.Instance?.PutarSFX("order");
        }
        else
        {
            AudioManager.Instance?.PutarSFX("klik");
        }

        // 1. ANIMASI SLIDE MASUK & FADE IN (0.3s)
        float elapsed = 0f;
        while (elapsed < durasiSlideMasuk)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durasiSlideMasuk);

            // Ease Out Quad
            float easeOut = 1f - (1f - t) * (1f - t);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, easeOut);
            float curY = Mathf.Lerp(posisiYSembunyi, posisiYMasuk, easeOut);
            rectBanner.anchoredPosition = new Vector2(0f, curY);

            yield return null;
        }

        rectBanner.anchoredPosition = new Vector2(0f, posisiYMasuk);
        canvasGroup.alpha = 1f;

        // 2. TAHAN SELAMA 2 DETIK
        yield return new WaitForSeconds(durasiTampil);

        // 3. ANIMASI FADE OUT PELAN-PELAN (1.0s)
        elapsed = 0f;
        Vector2 startPosFade = rectBanner.anchoredPosition;
        float targetYFade = posisiYMasuk + 15f; // Naik pelan ke atas sambil memudar

        while (elapsed < durasiFadeOut)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durasiFadeOut);

            // Smooth step fade out
            float smoothT = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, smoothT);
            float curY = Mathf.Lerp(startPosFade.y, targetYFade, smoothT);
            rectBanner.anchoredPosition = new Vector2(0f, curY);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        bannerToast.SetActive(false);
        coroutineToast = null;
    }
}
