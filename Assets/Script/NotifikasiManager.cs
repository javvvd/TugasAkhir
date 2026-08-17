using UnityEngine;
using Yarn.Unity; // Wajib dipanggil untuk YarnCommand

public class NotifikasiManager : MonoBehaviour
{
    [Header("UI Pop-up")]
    public GameObject panelHandphone;

    // Fungsi untuk memunculkan HP
    [YarnCommand("munculkan_hp")]
    public void MunculkanHP()
    {
        panelHandphone.SetActive(true);
    }

    // Fungsi untuk menyembunyikan HP
    [YarnCommand("tutup_hp")]
    public void TutupHP()
    {
        panelHandphone.SetActive(false);
    }
}