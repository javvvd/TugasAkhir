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
            subjudulStr = "Terjebak Saham Gocap & Uang Panas";
            narasiStr = "<b>Poin Tindakan FOMO yang Dilakukan Raka:</b>\n" +
                        "• <b>Herding Behavior:</b> Ikut-ikutan kerumunan grup Telegram & teman tanpa riset mandiri.\n" +
                        "• <b>Spekulatif & Uang Panas:</b> Memakai seluruh tabungan laptop & biaya kos untuk berspekulasi.\n" +
                        "• <b>Tanpa Manajemen Risiko:</b> Menolak <i>Cut Loss</i> dan nekat <i>Average Down</i> saat harga anjlok.";
            edukasiStr = "<b>Kesimpulan & Bahaya:</b>\nKombinasi FOMO, uang panas, dan ketiadaan manajemen risiko membuat seluruh tabungan Rp5 juta Raka terkunci mati di saham Rp50 (<i>Gocap</i>) hingga bangkrut total.";
        }
        else if (upper.Contains("ENDING B"))
        {
            PlayerPrefs.SetInt("Ending_B_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 2 DARI 4";
            ColorUtility.TryParseHtmlString("#FDE047", out accentColor); // Kuning emas hangat
            judulStr = "BERANI CUT LOSS";
            subjudulStr = "Disiplin Membatasi Kerugian";
            narasiStr = "<b>Poin Tindakan yang Dilakukan Raka:</b>\n" +
                        "• <b>Herding Behavior & Spekulatif:</b> Awalnya tergoda ikut-ikutan membeli saham pompom Rp2 juta.\n" +
                        "• <b>Evaluasi Fundamental:</b> Menyadari kesalahan setelah mengecek laporan keuangan yang merugi.\n" +
                        "• <b>Manajemen Risiko (Cut Loss):</b> Berani memotong kerugian lebih awal sebelum harga hancur.";
            edukasiStr = "<b>Kesimpulan & Bahaya FOMO:</b>\nTanpa manajemen risiko (<i>Cut Loss</i>), FOMO akan menghancurkan modal di harga Rp50. Keberanian menerima rugi Rp300 ribu berhasil menyelamatkan Rp4,7 juta tabungan Raka.";
        }
        else if (upper.Contains("ENDING C"))
        {
            PlayerPrefs.SetInt("Ending_C_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 3 DARI 4";
            ColorUtility.TryParseHtmlString("#FDBA74", out accentColor); // Amber lembut
            judulStr = "BELI DI PUNCAK";
            subjudulStr = "Tergiur Janji Manis saat Harga Tertinggi";
            narasiStr = "<b>Poin Tindakan FOMO yang Dilakukan Raka:</b>\n" +
                        "• <b>Buying at Peak:</b> Raka tergoda pamer profit Bima sehingga nekat membeli di harga puncak (Rp135).\n" +
                        "• <b>Abaikan Fundamental:</b> Sengaja menutup mata meski sudah melihat laporan keuangan yang merugi.\n" +
                        "• <b>Spekulatif & Tanpa Manajemen Risiko:</b> Masukin seluruh Rp5 juta demi mengejar untung instan.";
            edukasiStr = "<b>Kesimpulan & Bahaya:</b>\nMembeli di puncak harga karena FOMO menjadikan Raka sasaran buangan bandar (<i>Exit Liquidity</i>). Harga langsung anjlok ke Rp93 (<i>ARB</i>) dan tabungan laptopnya nyangkut parah.";
        }
        else if (upper.Contains("ENDING D"))
        {
            PlayerPrefs.SetInt("Ending_D_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 4 DARI 4";
            ColorUtility.TryParseHtmlString("#86EFAC", out accentColor); // Hijau mint cerah
            judulStr = "INVESTOR BIJAK";
            subjudulStr = "Tenang & Disiplin Menjaga Tabungan";
            narasiStr = "<b>Poin Sikap Rasional yang Dilakukan Raka:</b>\n" +
                        "• <b>Tolak Herding Behavior:</b> Tidak latah mengikuti euforia grup pompom maupun ajakan influencer.\n" +
                        "• <b>Patuhi Fundamental:</b> Memeriksa laporan keuangan dan menghindari saham perusahaan yang rugi.\n" +
                        "• <b>Hindari Buying at Peak & Spekulasi:</b> Menolak beli di pucuk (Rp135) demi menjaga uang kuliah.";
            edukasiStr = "<b>Kesimpulan & Bahaya yang Dihindari:</b>\nDisiplin pada analisis fundamental dan manajemen risiko menyelamatkan Raka dari jebakan manipulasi <i>Pump & Dump</i>. Tabungan Rp5 juta tetap aman 100%.";
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
