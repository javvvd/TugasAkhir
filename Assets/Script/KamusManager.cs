using System.Collections;
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

    [Header("Referensi UI (Otomatis dicari jika kosong)")]
    public GameObject tombolBukaKamus;
    public GameObject panelKamusModal;

    [Header("Daftar Istilah Edukasi Saham")]
    public List<IstilahSaham> daftarIstilah = new List<IstilahSaham>();

    // Data statis di RAM untuk lompatan babak (0ms, tanpa beban I/O disk PlayerPrefs)
    public static string TargetNodeLompatan = "";
    public static int SaldoLompatan = 5000000;

    private bool sedangLompatBabak = false;
    private string nodeTujuanAktif = "";
    private int saldoTujuanAktif = 5000000;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Jika ada request lompat babak dari Main Menu, alihkan startNode DialogueRunner sebelum Start() berjalan
        if (!string.IsNullOrEmpty(TargetNodeLompatan))
        {
            sedangLompatBabak = true;
            nodeTujuanAktif = TargetNodeLompatan;
            saldoTujuanAktif = SaldoLompatan;

            // Langsung bersihkan variabel statis agar sesi berikutnya kembali normal
            TargetNodeLompatan = "";

            DialogueRunner runner = FindFirstObjectByType<DialogueRunner>();
            if (runner != null)
            {
                runner.startNode = nodeTujuanAktif;
            }
        }
    }

    void Start()
    {
        InisialisasiUI();

        if (sedangLompatBabak)
        {
            StartCoroutine(TerapkanKondisiAwalBabak());
        }
    }

    private void InisialisasiUI()
    {
        // 1. Cari otomatis jika belum di-drag ke Inspector
        if (tombolBukaKamus == null || panelKamusModal == null)
        {
            Canvas canvasUtama = null;
            GameObject uiGo = GameObject.Find("UI");
            if (uiGo != null) canvasUtama = uiGo.GetComponent<Canvas>();
            if (canvasUtama == null) canvasUtama = FindFirstObjectByType<Canvas>();

            if (canvasUtama != null)
            {
                Transform canvasT = canvasUtama.transform;
                if (tombolBukaKamus == null)
                {
                    Transform tBtn = canvasT.Find("TombolKamusSaham");
                    if (tBtn != null) tombolBukaKamus = tBtn.gameObject;
                }
                if (panelKamusModal == null)
                {
                    Transform tModal = canvasT.Find("PanelKamusModal");
                    if (tModal != null) panelKamusModal = tModal.gameObject;
                }
            }
        }

        // 2. Pasang event click tombol buka kamus
        if (tombolBukaKamus != null)
        {
            Button btn = tombolBukaKamus.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(BukaKamus);
                btn.onClick.AddListener(BukaKamus);
            }
        }

        // 3. Pasang event click tombol tutup di dalam modal
        if (panelKamusModal != null)
        {
            PasangListenerTombolTutup(panelKamusModal);
            panelKamusModal.SetActive(false);
        }
    }

    private void PasangListenerTombolTutup(GameObject modal)
    {
        if (modal == null) return;

        Button[] buttons = modal.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            string n = btn.name.ToLower();
            if (n.Contains("tutup") || n.Contains("close") || n.Contains("tomboltutup") || n.Contains("labelx"))
            {
                btn.onClick.RemoveListener(TutupKamus);
                btn.onClick.AddListener(TutupKamus);
            }
        }
    }

    [YarnCommand("buka_kamus")]
    public void BukaKamus()
    {
        if (panelKamusModal != null)
        {
            panelKamusModal.SetActive(true);
            PasangListenerTombolTutup(panelKamusModal);

            ScrollRect sr = panelKamusModal.GetComponentInChildren<ScrollRect>(true);
            if (sr != null)
            {
                Canvas.ForceUpdateCanvases();
                sr.verticalNormalizedPosition = 1f;
            }

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

    // ================= LOGIKA LOMPATAN PERCABANGAN CERITA =================
    private IEnumerator TerapkanKondisiAwalBabak()
    {
        // Tunggu 1 frame agar seluruh Start() manager lain selesai dieksekusi
        yield return null;

        // 1. Sesuaikan Saldo Tabungan
        FindFirstObjectByType<UangManager>()?.SetSaldo(saldoTujuanAktif);

        // 2. Set Kamera langsung ke Meja menghadap Laptop/HP
        PengendaliKamera camMgr = PengendaliKamera.Instance != null ? PengendaliKamera.Instance : FindFirstObjectByType<PengendaliKamera>();
        camMgr?.SetKameraInstan("meja");

        // 3. Sembunyikan Tugas Pembuka
        FindFirstObjectByType<TugasManager>()?.SembunyikanTugas();

        // 4. Terangkan layar (Fade In) jika node tujuan tidak memulai dengan fade_out sendiri
        if (nodeTujuanAktif != "Fase_Anjlok_Hold")
        {
            FindFirstObjectByType<TransisiLayar>()?.FadeIn();
            AudioManager.Instance?.PutarBGM("santai");
        }
    }
}
