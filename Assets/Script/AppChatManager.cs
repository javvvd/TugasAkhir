using UnityEngine;
using Yarn.Unity;

public class ChatManager : MonoBehaviour
{
    [Header("Layar Aplikasi Chat Utama (WhatsApp Bima Pagi)")]
    public GameObject appChat; // ChatBima (dipertahankan untuk kompatibilitas)
    public GameObject layarTelepon;

    [Header("Daftar Layar Chat Spesifik")]
    public GameObject chatMama;
    public GameObject chatBimaBangkrut;
    public GameObject chatBimaSelamat;
    public GameObject chatGrupAnjlok;
    public GameObject chatGrupHold;
    public GameObject chatGrupPuncak;

    [Header("Daftar Pesan Bima Pagi")]
    public GameObject[] daftarPesanBima;
    public GameObject[] daftarPesanRaka;

    void Awake()
    {
        // Cari otomatis jika referensi belum dipasang di Inspector
        GameObject panelHp = GameObject.Find("PanelHandphone");
        if (panelHp == null)
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var g in all)
            {
                if (g.name == "PanelHandphone") { panelHp = g; break; }
            }
        }

        if (panelHp != null)
        {
            Transform t = panelHp.transform;
            if (appChat == null) appChat = t.Find("ChatBima")?.gameObject;
            if (layarTelepon == null) layarTelepon = t.Find("TeleponBima")?.gameObject;
            if (chatMama == null) chatMama = t.Find("Chat_mama")?.gameObject;
            if (chatBimaBangkrut == null) chatBimaBangkrut = t.Find("Chat_bima_bangkrut")?.gameObject;
            if (chatBimaSelamat == null) chatBimaSelamat = t.Find("Chat_bima_selamat")?.gameObject;
            if (chatGrupAnjlok == null) chatGrupAnjlok = t.Find("Chat_grup_anjlok")?.gameObject;
            if (chatGrupHold == null) chatGrupHold = t.Find("Chat_grup_hold")?.gameObject;
            if (chatGrupPuncak == null) chatGrupPuncak = t.Find("Chat_grup_puncak")?.gameObject;
        }
    }

    [YarnCommand("munculkan_layar_telepon")]
    public void MunculkanTelepon()
    {
        TutupSemuaChat();
        if (layarTelepon != null) layarTelepon.SetActive(true);
    }

    [YarnCommand("tutup_telepon")]
    public void TutupTelepon()
    {
        if (layarTelepon != null) layarTelepon.SetActive(false);
    }

    public void TutupSemuaChat()
    {
        if (appChat != null) appChat.SetActive(false);
        if (chatMama != null) chatMama.SetActive(false);
        if (chatBimaBangkrut != null) chatBimaBangkrut.SetActive(false);
        if (chatBimaSelamat != null) chatBimaSelamat.SetActive(false);
        if (chatGrupAnjlok != null) chatGrupAnjlok.SetActive(false);
        if (chatGrupHold != null) chatGrupHold.SetActive(false);
        if (chatGrupPuncak != null) chatGrupPuncak.SetActive(false);
    }

    [YarnCommand("buka_chat")]
    public void BukaChat(string namaChat = "")
    {
        TutupSemuaChat();

        string key = namaChat != null ? namaChat.ToLower().Trim() : "";

        switch (key)
        {
            case "mama":
            case "chat_mama":
                if (chatMama != null)
                {
                    chatMama.SetActive(true);
                    AktifkanKontenPesan(chatMama);
                }
                break;

            case "bima_bangkrut":
            case "chat_bima_bangkrut":
            case "bangkrut":
                if (chatBimaBangkrut != null)
                {
                    chatBimaBangkrut.SetActive(true);
                    AktifkanKontenPesan(chatBimaBangkrut);
                }
                break;

            case "bima_selamat":
            case "chat_bima_selamat":
            case "selamat":
            case "ending_d":
                if (chatBimaSelamat != null)
                {
                    chatBimaSelamat.SetActive(true);
                    AktifkanKontenPesan(chatBimaSelamat);
                }
                break;

            case "grup_anjlok":
            case "chat_grup_anjlok":
            case "anjlok":
                if (chatGrupAnjlok != null)
                {
                    chatGrupAnjlok.SetActive(true);
                    AktifkanKontenPesan(chatGrupAnjlok);
                }
                break;

            case "grup_hold":
            case "chat_grup_hold":
            case "hold":
            case "average_down":
                if (chatGrupHold != null)
                {
                    chatGrupHold.SetActive(true);
                    AktifkanKontenPesan(chatGrupHold);
                }
                break;

            case "grup_puncak":
            case "chat_grup_puncak":
            case "puncak":
                if (chatGrupPuncak != null)
                {
                    chatGrupPuncak.SetActive(true);
                    AktifkanKontenPesan(chatGrupPuncak);
                }
                break;

            default:
                // Default ke chat Bima pagi
                if (appChat != null) appChat.SetActive(true);
                break;
        }
    }

    [YarnCommand("buka_chat_mama")]
    public void BukaChatMama() => BukaChat("mama");

    [YarnCommand("buka_chat_bima_bangkrut")]
    public void BukaChatBimaBangkrut() => BukaChat("bima_bangkrut");

    [YarnCommand("buka_chat_bima_selamat")]
    public void BukaChatBimaSelamat() => BukaChat("bima_selamat");

    [YarnCommand("buka_chat_grup_anjlok")]
    public void BukaChatGrupAnjlok() => BukaChat("grup_anjlok");

    [YarnCommand("buka_chat_grup_hold")]
    public void BukaChatGrupHold() => BukaChat("grup_hold");

    [YarnCommand("buka_chat_grup_puncak")]
    public void BukaChatGrupPuncak() => BukaChat("grup_puncak");

    private void AktifkanKontenPesan(GameObject chatGo)
    {
        if (chatGo == null) return;
        Transform konten = chatGo.transform.Find("Viewport/Konten");
        if (konten != null)
        {
            foreach (Transform p in konten)
            {
                p.gameObject.SetActive(true);
            }
        }
    }

    [YarnCommand("tutup_chat")]
    public void TutupChat()
    {
        TutupSemuaChat();
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