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

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        InisialisasiUI();
        CekEksekusiLompatanBabak();
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
    private void CekEksekusiLompatanBabak()
    {
        if (PlayerPrefs.HasKey("JumpTargetNode"))
        {
            string targetNode = PlayerPrefs.GetString("JumpTargetNode", "");
            int saldo = PlayerPrefs.GetInt("JumpSaldo", 5000000);

            PlayerPrefs.DeleteKey("JumpTargetNode");
            PlayerPrefs.DeleteKey("JumpSaldo");
            PlayerPrefs.Save();

            if (!string.IsNullOrEmpty(targetNode))
            {
                StartCoroutine(ProsesLompatanRoutine(targetNode, saldo));
            }
        }
    }

    private IEnumerator ProsesLompatanRoutine(string targetNode, int saldo)
    {
        // Tunggu satu frame agar seluruh sistem scene & DialogueRunner siap
        yield return null;

        // 1. Sesuaikan Saldo Tabungan
        UangManager uangMgr = FindFirstObjectByType<UangManager>();
        if (uangMgr != null)
        {
            uangMgr.SetSaldo(saldo);
        }

        // 2. Set Kamera langsung ke Meja menghadap Laptop/HP
        PengendaliKamera camMgr = PengendaliKamera.Instance != null ? PengendaliKamera.Instance : FindFirstObjectByType<PengendaliKamera>();
        if (camMgr != null)
        {
            camMgr.SetKameraInstan("meja");
        }

        // 3. Sembunyikan Tugas Pembuka jika melompat
        TugasManager tugasMgr = FindFirstObjectByType<TugasManager>();
        if (tugasMgr != null)
        {
            tugasMgr.SembunyikanTugas();
        }

        // 4. Jalankan DialogueRunner langsung ke targetNode
        DialogueRunner runner = FindFirstObjectByType<DialogueRunner>();
        if (runner != null)
        {
            if (runner.IsDialogueRunning)
            {
                runner.Stop();
            }
            runner.StartDialogue(targetNode);
        }
    }
}
