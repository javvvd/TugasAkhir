using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class AppGrupManager : MonoBehaviour
{
    [Header("Aplikasi Grup Telegram")]
    public GameObject appGrup;
    public Image gambarLayarGrup;
    public Sprite[] daftarLayarGrup;

    [YarnCommand("buka_AppGrup")]
    public void BukaAppGrup()
    {
        if (appGrup != null) appGrup.SetActive(true);
    }

    [YarnCommand("tutup_AppGrup")]
    public void TutupAppGrup()
    {
        if (appGrup != null) appGrup.SetActive(false);
    }

    [YarnCommand("ganti_layar_grup")]
    public void GantiLayarGrup(int indexLayar)
    {
        if (indexLayar >= 0 && indexLayar < daftarLayarGrup.Length)
        {
            if (gambarLayarGrup != null)
            {
                gambarLayarGrup.sprite = daftarLayarGrup[indexLayar];
            }
        }
        else
        {
            Debug.LogWarning("Index layar grup tidak ditemukan! Pastikan array di Inspector sudah diisi.");
        }
    }
}