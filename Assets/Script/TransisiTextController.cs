using UnityEngine;
using TMPro;
using Yarn.Unity;

public class TransisiTextController : MonoBehaviour
{
    [Header("Referensi UI")]
    public TextMeshProUGUI teksLayarHitam;

    void Start()
    {
        // Menyembunyikan teks saat game baru dimulai
        if (teksLayarHitam != null)
        {
            teksLayarHitam.gameObject.SetActive(false);
        }
    }

    // Perintah Yarn untuk memunculkan teks custom
    [YarnCommand("munculkan_teks")]
    public void MunculkanTeks(string kalimat)
    {
        if (teksLayarHitam != null)
        {
            teksLayarHitam.text = kalimat;
            teksLayarHitam.gameObject.SetActive(true);
        }
    }

    // Perintah Yarn untuk menyembunyikan teks kembali
    [YarnCommand("hapus_teks")]
    public void HapusTeks()
    {
        if (teksLayarHitam != null)
        {
            teksLayarHitam.gameObject.SetActive(false);
        }
    }
}