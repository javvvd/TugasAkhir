using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class TransisiTextController : MonoBehaviour
{
    [Header("Referensi UI")]
    public TextMeshProUGUI teksLayarHitam;

    private GameObject panelEndingCard;

    void Awake()
    {
        InisialisasiCardEnding();
    }

    void Start()
    {
        // Menyembunyikan teks saat game baru dimulai
        if (teksLayarHitam != null)
        {
            teksLayarHitam.gameObject.SetActive(false);
        }

        if (panelEndingCard != null)
        {
            panelEndingCard.SetActive(false);
        }
    }

    private void InisialisasiCardEnding()
    {
        if (teksLayarHitam == null) return;

        // Buat panel kartu di belakang teks jika belum ada
        if (panelEndingCard == null)
        {
            Transform parent = teksLayarHitam.transform.parent;
            if (parent != null)
            {
                panelEndingCard = new GameObject("PanelEndingCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
                panelEndingCard.transform.SetParent(parent, false);
                panelEndingCard.transform.SetSiblingIndex(teksLayarHitam.transform.GetSiblingIndex());

                RectTransform rt = panelEndingCard.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(1120f, 660f);

                Image img = panelEndingCard.GetComponent<Image>();
                img.color = new Color(0.06f, 0.08f, 0.14f, 0.95f); // Dark luxury slate blue

                Outline outline = panelEndingCard.GetComponent<Outline>();
                outline.effectColor = new Color(0.25f, 0.40f, 0.65f, 0.75f);
                outline.effectDistance = new Vector2(2f, -2f);

                panelEndingCard.SetActive(false);
            }
        }
    }

    // Perintah Yarn untuk memunculkan teks custom (transisi jam/hari atau ending cerita)
    [YarnCommand("munculkan_teks")]
    public void MunculkanTeks(string kalimat)
    {
        if (teksLayarHitam == null) return;

        // Bersihkan escape newline dari yarn string
        kalimat = kalimat.Replace("\\n", "\n").Trim();

        bool isEnding = kalimat.ToUpper().Contains("ENDING ");

        if (isEnding)
        {
            if (panelEndingCard == null) InisialisasiCardEnding();
            if (panelEndingCard != null) panelEndingCard.SetActive(true);

            // Sesuaikan ukuran RectTransform teks agar pas di dalam kartu
            RectTransform rt = teksLayarHitam.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(1040f, 600f);

            teksLayarHitam.text = FormatEndingCard(kalimat);
        }
        else
        {
            if (panelEndingCard != null) panelEndingCard.SetActive(false);

            // Untuk transisi subtitle biasa (misal: "3 Jam Kemudian...")
            RectTransform rt = teksLayarHitam.rectTransform;
            rt.sizeDelta = new Vector2(1000f, 200f);
            teksLayarHitam.text = $"<align=center><size=34><b>{kalimat}</b></size></align>";
        }

        teksLayarHitam.gameObject.SetActive(true);
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

    // Format tampilan kartu ending secara estetis dan terstruktur sesuai literatur skripsi
    private string FormatEndingCard(string rawText)
    {
        string upper = rawText.ToUpper();

        string badge = "";
        string title = "";
        string subtitle = "";
        string accentColor = "#FF5252";
        string narrative = "";
        string pesanEdukasi = "";

        if (upper.Contains("ENDING A"))
        {
            badge = "<color=#FF5252><b>[ BAD ENDING - KEBANGKRUTAN ]</b></color>";
            title = "<color=#FF5252>ENDING A: KEBANGKRUTAN TOTAL</color>";
            subtitle = "<color=#90A4AE><i>Jebakan Saham Gocap & Uang Panas</i></color>";
            accentColor = "#FF5252";
            narrative = "Raka kehilangan seluruh tabungannya karena tergiur ilusi cuan instan di grup influencer tanpa analisis mandiri. Nekat menggunakan uang kos dan biaya hidup membuatnya terjebak di saham likuiditas mati (gocap Rp50) tanpa bisa ditarik kembali.";
            pesanEdukasi = "Jangan pernah berinvestasi menggunakan 'uang panas' atau dana kebutuhan pokok. Di pasar modal, keserakahan tanpa literasi finansial hanya akan berujung pada kehancuran.";
        }
        else if (upper.Contains("ENDING B"))
        {
            badge = "<color=#FFB300><b>[ REALISTIC ENDING - DISIPLIN ]</b></color>";
            title = "<color=#FFB300>ENDING B: REALITA TRADER</color>";
            subtitle = "<color=#90A4AE><i>Disiplin Manajemen Risiko (Cut Loss)</i></color>";
            accentColor = "#FFB300";
            narrative = "Raka menelan kerugian 5%, namun ia memilih berlapang dada dan segera melakukan Cut Loss sebelum penurunan semakin parah. Keputusan pahit ini berhasil menyelamatkan sebagian besar tabungan laptopnya.";
            pesanEdukasi = "Mengakui kesalahan dan membatasi risiko adalah tanda kedewasaan investor. Cut loss bukan kegagalan, melainkan benteng pertahanan terbaik dari kebangkrutan total.";
        }
        else if (upper.Contains("ENDING C"))
        {
            badge = "<color=#FF7043><b>[ TRAP ENDING - EXIT LIQUIDITY ]</b></color>";
            title = "<color=#FF7043>ENDING C: KORBAN CUCI PIRING</color>";
            subtitle = "<color=#90A4AE><i>Terjebak Skema Pump & Dump di Pucuk</i></color>";
            accentColor = "#FF7043";
            narrative = "Terpancing euforia profit di menit-menit akhir bursa, Raka membeli saham di harga tertinggi. Ia tidak sadar telah menjadi Exit Liquidity bagi para bandar yang langsung mengguyur pasar (Dump) hingga modalnya habis.";
            pesanEdukasi = "Waspadai euforia kelompok (Herding Behavior) di media sosial. Membeli saham yang sedang terbang tanpa riset hanya akan menjadikanmu tumbal para manipulator pasar.";
        }
        else if (upper.Contains("ENDING D"))
        {
            badge = "<color=#66BB6A><b>[ BEST ENDING - INVESTOR BIJAK ]</b></color>";
            title = "<color=#66BB6A>ENDING D: INVESTOR RASIONAL</color>";
            subtitle = "<color=#90A4AE><i>Kemenangan Literasi Atas Godaan FOMO</i></color>";
            accentColor = "#66BB6A";
            narrative = "Raka berhasil menahan diri dari godaan euforia sesaat dan tetap teguh pada rencana keuangannya. Seluruh tabungan laptopnya aman, sementara trader yang panik menelan kerugian parah.";
            pesanEdukasi = "Kunci sukses investasi bukanlah kecepatan mengejar keuntungan sesaat, melainkan kedisiplinan mengelola risiko dan menjaga kepala dingin di tengah hiruk-pikuk pasar.";
        }
        else
        {
            // Fallback jika ada custom ending lainnya
            return $"<align=center><size=28><b>{rawText}</b></size></align>";
        }

        return $"<align=center><size=18>{badge}</size>\n" +
               $"<size=30><b>{title}</b></size>\n" +
               $"<size=18>{subtitle}</size>\n" +
               $"<size=14><color={accentColor}>──────────────────────────────────────────────────</color></size></align>\n\n" +
               $"<align=left><size=20><color=#ECEFF1>{narrative}</color></size>\n\n" +
               $"<size=19><color=#FFD54F><b>💡 PESAN EDUKASI FINANSIAL:</b>\n" +
               $"<i>\"{pesanEdukasi}\"</i></color></size></align>\n\n" +
               $"<align=center><size=14><color=#78909C><i>(Menuju Menu Utama...)</i></color></size></align>";
    }
}