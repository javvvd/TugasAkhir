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
                tmpC.text = $"{total} / 4";
            }
        }

        // Update TextHint (jika ada)
        Transform tHint = card.transform.Find("TextHint");
        if (tHint != null)
        {
            tHint.gameObject.SetActive(false);
        }
    }

    public void MainkanBabak1()
    {
        if (MainMenuController.Instance != null) MainMenuController.Instance.PlaySFXClick();
        LompatKeBabak("Fase_Heri_Datang", 5000000);
    }

    public void MainkanBabak2()
    {
        if (MainMenuController.Instance != null) MainMenuController.Instance.PlaySFXClick();
        LompatKeBabak("Fase_Rasional_Telepon", 5000000);
    }

    public void MainkanBabak3()
    {
        if (MainMenuController.Instance != null) MainMenuController.Instance.PlaySFXClick();
        LompatKeBabak("Fase_Anjlok_Hold", 3000000);
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
            Transform c1 = container.Find("CardBabak_1");
            if (c1 != null) PasangListenerBtn(c1, MainkanBabak1);

            Transform c2 = container.Find("CardBabak_2");
            if (c2 != null) PasangListenerBtn(c2, MainkanBabak2);

            Transform c3 = container.Find("CardBabak_3");
            if (c3 != null) PasangListenerBtn(c3, MainkanBabak3);
        }
    }

    private void PasangListenerBtn(Transform cardBabak, UnityEngine.Events.UnityAction aksi)
    {
        Transform btnT = cardBabak.Find("BtnMainkan");
        if (btnT != null)
        {
            Button btn = btnT.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(aksi);
                btn.onClick.AddListener(aksi);
            }
        }
    }

    public void BukaModalPilihBabak()
    {
        if (MainMenuController.Instance != null) MainMenuController.Instance.PlaySFXClick();
        if (panelModalBabak != null)
        {
            panelModalBabak.SetActive(true);
            HubungkanListenerModalBabak(panelModalBabak);
        }
    }

    public void TutupModalPilihBabak()
    {
        if (MainMenuController.Instance != null) MainMenuController.Instance.PlaySFXClick();
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
