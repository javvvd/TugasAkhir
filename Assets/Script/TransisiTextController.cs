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

        bool isEnding = kalimat.ToUpper().Contains("ENDING ");

        if (isEnding)
        {
            // Sembunyikan teks subtitle biasa
            if (teksLayarHitam != null) teksLayarHitam.gameObject.SetActive(false);

            // Tampilkan kartu ending fisik
            TampilkanEndingCard(kalimat);
        }
        else
        {
            // Sembunyikan panel ending jika sebelumnya aktif
            if (panelEndingCard != null) panelEndingCard.SetActive(false);

            // Untuk transisi subtitle biasa (misal: "3 Jam Kemudian...")
            if (teksLayarHitam != null)
            {
                RectTransform rt = teksLayarHitam.rectTransform;
                rt.sizeDelta = new Vector2(1000f, 200f);
                teksLayarHitam.text = $"<align=center><size=34><b>{kalimat}</b></size></align>";
                teksLayarHitam.gameObject.SetActive(true);
            }
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
            narasiStr = "Raka kehilangan seluruh tabungannya, termasuk uang kos dan biaya hidup, karena nekat membeli saham gorengan yang akhirnya mati di harga Rp50.";
            edukasiStr = "Jangan pernah berinvestasi menggunakan uang kebutuhan pokok (uang panas).";
        }
        else if (upper.Contains("ENDING B"))
        {
            PlayerPrefs.SetInt("Ending_B_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 2 DARI 4";
            ColorUtility.TryParseHtmlString("#FDE047", out accentColor); // Kuning emas hangat
            judulStr = "BERANI CUT LOSS";
            subjudulStr = "Disiplin Membatasi Kerugian";
            narasiStr = "Meski sempat rugi sedikit karena ikut-ikutan membeli saham, Raka sadar tepat waktu dan segera menjual sahamnya sebelum harga anjlok lebih dalam.";
            edukasiStr = "Membatasi kerugian (cut loss) bukan kegagalan, melainkan cara melindungi modal dari kehancuran.";
        }
        else if (upper.Contains("ENDING C"))
        {
            PlayerPrefs.SetInt("Ending_C_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 3 DARI 4";
            ColorUtility.TryParseHtmlString("#FDBA74", out accentColor); // Amber lembut
            judulStr = "TERJEBAK DI PUCUK";
            subjudulStr = "Tergiur Janji Manis saat Harga Tinggi";
            narasiStr = "Raka awalnya sudah berhati-hati, namun akhirnya goyah saat melihat harga saham terbang tinggi. Begitu ia membeli di harga pucuk, harga langsung anjlok drastis.";
            edukasiStr = "Jangan membeli saham hanya karena takut ketinggalan (FOMO) saat harganya sudah naik tinggi.";
        }
        else if (upper.Contains("ENDING D"))
        {
            PlayerPrefs.SetInt("Ending_D_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "ENDING 4 DARI 4";
            ColorUtility.TryParseHtmlString("#86EFAC", out accentColor); // Hijau mint cerah
            judulStr = "INVESTOR BIJAK";
            subjudulStr = "Tenang & Disiplin Menjaga Tabungan";
            narasiStr = "Raka berhasil menahan diri dari ajakan grup maupun godaan keuntungan instan. Tabungan laptopnya tetap utuh dan aman hingga akhir.";
            edukasiStr = "Kunci utama mengelola keuangan adalah kepala dingin, riset mandiri, dan disiplin pada tujuan.";
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
            garisPemisah.gameObject.SetActive(false);
        }

        if (textNarasi != null)
        {
            textNarasi.text = narasiStr;
        }

        if (textEdukasi != null)
        {
            textEdukasi.text = $"<b>Pelajaran Penting:</b>\n{edukasiStr}";
        }

        panelEndingCard.SetActive(true);
    }

    public void KembaliKeMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
