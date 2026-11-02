using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class TransisiTextController : MonoBehaviour
{
    [Header("Referensi Layar Transisi Biasa")]
    [Tooltip("Text transisi waktu / jam di atas layar hitam")]
    public TextMeshProUGUI teksLayarHitam;

    [Header("Pengaturan Efek Ketikan (Typewriter)")]
    [Tooltip("Kecepatan ketikan teks prolog (detik per karakter)")]
    public float kecepatanKetik = 0.018f;
    private Coroutine coroutineKetik;

    [Header("Referensi Gambar Panduan")]
    [Tooltip("CanvasGroup panel gambar panduan bermain")]
    public CanvasGroup gambarPanduan;
    [Tooltip("Durasi default munculnya panduan (detik)")]
    public float durasiPanduanDefault = 8f;
    private Coroutine coroutinePanduan;

    [Header("Referensi Objek Panel Ending")]
    [Tooltip("Panel utama kartu ending")]
    public GameObject panelEndingCard;
    public TextMeshProUGUI textBadge;
    public TextMeshProUGUI textJudul;
    public TextMeshProUGUI textSubjudul;
    public Image garisPemisah;
    public TextMeshProUGUI textNarasi;
    public TextMeshProUGUI textEdukasi;
    public Button tombolKembaliMenu;

    void Start()
    {
        // Menyembunyikan teks & kartu ending saat gameplay dimulai
        if (teksLayarHitam != null)
        {
            teksLayarHitam.maxVisibleCharacters = int.MaxValue;
            teksLayarHitam.gameObject.SetActive(false);
        }

        if (panelEndingCard != null)
        {
            panelEndingCard.SetActive(false);
        }

        if (gambarPanduan != null)
        {
            gambarPanduan.alpha = 0f;
            gambarPanduan.blocksRaycasts = false;
            gambarPanduan.gameObject.SetActive(false);
        }

        if (tombolKembaliMenu != null)
        {
            tombolKembaliMenu.onClick.RemoveAllListeners();
            tombolKembaliMenu.onClick.AddListener(KembaliKeMenu);
        }
    }

    void Update()
    {
        // Kontrol input saat Panel Ending Card sedang aktif
        if (panelEndingCard != null && panelEndingCard.activeInHierarchy)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }

            // Failsafe 1: Deteksi klik langsung (mouse click / mobile touch) pada area Tombol Kembali
            if (Input.GetMouseButtonDown(0) && tombolKembaliMenu != null)
            {
                RectTransform btnRt = tombolKembaliMenu.GetComponent<RectTransform>();
                if (btnRt != null && RectTransformUtility.RectangleContainsScreenPoint(btnRt, Input.mousePosition, null))
                {
                    KembaliKeMenu();
                    return;
                }
            }

            // Failsafe 2: Shortcut keyboard (Spasi, Enter, atau Escape)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape))
            {
                KembaliKeMenu();
            }
        }
    }

    // Perintah Yarn untuk memunculkan teks custom (transisi jam/hari atau ending cerita)
    [YarnCommand("munculkan_teks")]
    public void MunculkanTeks(string kalimat)
    {
        // Pastikan panduan selesai/tersembunyi jika teks mulai muncul
        HentikanPanduan();

        // Bersihkan escape newline dari yarn string
        kalimat = kalimat.Replace("\\n", "\n").Trim();

        // Hentikan efek ketik sebelumnya jika masih berjalan
        if (coroutineKetik != null)
        {
            StopCoroutine(coroutineKetik);
            coroutineKetik = null;
        }

        bool isEnding = kalimat.ToUpper().Contains("ENDING ");

        if (isEnding)
        {
            // Sembunyikan teks subtitle biasa
            if (teksLayarHitam != null)
            {
                teksLayarHitam.maxVisibleCharacters = int.MaxValue;
                teksLayarHitam.gameObject.SetActive(false);
            }

            // Tampilkan kartu ending fisik
            TampilkanEndingCard(kalimat);
        }
        else
        {
            // Sembunyikan panel ending jika sebelumnya aktif
            if (panelEndingCard != null) panelEndingCard.SetActive(false);

            // Untuk transisi subtitle biasa atau prolog cerita
            if (teksLayarHitam != null)
            {
                RectTransform rt = teksLayarHitam.rectTransform;
                if (kalimat.Length > 60 || kalimat.Contains("\n"))
                {
                    // Tampilan paragraf prolog (lega, ukuran font nyaman, jarak baris pas)
                    rt.sizeDelta = new Vector2(1100f, 480f);
                    teksLayarHitam.text = $"<align=center><line-height=150%><size=24>{kalimat}</size></line-height></align>";

                    // Jalankan efek ketikan tanpa alokasi memori (Zero GC Alloc)
                    coroutineKetik = StartCoroutine(ProsesKetikProlog(kecepatanKetik));
                }
                else
                {
                    // Tampilan judul transisi waktu pendek (misal: "3 Jam Kemudian...")
                    rt.sizeDelta = new Vector2(1000f, 200f);
                    teksLayarHitam.text = $"<align=center><size=34><b>{kalimat}</b></size></align>";
                    teksLayarHitam.maxVisibleCharacters = int.MaxValue;
                    teksLayarHitam.gameObject.SetActive(true);
                }
            }
        }
    }

    // Coroutine efisien (Zero GC Allocation & Frame-rate Independent) untuk efek ketikan
    private System.Collections.IEnumerator ProsesKetikProlog(float delayPerChar)
    {
        if (teksLayarHitam == null) yield break;

        teksLayarHitam.gameObject.SetActive(true);
        teksLayarHitam.maxVisibleCharacters = 0;
        teksLayarHitam.ForceMeshUpdate();

        TMP_TextInfo textInfo = teksLayarHitam.textInfo;
        int totalCharacters = textInfo.characterCount;
        int currentChar = 0;
        float timer = 0f;
        float currentDelay = delayPerChar;

        // Tunggu 1 frame awal agar input klik dari scene/tombol sebelumnya tidak memicu skip
        yield return null;

        while (currentChar < totalCharacters)
        {
            // Fitur skip: jika pemain klik layar / tekan Spasi saat mengetik, langsung munculkan penuh
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }

            timer += Time.deltaTime;
            while (timer >= currentDelay && currentChar < totalCharacters)
            {
                timer -= currentDelay;
                currentChar++;
                teksLayarHitam.maxVisibleCharacters = currentChar;

                // Variasi jeda alami untuk tanda baca
                currentDelay = delayPerChar;
                if (currentChar - 1 < textInfo.characterInfo.Length)
                {
                    char c = textInfo.characterInfo[currentChar - 1].character;
                    if (c == '.' || c == '!' || c == '?')
                    {
                        currentDelay = delayPerChar * 4f; // Jeda akhir kalimat
                    }
                    else if (c == ',' || c == ';')
                    {
                        currentDelay = delayPerChar * 2.5f; // Jeda koma
                    }
                    else if (c == '\n')
                    {
                        currentDelay = delayPerChar * 3f; // Jeda baris baru
                    }
                }
            }

            yield return null;
        }

        teksLayarHitam.maxVisibleCharacters = int.MaxValue;
        coroutineKetik = null;
    }

    // Perintah Yarn untuk menyembunyikan teks kembali
    [YarnCommand("hapus_teks")]
    public void HapusTeks()
    {
        HentikanPanduan();

        if (coroutineKetik != null)
        {
            StopCoroutine(coroutineKetik);
            coroutineKetik = null;
        }

        if (teksLayarHitam != null)
        {
            teksLayarHitam.maxVisibleCharacters = int.MaxValue;
            teksLayarHitam.gameObject.SetActive(false);
        }

        if (panelEndingCard != null)
        {
            panelEndingCard.SetActive(false);
        }
    }

    // Perintah Yarn untuk menampilkan gambar panduan sebelum prolog
    [YarnCommand("tampilkan_panduan")]
    public void TampilkanPanduan(float durasi = 8f)
    {
        if (gambarPanduan == null) return;
        if (coroutinePanduan != null) StopCoroutine(coroutinePanduan);
        coroutinePanduan = StartCoroutine(ProsesTampilkanPanduan(durasi));
    }

    private System.Collections.IEnumerator ProsesTampilkanPanduan(float totalDurasi)
    {
        if (gambarPanduan == null) yield break;

        float durasiFadeIn = 1.0f;
        float durasiFadeOut = 1.0f;
        float holdDurasi = Mathf.Max(0.5f, totalDurasi - durasiFadeIn - durasiFadeOut);

        gambarPanduan.alpha = 0f;
        gambarPanduan.blocksRaycasts = true;
        gambarPanduan.gameObject.SetActive(true);

        // 1. Fade In
        float waktu = 0f;
        while (waktu < durasiFadeIn)
        {
            waktu += Time.deltaTime;
            gambarPanduan.alpha = Mathf.Clamp01(waktu / durasiFadeIn);
            yield return null;
        }
        gambarPanduan.alpha = 1f;

        // 2. Durasi Tampil (Dapat di-skip jika pemain menekan klik / Spasi)
        float timerHold = 0f;
        yield return new WaitForSeconds(0.2f);
        timerHold += 0.2f;

        while (timerHold < holdDurasi)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                break;
            }
            timerHold += Time.deltaTime;
            yield return null;
        }

        // 3. Fade Out
        waktu = 0f;
        while (waktu < durasiFadeOut)
        {
            waktu += Time.deltaTime;
            gambarPanduan.alpha = Mathf.Clamp01(1f - (waktu / durasiFadeOut));
            yield return null;
        }

        gambarPanduan.alpha = 0f;
        gambarPanduan.blocksRaycasts = false;
        gambarPanduan.gameObject.SetActive(false);
        coroutinePanduan = null;
    }

    private void HentikanPanduan()
    {
        if (coroutinePanduan != null)
        {
            StopCoroutine(coroutinePanduan);
            coroutinePanduan = null;
        }
        if (gambarPanduan != null)
        {
            gambarPanduan.alpha = 0f;
            gambarPanduan.blocksRaycasts = false;
            gambarPanduan.gameObject.SetActive(false);
        }
    }

    // Menampilkan data ending ke objek-objek UI terpisah
    private void TampilkanEndingCard(string rawText)
    {
        if (panelEndingCard == null) return;

        string upper = rawText.ToUpper();

        string badgeStr = "";
        string judulStr = "";
        string subjudulStr = "";
        string narasiStr = "";
        string edukasiStr = "";
        Color accentColor = new Color(0.99f, 0.94f, 0.54f); // Default warm gold (#FEF08A)

        if (upper.Contains("ENDING A"))
        {
            PlayerPrefs.SetInt("Ending_A_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 1 DARI 4";
            ColorUtility.TryParseHtmlString("#FCA5A5", out accentColor); // Coral lembut
            judulStr = "KEBANGKRUTAN TOTAL";
            subjudulStr = "Uang Terjebak";
            narasiStr = "<b>Evaluasi:</b>\n" +
                        "• <b>Herding Behavior & FOMO:</b> Kamu terbuai ajakan teman dan euforia grup tanpa melakukan riset mandiri.\n" +
                        "• <b>Spekulasi Uang Panas:</b> Kamu mempertaruhkan uang kos dan tabungan laptop impianmu ke saham gorengan.\n" +
                        "• <b>Tanpa Manajemen Risiko:</b> Egumu menolak <i>Cut Loss</i> dan nekat <i>Average Down</i> saat harga terus ambruk.";
            edukasiStr = "<b>Bahaya FOMO dalam Investasi:</b>\nTabunganmu Rp5.000.000 musnah tak bersisa. FOMO melumpuhkan logika dan mengubah investasimu menjadi judi spekulatif yang berujung kebangkrutan.";
        }
        else if (upper.Contains("ENDING B"))
        {
            PlayerPrefs.SetInt("Ending_B_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 2 DARI 4";
            ColorUtility.TryParseHtmlString("#FDE047", out accentColor); // Kuning emas hangat
            judulStr = "BERANI CUT LOSS";
            subjudulStr = "Disiplin Membatasi Kerugian";
            narasiStr = "<b>Evaluasi Keputusanmu:</b>\n" +
                        "• <b>Sadar dari Herding Behavior:</b> Kamu sempat terbawa FOMO membeli saham pom-pom Rp2 juta, namun segera sadar dari laporan keuangannya.\n" +
                        "• <b>Disiplin Manajemen Risiko:</b> Kamu berani mengakui kesalahan dan disiplin <i>Cut Loss</i> sebelum harga anjlok lebih parah.\n" +
                        "• <b>Tolak Spekulasi:</b> Kamu menolak godaan <i>Average Down</i> yang berisiko menenggelamkan sisa modalmu.";
            edukasiStr = "<b>Bahaya FOMO dalam Investasi:</b>\nTanpa keberanian <i>Cut Loss</i>, siklus FOMO akan menelan seluruh modalmu. Rela rugi Rp300 ribu berhasil menyelamatkan sisa Rp4.700.000 tabunganmu.";
        }
        else if (upper.Contains("ENDING C"))
        {
            PlayerPrefs.SetInt("Ending_C_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 3 DARI 4";
            ColorUtility.TryParseHtmlString("#FDBA74", out accentColor); // Amber lembut
            judulStr = "BELI DI PUNCAK";
            subjudulStr = "Korban Euforia & Pintu Keluar Bandar";
            narasiStr = "<b>Evaluasi Keputusanmu:</b>\n" +
                        "• <b>Buying at Peak (FOMO):</b> Rasa takut tertinggal melihat pamer profit membuatmu nekat membeli di harga tertinggi (Rp135).\n" +
                        "• <b>Abaikan Fundamental:</b> Kamu menutup mata terhadap kondisi riil perusahaan demi mengejar keuntungan instan.\n" +
                        "• <b>Exit Liquidity Bandar:</b> Pembelian impulsifmu dijadikan alat buangan bandar untuk merealisasikan keuntungan (*Pump and Dump*).";
            edukasiStr = "<b>Bahaya FOMO dalam Investasi:</b>\nModalmu Rp5.000.000 terkunci di batas <i>Auto Rejection Bawah (ARB)</i> Rp93 tanpa ada antrean beli. Membeli karena FOMO hanya menjadikanmu tumbal pasar.";
        }
        else if (upper.Contains("ENDING D"))
        {
            PlayerPrefs.SetInt("Ending_D_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 4 DARI 4";
            ColorUtility.TryParseHtmlString("#86EFAC", out accentColor); // Hijau mint cerah
            judulStr = "INVESTOR BIJAK";
            subjudulStr = "Menang Melawan Jebakan Emosi";
            narasiStr = "<b>Evaluasi Keputusanmu:</b>\n" +
                        "• <b>Kendalikan FOMO:</b> Kamu berhasil menahan nafsu serakah dan tidak tergiur pamer profit sesaat di grup saham.\n" +
                        "• <b>Patuhi Fundamental:</b> Kamu mengutamakan data kinerja keuangan perusahaan di atas rumor dan bisikan spekulatif.\n" +
                        "• <b>Disiplin Finansial:</b> Kamu memegang teguh komitmen menjaga tabungan laptop demi masa depan kuliahmu.";
            edukasiStr = "<b>Bahaya FOMO yang Dihindari:</b>\nKedisiplinanmu menyelamatkan modal Rp5.000.000 tetap utuh 100% dari jebakan <i>Pump and Dump</i>. Kemenangan sejati investor adalah mampu mengendalikan emosinya sendiri.";
        }
        else
        {
            badgeStr = "ENDING CERITA";
            judulStr = rawText;
            subjudulStr = "";
            narasiStr = "";
            edukasiStr = "";
        }

        // Terapkan ke masing-masing objek UI
        if (textBadge != null)
        {
            textBadge.text = badgeStr;
            textBadge.color = accentColor;
        }

        if (textJudul != null)
        {
            textJudul.text = judulStr;
            textJudul.color = Color.white;
        }

        if (textSubjudul != null)
        {
            textSubjudul.text = subjudulStr;
            textSubjudul.color = accentColor;
        }

        if (garisPemisah != null)
        {
            garisPemisah.gameObject.SetActive(true);
            garisPemisah.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.3f);
        }

        if (textNarasi != null)
        {
            textNarasi.text = $"<line-height=135%>{narasiStr}</line-height>";
        }

        if (textEdukasi != null)
        {
            textEdukasi.text = $"<line-height=130%>{edukasiStr}</line-height>";
        }

        PauseManager.Instance?.SembunyikanTombolPause();
        AudioManager.Instance?.PutarBGM("calm");

        // Nonaktifkan raycast pada Dialogue System dan CanvasHP agar tidak menutupi tombol ending
        GameObject dsGo = GameObject.Find("Dialogue System");
        if (dsGo != null)
        {
            foreach (var cg in dsGo.GetComponentsInChildren<CanvasGroup>(true))
            {
                cg.blocksRaycasts = false;
            }
        }

        GameObject hpGo = GameObject.Find("CanvasHP");
        if (hpGo != null)
        {
            foreach (var cg in hpGo.GetComponentsInChildren<CanvasGroup>(true))
            {
                cg.blocksRaycasts = false;
            }
        }

        if (tombolKembaliMenu != null)
        {
            tombolKembaliMenu.interactable = true;
            tombolKembaliMenu.onClick.RemoveAllListeners();
            tombolKembaliMenu.onClick.AddListener(KembaliKeMenu);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panelEndingCard.SetActive(true);
    }

    public void KembaliKeMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
