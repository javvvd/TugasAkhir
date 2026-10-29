using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class AppSahamManager : MonoBehaviour
{
    [Header("Aplikasi Saham")]
    public GameObject appSaham;
    public Image gambarLayarSaham;

    // Urutan index di Inspector (harus sesuai dengan Naskah.yarn):
    // 0 = AppSaham_FaseFomo1         (harga 100, 200 lot, modal 2 jt)
    // 1 = AppSaham_FaseFomo2         (harga 110, +10%, untung 200 rb)
    // 2 = AppSaham_TambahMuatan      (harga 110, 472 lot, avg 106)
    // 3 = AppSahamRugi_ARB1          (harga 85, 472 lot, rugi 980 rb)
    // 4 = AppSahamRugi_Bangkrut      (harga 50 gocap, 472 lot)
    // 5 = AppSahamRugi_Cutloss       (harga 85, 200 lot, rugi 300 rb)
    // 6 = AppSaham_RasionalPeak      (harga 135, ARA +35%)
    // 7 = AppSaham_RasionalPeakBeli  (harga 135, 370 lot, modal 5 jt)
    // 8 = AppSaham_RasionalPeakRugi  (harga 85, 370 lot, rugi 1,85 jt)
    // 9 = AppSaham_LaporanKeuangan   (laporan keuangan minus & market cap kecil)
    // 10 = AppSahamRugi_CutlossJual  (harga 87 ARB, portofolio kosong/terjual, tombol Beli)
    public Sprite[] daftarLayarSaham;

    [Tooltip("Layar awal sebelum Raka membeli (AppSaham_Fase0). Jika kosong, diambil dari sprite awal Image.")]
    public Sprite layarAwal;

    [Header("Efek Dramatis ARB / Gagal Jual")]
    [Tooltip("Pop-up notifikasi merah bahwa order jual gagal karena ARB")]
    public GameObject bannerGagalJual;

    [Tooltip("Transform Panel Handphone untuk efek getaran saat gagal eksekusi")]
    public RectTransform rectPanelHandphone;

    private Coroutine coroutineGagalJual;
    private Vector2 posisiAwalPanel;

    void Awake()
    {
        if (layarAwal == null && gambarLayarSaham != null)
        {
            layarAwal = gambarLayarSaham.sprite;
        }

        if (rectPanelHandphone == null)
        {
            var hpMgr = Object.FindFirstObjectByType<HPManager>();
            if (hpMgr != null && hpMgr.panelHandphone != null)
            {
                rectPanelHandphone = hpMgr.panelHandphone.GetComponent<RectTransform>();
            }
        }

        if (rectPanelHandphone != null)
        {
            posisiAwalPanel = rectPanelHandphone.anchoredPosition;
        }
    }

    [YarnCommand("buka_AppSaham")]
    public void BukaAppSaham()
    {
        if (appSaham != null) appSaham.SetActive(true);
    }

    [YarnCommand("tutup_AppSaham")]
    public void TutupAppSaham()
    {
        if (appSaham != null) appSaham.SetActive(false);
        if (bannerGagalJual != null) bannerGagalJual.SetActive(false);
        if (rectPanelHandphone != null && posisiAwalPanel != Vector2.zero)
        {
            rectPanelHandphone.anchoredPosition = posisiAwalPanel;
        }
    }

    [YarnCommand("ganti_layar_saham")]
    public void GantiLayarSaham(int indexLayar)
    {
        if (daftarLayarSaham != null && indexLayar >= 0 && indexLayar < daftarLayarSaham.Length)
        {
            if (gambarLayarSaham != null)
            {
                gambarLayarSaham.sprite = daftarLayarSaham[indexLayar];
            }
        }
        else
        {
            Debug.LogWarning("Index layar saham " + indexLayar + " tidak ditemukan! Pastikan array di Inspector sudah diisi.");
        }
    }

    // Mengembalikan layar ke tampilan awal (harga 100, belum punya saham)
    [YarnCommand("reset_layar_saham")]
    public void ResetLayarSaham()
    {
        if (bannerGagalJual != null) bannerGagalJual.SetActive(false);

        if (gambarLayarSaham != null)
        {
            if (layarAwal == null && gambarLayarSaham.sprite != null)
            {
                layarAwal = gambarLayarSaham.sprite;
            }
            if (layarAwal != null)
            {
                gambarLayarSaham.sprite = layarAwal;
            }
        }
    }

    [YarnCommand("efek_gagal_jual")]
    public void EfekGagalJual()
    {
        if (coroutineGagalJual != null) StopCoroutine(coroutineGagalJual);
        coroutineGagalJual = StartCoroutine(ProsesEfekGagalJual());
    }

    private IEnumerator ProsesEfekGagalJual()
    {
        if (rectPanelHandphone != null)
        {
            posisiAwalPanel = rectPanelHandphone.anchoredPosition;
        }

        // 1. Tampilkan banner pop-up
        if (bannerGagalJual != null)
        {
            bannerGagalJual.SetActive(true);
            bannerGagalJual.transform.localScale = Vector3.one * 0.7f;
        }

        // Suara klik panik
        AudioManager.Instance?.PutarSFX("klik");

        // 2. Animasi Scale Pop-in & Getar Panel HP (Vibration haptic feedback)
        float durasi = 0.5f;
        float elapsed = 0f;

        while (elapsed < durasi)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durasi);

            // Pop-in scale dengan bounce overshoot
            if (bannerGagalJual != null && t < 0.6f)
            {
                float scaleT = t / 0.6f;
                float currentScale = Mathf.Lerp(0.7f, 1.05f, Mathf.Sin(scaleT * Mathf.PI * 0.5f));
                bannerGagalJual.transform.localScale = Vector3.one * currentScale;
            }
            else if (bannerGagalJual != null)
            {
                float settleT = (t - 0.6f) / 0.4f;
                bannerGagalJual.transform.localScale = Vector3.Lerp(Vector3.one * 1.05f, Vector3.one, settleT);
            }

            // Getaran HP (Simulasi feedback tombol reject)
            if (rectPanelHandphone != null)
            {
                float decay = 1f - t;
                float shakeX = (Random.value * 2f - 1f) * 14f * decay;
                float shakeY = (Random.value * 2f - 1f) * 8f * decay;
                rectPanelHandphone.anchoredPosition = posisiAwalPanel + new Vector2(shakeX, shakeY);
            }

            yield return null;
        }

        if (rectPanelHandphone != null)
        {
            rectPanelHandphone.anchoredPosition = posisiAwalPanel;
        }

        coroutineGagalJual = null;
    }
}