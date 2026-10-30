using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuEndingManager : MonoBehaviour
{
    public static MainMenuEndingManager Instance { get; private set; }

    [Header("Referensi Objek UI")]
    public GameObject cardGaleri;
    public GameObject btnPilihBabak;
    public GameObject panelModalBabak;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        InisialisasiUI();
    }

    void OnEnable()
    {
        InisialisasiUI();
    }

    [ContextMenu("Segarkan Progress UI")]
    public void RefreshProgress()
    {
        InisialisasiUI();
    }

    public void InisialisasiUI()
    {
        // 1. Cari referensi otomatis di CanvasMainMenu jika belum di-assign di Inspector
        if (cardGaleri == null || btnPilihBabak == null || panelModalBabak == null)
        {
            GameObject canvasGo = GameObject.Find("CanvasMainMenu");
            if (canvasGo == null)
            {
                Canvas c = FindFirstObjectByType<Canvas>();
                if (c != null) canvasGo = c.gameObject;
            }

            if (canvasGo != null)
            {
                Transform canvasT = canvasGo.transform;
                if (cardGaleri == null)
                {
                    Transform t = canvasT.Find("CardEndingCollector");
                    if (t == null) t = canvasT.Find("CardGaleriEnding");
                    if (t != null) cardGaleri = t.gameObject;
                }
                if (btnPilihBabak == null)
                {
                    Transform t = canvasT.Find("TombolPilihBabak");
                    if (t != null) btnPilihBabak = t.gameObject;
                }
                if (panelModalBabak == null)
                {
                    Transform t = canvasT.Find("PanelModalPilihBabak");
                    if (t != null) panelModalBabak = t.gameObject;
                }
            }
        }

        // Baca status pencapaian ending
        bool endingA = PlayerPrefs.GetInt("Ending_A_Unlocked", 0) == 1;
        bool endingB = PlayerPrefs.GetInt("Ending_B_Unlocked", 0) == 1;
        bool endingC = PlayerPrefs.GetInt("Ending_C_Unlocked", 0) == 1;
        bool endingD = PlayerPrefs.GetInt("Ending_D_Unlocked", 0) == 1;
        int totalUnlocked = (endingA ? 1 : 0) + (endingB ? 1 : 0) + (endingC ? 1 : 0) + (endingD ? 1 : 0);

        // Fitur shortcut / pilih babak HANYA aktif jika sudah selesai minimal 1 ending
        bool isShortcutUnlocked = totalUnlocked >= 1;

        // 2. Perbarui CardEndingCollector
        if (cardGaleri != null)
        {
            UpdateTampilanGaleri(cardGaleri, endingA, endingB, endingC, endingD, totalUnlocked);
        }

        // 3. Tombol Shortcut / Pilih Babak
        if (btnPilihBabak != null)
        {
            btnPilihBabak.SetActive(isShortcutUnlocked);

            Button btn = btnPilihBabak.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(BukaModalPilihBabak);
                btn.onClick.AddListener(BukaModalPilihBabak);
            }
        }

        // 4. Hubungkan tombol-tombol di dalam modal babak
        if (panelModalBabak != null)
        {
            HubungkanListenerModalBabak(panelModalBabak);
            panelModalBabak.SetActive(false);
        }
    }

    private void UpdateTampilanGaleri(GameObject card, bool ea, bool eb, bool ec, bool ed, int total)
    {
        if (card == null) return;

        // Update TextCount
        Transform tCount = card.transform.Find("TextCount");
        if (tCount != null)
        {
            TextMeshProUGUI tmpC = tCount.GetComponent<TextMeshProUGUI>();
            if (tmpC != null)
            {
                tmpC.text = $"<size=36><b><color=#38BDF8>{total}</color></b></size> <size=22><color=#94A3B8>/ 4</color></size> <size=18><color=#E2E8F0>Ending Terbuka</color></size>";
            }
        }

        // Update TextHint
        Transform tHint = card.transform.Find("TextHint");
        if (tHint != null)
        {
            TextMeshProUGUI tmpH = tHint.GetComponent<TextMeshProUGUI>();
            if (tmpH != null)
            {
                if (total >= 1)
                {
                    tmpH.text = "<i>Shortcut Babak Terbuka! Klik 'Pilih Babak' untuk mencoba cabang lain.</i>";
                }
                else
                {
                    tmpH.text = "<i>Selesaikan minimal 1 ending untuk membuka fitur Pilih Babak (Shortcut).</i>";
                }
            }
        }
    }

    private void HubungkanListenerModalBabak(GameObject modal)
    {
        if (modal == null) return;

        // Hubungkan semua tombol close / backdrop
        Button[] allButtons = modal.GetComponentsInChildren<Button>(true);
        foreach (var b in allButtons)
        {
            string n = b.name.ToLower();
            if (n.Contains("tutup") || n.Contains("close") || n.Contains("btnclose") || n.Contains("labelx") || n.Contains("backdrop"))
            {
                b.onClick.RemoveListener(TutupModalPilihBabak);
                b.onClick.AddListener(TutupModalPilihBabak);
            }
        }

        Transform cardModal = modal.transform.Find("CardModalBabak");
        if (cardModal == null) return;

        Transform container = cardModal.Find("ContainerBabak");
        if (container != null)
        {
            // Percabangan 1: Pertimbangan Awal (Kata Heri vs Beli FOMO)
            Transform c1 = container.Find("CardBabak_1");
            if (c1 != null) PasangListenerBtnMainkan(c1, "Fase_Heri_Datang", 5000000);

            // Percabangan 2: Ujian Telepon Bima (ARA 135 vs Tetap Nabung)
            Transform c2 = container.Find("CardBabak_2");
            if (c2 != null) PasangListenerBtnMainkan(c2, "Fase_Rasional_Telepon", 5000000);

            // Percabangan 3: Krisis Penurunan Saham (Average Down vs Cut Loss)
            Transform c3 = container.Find("CardBabak_3");
            if (c3 != null) PasangListenerBtnMainkan(c3, "Fase_Anjlok_Hold", 3000000);
        }
    }

    private void PasangListenerBtnMainkan(Transform cardBabak, string targetNode, int saldo)
    {
        Transform btnT = cardBabak.Find("BtnMainkan");
        if (btnT != null)
        {
            Button btn = btnT.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(() => LompatKeBabak(targetNode, saldo));
                btn.onClick.AddListener(() => LompatKeBabak(targetNode, saldo));
            }
        }
    }

    public void BukaModalPilihBabak()
    {
        if (panelModalBabak != null)
        {
            panelModalBabak.SetActive(true);
            HubungkanListenerModalBabak(panelModalBabak);
        }
    }

    public void TutupModalPilihBabak()
    {
        if (panelModalBabak != null) panelModalBabak.SetActive(false);
    }

    public void LompatKeBabak(string targetNode, int saldoAwal)
    {
        PlayerPrefs.SetString("JumpTargetNode", targetNode);
        PlayerPrefs.SetInt("JumpSaldo", saldoAwal);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Gameplay");
    }
}
