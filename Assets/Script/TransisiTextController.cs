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
            subjudulStr = "Terjebak Saham 'Gorengan' & Uang Panas";
            narasiStr = "<b>Tindakan FOMO Raka:</b>\n" +
                        "• <b>Herding Behavior:</b> Ikut hasutan grup pompom & teman tanpa riset.\n" +
                        "• <b>Spekulatif & Uang Panas:</b> Pakai tabungan laptop & kos untuk judi saham.\n" +
                        "• <b>Tanpa Manajemen Risiko:</b> Menolak <i>Cut Loss</i> dan nekat <i>Average Down</i>.";
            edukasiStr = "<b>Kesimpulan & Bahaya:</b>\nTabungan Rp5 juta Raka terkunci dan terus anjlok tanpa ada antrean beli hingga modalnya musnah tak bersisa.";
        }
        else if (upper.Contains("ENDING B"))
        {
            PlayerPrefs.SetInt("Ending_B_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 2 DARI 4";
            ColorUtility.TryParseHtmlString("#FDE047", out accentColor); // Kuning emas hangat
            judulStr = "BERANI CUT LOSS";
            subjudulStr = "Disiplin Membatasi Kerugian";
            narasiStr = "<b>Tindakan Raka:</b>\n" +
                        "• <b>Herding Behavior:</b> Sempat latah membeli saham pompom Rp2 juta.\n" +
                        "• <b>Cek Fundamental:</b> Sadar setelah melihat laporan keuangan yang merugi.\n" +
                        "• <b>Manajemen Risiko:</b> Disiplin <i>Cut Loss</i> sebelum harga anjlok semakin dalam.";
            edukasiStr = "<b>Kesimpulan & Bahaya:</b>\nTanpa <i>Cut Loss</i>, FOMO akan menguras seluruh modal. Rela rugi Rp300 ribu berhasil menyelamatkan Rp4,7 juta tabungan Raka.";
        }
        else if (upper.Contains("ENDING C"))
        {
            PlayerPrefs.SetInt("Ending_C_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 3 DARI 4";
            ColorUtility.TryParseHtmlString("#FDBA74", out accentColor); // Amber lembut
            judulStr = "BELI DI PUNCAK";
            subjudulStr = "Tergiur Janji Manis saat Harga Tertinggi";
            narasiStr = "<b>Tindakan FOMO Raka:</b>\n" +
                        "• <b>Buying at Peak:</b> Tergoda pamer profit Bima lalu nekat beli di pucuk (Rp135).\n" +
                        "• <b>Abaikan Fundamental:</b> Tutup mata meski tahu laporan keuangannya rugi.\n" +
                        "• <b>Spekulatif:</b> Masukin seluruh Rp5 juta demi mengejar untung instan.";
            edukasiStr = "<b>Kesimpulan & Bahaya:</b>\nBeli di puncak menjadikan Raka sasaran buangan bandar (<i>Exit Liquidity</i>). Harga anjlok ke Rp93 (<i>ARB</i>) dan modalnya nyangkut.";
        }
        else if (upper.Contains("ENDING D"))
        {
            PlayerPrefs.SetInt("Ending_D_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 4 DARI 4";
            ColorUtility.TryParseHtmlString("#86EFAC", out accentColor); // Hijau mint cerah
            judulStr = "INVESTOR BIJAK";
            subjudulStr = "Tenang & Disiplin Menjaga Tabungan";
            narasiStr = "<b>Sikap Rasional Raka:</b>\n" +
                        "• <b>Tolak Herding Behavior:</b> Tidak ikut-ikutan euforia grup & influencer.\n" +
                        "• <b>Patuhi Fundamental:</b> Cek laporan keuangan & hindari perusahaan rugi.\n" +
                        "• <b>Hindari Spekulasi:</b> Menolak beli di pucuk (Rp135) demi menjaga tabungan.";
            edukasiStr = "<b>Kesimpulan & Bahaya yang Dihindari:</b>\nDisiplin riset dan manajemen risiko menyelamatkan Raka dari jebakan <i>Pump & Dump</i>. Tabungan Rp5 juta tetap aman 100%.";
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
