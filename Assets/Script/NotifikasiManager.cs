using UnityEngine;
using Yarn.Unity;

public class NotifikasiManager : MonoBehaviour
{
    [Header("UI Pop-up")]
    public GameObject panelHandphone;
    [Header("Layar Aplikasi")]
    public GameObject appChat;
    public GameObject latarGelap; // <-- Tambahkan baris ini

    [YarnCommand("munculkan_hp")]
    public void MunculkanHP()
    {
        latarGelap.SetActive(true); // <-- Nyalakan latar gelap
        panelHandphone.SetActive(true);
    }

    [YarnCommand("tutup_hp")]
    public void TutupHP()
    {
        panelHandphone.SetActive(false);
        latarGelap.SetActive(false); // <-- Matikan latar gelap
    }

    [YarnCommand("buka_chat")]
    public void BukaChat()
    {
        appChat.SetActive(true);
    }

    // ... (Kode latar gelap dan aplikasi yang sebelumnya sudah ada) ...

    [Header("Daftar Pesan Bima")]
    // Array ini untuk menyimpan semua kotak pesan Bima secara berurutan
    public GameObject[] daftarPesanBima;

    [YarnCommand("munculkan_pesan")]
    public void MunculkanPesanBima(int indexPesan)
    {
        // Mengecek agar index tidak error
        if (indexPesan >= 0 && indexPesan < daftarPesanBima.Length)
        {
            // Menyalakan pesan sesuai urutan nomor yang diminta Yarn Spinner
            daftarPesanBima[indexPesan].SetActive(true);
        }
    }
    //Pesan Raka
    [Header("Daftar Pesan Raka")]
    // Array ini untuk menyimpan semua kotak pesan Bima secara berurutan
    public GameObject[] daftarPesanRaka;

    [YarnCommand("munculkan_pesan_raka")]
    public void MunculkanPesanRaka(int indexPesanRaka)
    {
        // Mengecek agar index tidak error
        if (indexPesanRaka >= 0 && indexPesanRaka < daftarPesanRaka.Length)
        {
            // Menyalakan pesan sesuai urutan nomor yang diminta Yarn Spinner
            daftarPesanRaka[indexPesanRaka].SetActive(true);
        }
    }
}