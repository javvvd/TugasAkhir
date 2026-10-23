using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class KamusManager : MonoBehaviour
{
    public static KamusManager Instance { get; private set; }

    [System.Serializable]
    public class IstilahSaham
    {
        public string judul;
        public string kategori;
        public string warnaKategoriHex;
        [TextArea(2, 4)] public string definisi;
        [TextArea(2, 4)] public string edukasiSkripsi;

        public IstilahSaham(string j, string k, string hex, string def, string edu)
        {
            judul = j;
            kategori = k;
            warnaKategoriHex = hex;
            definisi = def;
            edukasiSkripsi = edu;
        }
    }

    [Header("Referensi UI (Opsional jika dibuat otomatis)")]
    public GameObject tombolBukaKamus;
    public GameObject panelKamusModal;

    [Header("Daftar Istilah Edukasi Saham")]
    public List<IstilahSaham> daftarIstilah = new List<IstilahSaham>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInit()
    {
        // Hanya inisialisasi jika ada Canvas UI atau BoxPorto di scene yang sedang aktif
        GameObject boxPorto = GameObject.Find("BoxPorto");
        if (boxPorto != null && Instance == null && FindFirstObjectByType<KamusManager>() == null)
        {
            GameObject kmGo = new GameObject("KamusManager", typeof(KamusManager));
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;

        InisialisasiDaftarIstilah();
        BangunUIKamusJikaBelumAda();
    }

    private void InisialisasiDaftarIstilah()
    {
        if (daftarIstilah.Count > 0) return;

        daftarIstilah.Add(new IstilahSaham(
            "FOMO (Fear of Missing Out)",
            "BIAS PSIKOLOGIS",
            "#EF4444",
            "Kecemasan dan ketakutan emosional merasa tertinggal keuntungan besar yang sedang dinikmati orang lain. Di pasar modal, investor terdorong membeli secara impulsif dan terburu-buru tanpa analisa rasional.",
            "Bahaya: Mendorong investor pemula masuk di saat harga sudah terlalu mahal (pucuk) hanya karena takut kehilangan kesempatan (loss aversion)."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Skema Pump and Dump",
            "MANIPULASI PASAR",
            "#F97316",
            "Praktik manipulasi ilegal di mana sekelompok manipulator (bandar/influencer) memborong saham murah, menyebarkan promosi berlebihan agar ritel ikut membeli (Pump), lalu serempak menjual seluruh sahamnya (Dump) saat harga naik tinggi.",
            "Bahaya: Ritel yang termakan euforia promosi akan menanggung kerugian telak saat harga mendadak jatuh bebas tanpa pembeli."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Exit Liquidity (Korban Cuci Piring)",
            "JEBAKAN BANDAR",
            "#DC2626",
            "Sebutan bagi investor ritel awam yang membeli saham di harga puncak. Uang mereka menjadi 'pintu keluar' (sarana likuiditas) bagi bandar untuk merealisasikan profit besar, meninggalkan ritel dengan aset yang anjlok drastis.",
            "Edukasi: Selalu waspada saat ada ajakan membeli saham yang sedang terbang tinggi di media sosial."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Saham Gocap (Rp 50)",
            "REGULASI & LIKUIDITAS",
            "#EAB308",
            "Batas harga terendah suatu saham di Pasar Reguler Bursa Efek Indonesia (Rp 50 per lembar). Saham di batas ini umumnya kehilangan likuiditas (tidak ada antrean beli sama sekali).",
            "Bahaya: Modal investor terkunci mati dan tidak bisa dicairkan kembali karena tidak ada investor lain yang mau membeli."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "ARB (Auto Rejection Bawah)",
            "MEKANISME BURSA",
            "#F59E0B",
            "Batas persentase penurunan harga harian maksimal yang diizinkan bursa. Saat saham ARB beruntun, jutaan lot antrean jual macet total karena tidak ada pembeli yang menampung.",
            "Edukasi: Jangan pernah mengabaikan volatilitas ekstrem pada saham spekulatif tanpa fundamental riil."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Cut Loss (Manajemen Risiko)",
            "DISIPLIN TRADING",
            "#3B82F6",
            "Tindakan menjual saham dalam keadaan rugi terkontrol (misal rugi 3%–5%) demi mencegah penurunan yang jauh lebih parah dan menyelamatkan sisa modal utama.",
            "Edukasi: Mengakui kesalahan analisis dan melakukan cut loss bukanlah kegagalan, melainkan benteng pertahanan terbaik dari kebangkrutan."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Uang Dingin vs Uang Panas",
            "PERENCANAAN FINANSIAL",
            "#10B981",
            "Uang Dingin adalah dana menganggur yang jika nilainya turun tidak mengganggu kebutuhan hidup. Uang Panas adalah dana kebutuhan primer (uang kos, makan, kuliah) atau dana utang yang pantang digunakan untuk investasi fluktuatif.",
            "Edukasi: Jangan pernah mempertaruhkan biaya hidup sehari-hari ke dalam pasar saham berisiko tinggi!"
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Herding Behavior (Latah Kerumunan)",
            "PERILAKU PASAR",
            "#8B5CF6",
            "Bias kognitif di mana seseorang meniru tindakan kelompok atau figur publik tanpa melakukan riset mandiri. Fenomena ini mempercepat terbentuknya gelembung spekulatif (market bubble).",
            "Edukasi: Keberhasilan investasi membutuhkan analisis independen, bukan sekadar mengikuti tren grup obrolan."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Analisis Fundamental",
            "METODE RISET",
            "#06B6D4",
            "Metode analisis yang mempelajari kesehatan bisnis perusahaan: pendapatan, laba, utang, dan model bisnis jangka panjang, bukan sekadar melihat rumor atau fluktuasi grafik sesaat.",
            "Edukasi: Fondasi perusahaan yang kuat melindungi nilai investasi dari jebakan saham spekulatif gorengan."
        ));

        daftarIstilah.Add(new IstilahSaham(
            "Toleransi Risiko & Diversifikasi",
            "STRATEGI INVESTASI",
            "#14B8A6",
            "Memahami batas toleransi psikologis terhadap potensi kerugian dan menyebar alokasi modal ke beberapa instrumen yang aman, tidak menempatkan seluruh uang dalam satu aset (all-in).",
            "Edukasi: 'Jangan menaruh semua telur dalam satu keranjang' adalah aturan emas dalam mengamankan portofolio."
        ));
    }

    private void BangunUIKamusJikaBelumAda()
    {
        // 1. Cari Canvas UI utama
        Canvas canvasUtama = null;
        GameObject uiGo = GameObject.Find("UI");
        if (uiGo != null) canvasUtama = uiGo.GetComponent<Canvas>();
        if (canvasUtama == null) canvasUtama = FindFirstObjectByType<Canvas>();
        if (canvasUtama == null) return;

        Transform canvasTransform = canvasUtama.transform;

        // 2. Buat Tombol Buka Kamus jika belum ada
        if (tombolBukaKamus == null)
        {
            Transform existingBtn = canvasTransform.Find("TombolKamusSaham");
            if (existingBtn != null)
            {
                tombolBukaKamus = existingBtn.gameObject;
            }
            else
            {
                tombolBukaKamus = BuatTombolKamus(canvasTransform);
            }
        }

        // 3. Buat Panel Kamus Modal jika belum ada
        if (panelKamusModal == null)
        {
            Transform existingModal = canvasTransform.Find("PanelKamusModal");
            if (existingModal != null)
            {
                panelKamusModal = existingModal.gameObject;
            }
            else
            {
                panelKamusModal = BuatModalKamus(canvasTransform);
            }
        }

        if (panelKamusModal != null)
        {
            panelKamusModal.SetActive(false);
        }
    }

    private GameObject BuatTombolKamus(Transform parent)
    {
        GameObject btnGo = new GameObject("TombolKamusSaham", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(Outline));
        btnGo.transform.SetParent(parent, false);

        RectTransform rt = btnGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        // Tepat di sebelah kanan BoxPorto (lebar BoxPorto 423)
        rt.anchoredPosition = new Vector2(435f, -22f);
        rt.sizeDelta = new Vector2(155f, 48f);

        Image img = btnGo.GetComponent<Image>();
        img.color = new Color(0.10f, 0.15f, 0.24f, 0.95f); // Dark luxury slate

        Outline outline = btnGo.GetComponent<Outline>();
        outline.effectColor = new Color(0.22f, 0.65f, 0.95f, 0.85f); // Cyan accent border
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        Button btn = btnGo.GetComponent<Button>();
        btn.onClick.AddListener(BukaKamus);

        // Teks di dalam tombol
        GameObject txtGo = new GameObject("TeksLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtGo.transform.SetParent(btnGo.transform, false);

        RectTransform txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = txtGo.GetComponent<TextMeshProUGUI>();
        tmp.text = "📖 Kamus Saham";
        tmp.fontSize = 17;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return btnGo;
    }

    private GameObject BuatModalKamus(Transform parent)
    {
        // 1. Backdrop Gelap (penutup layar penuh)
        GameObject backdrop = new GameObject("PanelKamusModal", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backdrop.transform.SetParent(parent, false);

        RectTransform rtBackdrop = backdrop.GetComponent<RectTransform>();
        rtBackdrop.anchorMin = Vector2.zero;
        rtBackdrop.anchorMax = Vector2.one;
        rtBackdrop.sizeDelta = Vector2.zero;

        Image imgBackdrop = backdrop.GetComponent<Image>();
        imgBackdrop.color = new Color(0f, 0f, 0f, 0.82f); // Gelap transparan

        // 2. Kartu Jendela Modal
        GameObject cardGo = new GameObject("CardKamus", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
        cardGo.transform.SetParent(backdrop.transform, false);

        RectTransform rtCard = cardGo.GetComponent<RectTransform>();
        rtCard.anchorMin = new Vector2(0.5f, 0.5f);
        rtCard.anchorMax = new Vector2(0.5f, 0.5f);
        rtCard.pivot = new Vector2(0.5f, 0.5f);
        rtCard.anchoredPosition = Vector2.zero;
        rtCard.sizeDelta = new Vector2(1060f, 660f);

        Image imgCard = cardGo.GetComponent<Image>();
        imgCard.color = new Color(0.07f, 0.10f, 0.17f, 0.98f); // Dark Slate Blue

        Outline cardOutline = cardGo.GetComponent<Outline>();
        cardOutline.effectColor = new Color(0.25f, 0.40f, 0.65f, 0.8f);
        cardOutline.effectDistance = new Vector2(2f, -2f);

        // 3. Header Bar
        GameObject headerGo = new GameObject("HeaderBar", typeof(RectTransform));
        headerGo.transform.SetParent(cardGo.transform, false);
        RectTransform rtHeader = headerGo.GetComponent<RectTransform>();
        rtHeader.anchorMin = new Vector2(0f, 1f);
        rtHeader.anchorMax = new Vector2(1f, 1f);
        rtHeader.pivot = new Vector2(0.5f, 1f);
        rtHeader.anchoredPosition = new Vector2(0f, -15f);
        rtHeader.sizeDelta = new Vector2(-40f, 75f);

        // Judul Header
        GameObject titleGo = new GameObject("JudulHeader", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleGo.transform.SetParent(headerGo.transform, false);
        RectTransform rtTitle = titleGo.GetComponent<RectTransform>();
        rtTitle.anchorMin = new Vector2(0f, 0.4f);
        rtTitle.anchorMax = new Vector2(1f, 1f);
        rtTitle.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmpTitle = titleGo.GetComponent<TextMeshProUGUI>();
        tmpTitle.text = "📖 GLOSARIUM & KAMUS PASAR MODAL";
        tmpTitle.fontSize = 24;
        tmpTitle.fontStyle = FontStyles.Bold;
        tmpTitle.color = new Color(0.96f, 0.98f, 1f);
        tmpTitle.alignment = TextAlignmentOptions.Left;

        // Subjudul Header
        GameObject subGo = new GameObject("SubjudulHeader", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        subGo.transform.SetParent(headerGo.transform, false);
        RectTransform rtSub = subGo.GetComponent<RectTransform>();
        rtSub.anchorMin = new Vector2(0f, 0f);
        rtSub.anchorMax = new Vector2(1f, 0.45f);
        rtSub.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmpSub = subGo.GetComponent<TextMeshProUGUI>();
        tmpSub.text = "Panduan edukasi literasi finansial untuk mengenali bahaya FOMO dan skema manipulasi pasar.";
        tmpSub.fontSize = 15;
        tmpSub.color = new Color(0.60f, 0.70f, 0.82f);
        tmpSub.alignment = TextAlignmentOptions.Left;

        // Tombol Close (X) di pojok kanan atas
        GameObject closeBtnGo = new GameObject("TombolTutupX", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        closeBtnGo.transform.SetParent(headerGo.transform, false);
        RectTransform rtClose = closeBtnGo.GetComponent<RectTransform>();
        rtClose.anchorMin = new Vector2(1f, 0.5f);
        rtClose.anchorMax = new Vector2(1f, 0.5f);
        rtClose.pivot = new Vector2(1f, 0.5f);
        rtClose.anchoredPosition = new Vector2(0f, 5f);
        rtClose.sizeDelta = new Vector2(44f, 44f);

        Image imgClose = closeBtnGo.GetComponent<Image>();
        imgClose.color = new Color(0.85f, 0.20f, 0.25f, 0.9f); // Merah tegas

        Button btnClose = closeBtnGo.GetComponent<Button>();
        btnClose.onClick.AddListener(TutupKamus);

        GameObject txtX = new GameObject("TeksX", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtX.transform.SetParent(closeBtnGo.transform, false);
        RectTransform rtX = txtX.GetComponent<RectTransform>();
        rtX.anchorMin = Vector2.zero;
        rtX.anchorMax = Vector2.one;
        rtX.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmpX = txtX.GetComponent<TextMeshProUGUI>();
        tmpX.text = "✕";
        tmpX.fontSize = 22;
        tmpX.fontStyle = FontStyles.Bold;
        tmpX.alignment = TextAlignmentOptions.Center;
        tmpX.color = Color.white;

        // 4. Scroll View untuk konten istilah
        GameObject scrollViewGo = new GameObject("ScrollViewKamus", typeof(RectTransform), typeof(ScrollRect));
        scrollViewGo.transform.SetParent(cardGo.transform, false);
        RectTransform rtScroll = scrollViewGo.GetComponent<RectTransform>();
        rtScroll.anchorMin = new Vector2(0f, 0f);
        rtScroll.anchorMax = new Vector2(1f, 1f);
        rtScroll.pivot = new Vector2(0.5f, 0.5f);
        // Sisakan ruang untuk Header (85px) dan Footer (60px)
        rtScroll.offsetMin = new Vector2(25f, 65f);
        rtScroll.offsetMax = new Vector2(-25f, -95f);

        ScrollRect scrollRect = scrollViewGo.GetComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;

        // Viewport
        GameObject viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
        viewportGo.transform.SetParent(scrollViewGo.transform, false);
        RectTransform rtViewport = viewportGo.GetComponent<RectTransform>();
        rtViewport.anchorMin = Vector2.zero;
        rtViewport.anchorMax = Vector2.one;
        rtViewport.sizeDelta = Vector2.zero;
        rtViewport.pivot = new Vector2(0f, 1f);

        scrollRect.viewport = rtViewport;

        // Content
        GameObject contentGo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentGo.transform.SetParent(viewportGo.transform, false);
        RectTransform rtContent = contentGo.GetComponent<RectTransform>();
        rtContent.anchorMin = new Vector2(0f, 1f);
        rtContent.anchorMax = new Vector2(1f, 1f);
        rtContent.pivot = new Vector2(0.5f, 1f);
        rtContent.sizeDelta = new Vector2(0f, 0f);

        scrollRect.content = rtContent;

        VerticalLayoutGroup vlg = contentGo.GetComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(5, 5, 8, 8);
        vlg.spacing = 14f;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = contentGo.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Masukkan semua kartu istilah ke dalam Content
        foreach (var istilah in daftarIstilah)
        {
            BuatKartuIstilah(contentGo.transform, istilah);
        }

        // 5. Footer Bar (Tombol Kembali & Petunjuk)
        GameObject footerGo = new GameObject("FooterBar", typeof(RectTransform));
        footerGo.transform.SetParent(cardGo.transform, false);
        RectTransform rtFooter = footerGo.GetComponent<RectTransform>();
        rtFooter.anchorMin = new Vector2(0f, 0f);
        rtFooter.anchorMax = new Vector2(1f, 0f);
        rtFooter.pivot = new Vector2(0.5f, 0f);
        rtFooter.anchoredPosition = new Vector2(0f, 12f);
        rtFooter.sizeDelta = new Vector2(-40f, 44f);

        // Petunjuk Edukasi di Kiri
        GameObject hintGo = new GameObject("TeksPetunjuk", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        hintGo.transform.SetParent(footerGo.transform, false);
        RectTransform rtHint = hintGo.GetComponent<RectTransform>();
        rtHint.anchorMin = new Vector2(0f, 0f);
        rtHint.anchorMax = new Vector2(0.75f, 1f);
        rtHint.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmpHint = hintGo.GetComponent<TextMeshProUGUI>();
        tmpHint.text = "💡 <i>Buka kamus ini kapan saja jika ragu sebelum mengambil keputusan investasi di dalam game.</i>";
        tmpHint.fontSize = 14;
        tmpHint.color = new Color(0.85f, 0.75f, 0.40f);
        tmpHint.alignment = TextAlignmentOptions.MidlineLeft;

        // Tombol Tutup di Kanan
        GameObject closeBtnFooter = new GameObject("TombolTutupFooter", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        closeBtnFooter.transform.SetParent(footerGo.transform, false);
        RectTransform rtCloseF = closeBtnFooter.GetComponent<RectTransform>();
        rtCloseF.anchorMin = new Vector2(1f, 0.5f);
        rtCloseF.anchorMax = new Vector2(1f, 0.5f);
        rtCloseF.pivot = new Vector2(1f, 0.5f);
        rtCloseF.anchoredPosition = Vector2.zero;
        rtCloseF.sizeDelta = new Vector2(140f, 38f);

        Image imgCloseF = closeBtnFooter.GetComponent<Image>();
        imgCloseF.color = new Color(0.18f, 0.25f, 0.38f, 0.95f);

        Button btnCloseF = closeBtnFooter.GetComponent<Button>();
        btnCloseF.onClick.AddListener(TutupKamus);

        GameObject txtCloseF = new GameObject("TeksBtnTutup", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtCloseF.transform.SetParent(closeBtnFooter.transform, false);
        RectTransform rtTxtCloseF = txtCloseF.GetComponent<RectTransform>();
        rtTxtCloseF.anchorMin = Vector2.zero;
        rtTxtCloseF.anchorMax = Vector2.one;
        rtTxtCloseF.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmpCloseF = txtCloseF.GetComponent<TextMeshProUGUI>();
        tmpCloseF.text = "Tutup Kamus";
        tmpCloseF.fontSize = 15;
        tmpCloseF.fontStyle = FontStyles.Bold;
        tmpCloseF.alignment = TextAlignmentOptions.Center;
        tmpCloseF.color = Color.white;

        return backdrop;
    }

    private void BuatKartuIstilah(Transform parent, IstilahSaham item)
    {
        GameObject itemCard = new GameObject("Item_" + item.judul, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        itemCard.transform.SetParent(parent, false);

        Image img = itemCard.GetComponent<Image>();
        img.color = new Color(0.11f, 0.16f, 0.26f, 0.95f); // Soft dark blue-grey card

        VerticalLayoutGroup vlg = itemCard.GetComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(16, 16, 12, 12);
        vlg.spacing = 6f;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = itemCard.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Baris 1: Judul dan Badge Kategori
        GameObject row1 = new GameObject("BarisJudul", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        row1.transform.SetParent(itemCard.transform, false);
        TextMeshProUGUI tmpRow1 = row1.GetComponent<TextMeshProUGUI>();
        tmpRow1.text = $"<b>{item.judul}</b>   <size=13><color={item.warnaKategoriHex}>[{item.kategori}]</color></size>";
        tmpRow1.fontSize = 19;
        tmpRow1.color = new Color(0.95f, 0.97f, 1f);

        // Baris 2: Definisi
        GameObject row2 = new GameObject("Definisi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        row2.transform.SetParent(itemCard.transform, false);
        TextMeshProUGUI tmpRow2 = row2.GetComponent<TextMeshProUGUI>();
        tmpRow2.text = item.definisi;
        tmpRow2.fontSize = 16;
        tmpRow2.color = new Color(0.80f, 0.86f, 0.94f);
        tmpRow2.lineSpacing = 4f;

        // Baris 3: Edukasi / Peringatan
        GameObject row3 = new GameObject("Edukasi", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        row3.transform.SetParent(itemCard.transform, false);
        TextMeshProUGUI tmpRow3 = row3.GetComponent<TextMeshProUGUI>();
        tmpRow3.text = $"<color=#FCD34D><b>Pelajaran Skripsi:</b> {item.edukasiSkripsi}</color>";
        tmpRow3.fontSize = 15;
        tmpRow3.lineSpacing = 3f;
    }

    [YarnCommand("buka_kamus")]
    public void BukaKamus()
    {
        if (panelKamusModal != null)
        {
            panelKamusModal.SetActive(true);
            AudioManager.Instance?.PutarSFX("klik");
        }
    }

    [YarnCommand("tutup_kamus")]
    public void TutupKamus()
    {
        if (panelKamusModal != null)
        {
            panelKamusModal.SetActive(false);
            AudioManager.Instance?.PutarSFX("klik");
        }
    }
}
