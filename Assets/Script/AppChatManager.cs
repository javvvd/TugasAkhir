using UnityEngine;
using Yarn.Unity;

public class ChatManager : MonoBehaviour
{
    [Header("Layar Aplikasi Chat & Telepon")]
    public GameObject appChat;
    public GameObject layarTelepon;

    [Header("Daftar Pesan")]
    public GameObject[] daftarPesanBima;
    public GameObject[] daftarPesanRaka;

    [YarnCommand("munculkan_layar_telepon")]
    public void MunculkanTelepon()
    {
        if (appChat != null) appChat.SetActive(false);
        if (layarTelepon != null) layarTelepon.SetActive(true);
    }

    [YarnCommand("tutup_telepon")]
    public void TutupTelepon()
    {
        if (layarTelepon != null) layarTelepon.SetActive(false);
    }

    [YarnCommand("buka_chat")]
    public void BukaChat()
    {
        if (appChat != null) appChat.SetActive(true);
    }

    [YarnCommand("tutup_chat")]
    public void TutupChat()
    {
        if (appChat != null) appChat.SetActive(false);
    }

    [YarnCommand("munculkan_pesan")]
    public void MunculkanPesanBima(int indexPesan)
    {
        if (indexPesan >= 0 && indexPesan < daftarPesanBima.Length)
        {
            if (daftarPesanBima[indexPesan] != null)
                daftarPesanBima[indexPesan].SetActive(true);
        }
    }

    [YarnCommand("munculkan_pesan_raka")]
    public void MunculkanPesanRaka(int indexPesanRaka)
    {
        if (indexPesanRaka >= 0 && indexPesanRaka < daftarPesanRaka.Length)
        {
            if (daftarPesanRaka[indexPesanRaka] != null)
                daftarPesanRaka[indexPesanRaka].SetActive(true);
        }
    }

    [YarnCommand("bersihkan_chat")]
    public void BersihkanChat()
    {
        foreach (GameObject pesan in daftarPesanBima)
        {
            if (pesan != null) pesan.SetActive(false);
        }
        foreach (GameObject pesan in daftarPesanRaka)
        {
            if (pesan != null) pesan.SetActive(false);
        }
    }
}