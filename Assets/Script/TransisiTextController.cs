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
        Color badgeColor = new Color(1f, 0.32f, 0.32f); // Default red

        if (upper.Contains("ENDING A"))
        {
            PlayerPrefs.SetInt("Ending_A_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "[ BAD ENDING - KEBANGKRUTAN ]";
            badgeColor = new Color(1f, 0.32f, 0.32f); // Merah (#FF5252)
            judulStr = "ENDING A: KEBANGKRUTAN TOTAL";
            subjudulStr = "Jebakan Saham Gocap & Uang Panas";
            narasiStr = "Raka kehilangan seluruh tabungannya karena tergiur ilusi cuan instan di grup influencer tanpa analisis mandiri. Nekat menggunakan uang kos dan biaya hidup membuatnya terjebak di saham likuiditas mati (gocap Rp50) tanpa bisa ditarik kembali.";
            edukasiStr = "Jangan pernah berinvestasi menggunakan 'uang panas' atau dana kebutuhan pokok. Di pasar modal, keserakahan tanpa literasi finansial hanya akan berujung pada kehancuran.";
        }
        else if (upper.Contains("ENDING B"))
        {
            PlayerPrefs.SetInt("Ending_B_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "[ REALISTIC ENDING - DISIPLIN ]";
            badgeColor = new Color(1f, 0.70f, 0f); // Kuning (#FFB300)
            judulStr = "ENDING B: REALITA TRADER";
            subjudulStr = "Disiplin Manajemen Risiko (Cut Loss)";
            narasiStr = "Raka menelan kerugian 5%, namun ia memilih berlapang dada dan segera melakukan Cut Loss sebelum penurunan semakin parah. Keputusan pahit ini berhasil menyelamatkan sebagian besar tabungan laptopnya.";
            edukasiStr = "Mengakui kesalahan dan membatasi risiko adalah tanda kedewasaan investor. Cut loss bukan kegagalan, melainkan benteng pertahanan terbaik dari kebangkrutan total.";
        }
        else if (upper.Contains("ENDING C"))
        {
            PlayerPrefs.SetInt("Ending_C_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "[ TRAP ENDING - EXIT LIQUIDITY ]";
            badgeColor = new Color(1f, 0.44f, 0.26f); // Oranye (#FF7043)
            judulStr = "ENDING C: KORBAN CUCI PIRING";
            subjudulStr = "Terjebak Skema Pump & Dump di Pucuk";
            narasiStr = "Terpancing euforia profit di menit-menit akhir bursa, Raka membeli saham di harga tertinggi. Ia tidak sadar telah menjadi Exit Liquidity bagi para bandar yang langsung mengguyur pasar (Dump) hingga modalnya habis.";
            edukasiStr = "Waspadai euforia kelompok (Herding Behavior) di media sosial. Membeli saham yang sedang terbang tanpa riset hanya akan menjadikanmu tumbal para manipulator pasar.";
        }
        else if (upper.Contains("ENDING D"))
        {
            PlayerPrefs.SetInt("Ending_D_Unlocked", 1);
            PlayerPrefs.Save();

            badgeStr = "[ BEST ENDING - INVESTOR BIJAK ]";
            badgeColor = new Color(0.40f, 0.73f, 0.42f); // Hijau (#66BB6A)
            judulStr = "ENDING D: INVESTOR RASIONAL";
            subjudulStr = "Kemenangan Literasi Atas Godaan FOMO";
            narasiStr = "Raka berhasil menahan diri dari godaan euforia sesaat dan tetap teguh pada rencana keuangannya. Seluruh tabungan laptopnya aman, sementara trader yang panik menelan kerugian parah.";
            edukasiStr = "Kunci sukses investasi bukanlah kecepatan mengejar keuntungan sesaat, melainkan kedisiplinan mengelola risiko dan menjaga kepala dingin di tengah hiruk-pikuk pasar.";
        }
        else
        {
            badgeStr = "[ ENDING ]";
            badgeColor = Color.white;
            judulStr = rawText;
            subjudulStr = "";
            narasiStr = "";
            edukasiStr = "";
        }

        // Terapkan ke masing-masing objek UI
        if (textBadge != null)
        {
            textBadge.text = badgeStr;
            textBadge.color = badgeColor;
        }

        if (textJudul != null)
        {
            textJudul.text = judulStr;
            textJudul.color = badgeColor;
        }

        if (textSubjudul != null)
        {
            textSubjudul.text = subjudulStr;
        }

        if (garisPemisah != null)
        {
            garisPemisah.color = new Color(badgeColor.r, badgeColor.g, badgeColor.b, 0.5f);
        }

        if (textNarasi != null)
        {
            textNarasi.text = narasiStr;
        }

        if (textEdukasi != null)
        {
            textEdukasi.text = $"<b>PESAN EDUKASI FINANSIAL:</b>\n<i>\"{edukasiStr}\"</i>";
        }

        panelEndingCard.SetActive(true);
    }

    public void KembaliKeMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}