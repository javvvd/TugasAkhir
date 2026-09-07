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

    public GameObject layarTelepon;
    [YarnCommand("munculkan_layar_telepon")]
    public void MunculkanTelepon()
    {
        // Matikan UI lain dan nyalakan UI layar panggilan
        appChat.SetActive(false);
        layarTelepon.SetActive(true);
    }

    [YarnCommand("tutup_telepon")]
    public void TutupTelepon()
    {
        layarTelepon.SetActive(false);
        // Bisa tambahkan kode untuk mematikan HP sepenuhnya di sini
    }

    [YarnCommand("buka_chat")]
    public void BukaChat()
    {
        appChat.SetActive(true);
    }

    [YarnCommand("tutup_chat")]
    public void TutupChat()
    {
        if (appChat != null)
        {
            appChat.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Objek appChat belum dimasukkan ke Inspector!");
        }
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
    [YarnCommand("bersihkan_chat")]
    public void BersihkanChat()
    {
        // Mematikan semua kotak chat Bima
        foreach (GameObject pesan in daftarPesanBima)
        {
            if (pesan != null) pesan.SetActive(false);
        }

        // Mematikan semua kotak chat Raka
        foreach (GameObject pesan in daftarPesanRaka)
        {
            if (pesan != null) pesan.SetActive(false);
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

    //App Saham
    [Header("Aplikasi Saham")]
    public GameObject AppSaham;

    [YarnCommand("buka_AppSaham")]
    public void BukaAppSaham()
    {
        AppSaham.SetActive(true);
    }

    [YarnCommand("tutup_AppSaham")]
    public void TutupAppSaham()
    {
        if (AppSaham != null) AppSaham.SetActive(false);
    }
}