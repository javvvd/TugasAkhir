using UnityEngine;
using UnityEngine.UI; // Wajib untuk komponen Image
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
    public Sprite[] daftarLayarSaham;

    [Tooltip("Layar awal sebelum Raka membeli (AppSaham_Fase0). Jika kosong, diambil dari sprite awal Image.")]
    public Sprite layarAwal;

    void Awake()
    {
        if (layarAwal == null && gambarLayarSaham != null)
        {
            layarAwal = gambarLayarSaham.sprite;
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
}