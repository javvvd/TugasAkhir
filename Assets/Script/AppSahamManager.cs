using UnityEngine;
using UnityEngine.UI; // Wajib untuk komponen Image
using Yarn.Unity;

public class AppSahamManager : MonoBehaviour
{
    [Header("Aplikasi Saham")]
    public GameObject appSaham;
    public Image gambarLayarSaham;
    public Sprite[] daftarLayarSaham;

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
        if (indexLayar >= 0 && indexLayar < daftarLayarSaham.Length)
        {
            if (gambarLayarSaham != null)
            {
                gambarLayarSaham.sprite = daftarLayarSaham[indexLayar];
            }
        }
        else
        {
            Debug.LogWarning("Index layar saham tidak ditemukan! Pastikan array di Inspector sudah diisi.");
        }
    }
}