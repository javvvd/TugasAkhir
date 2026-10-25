using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuEndingManager : MonoBehaviour
{
    public static MainMenuEndingManager Instance { get; private set; }

    [Header("Referensi Objek UI (Otomatis dicari jika kosong)")]
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

    private void InisialisasiUI()
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

        // Baca progress ending dari PlayerPrefs
        bool endingA = PlayerPrefs.GetInt("Ending_A_Unlocked", 0) == 1;
        bool endingB = PlayerPrefs.GetInt("Ending_B_Unlocked", 0) == 1;
        bool endingC = PlayerPrefs.GetInt("Ending_C_Unlocked", 0) == 1;
        bool endingD = PlayerPrefs.GetInt("Ending_D_Unlocked", 0) == 1;
        int totalUnlocked = (endingA ? 1 : 0) + (endingB ? 1 : 0) + (endingC ? 1 : 0) + (endingD ? 1 : 0);

        // 2. Perbarui teks dan centang pada kartu galeri / ending collector
        if (cardGaleri != null)
        {
            UpdateTampilanGaleri(cardGaleri, endingA, endingB, endingC, endingD, totalUnlocked);
        }

        // 3. Hubungkan tombol buka modal percabangan babak
        if (btnPilihBabak != null)
        {
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

        // Update TextCount (jika menggunakan format CardEndingCollector)
        Transform tCount = card.transform.Find("TextCount");
        if (tCount != null)
        {
            TextMeshProUGUI tmpC = tCount.GetComponent<TextMeshProUGUI>();
            if (tmpC != null)
            {
                tmpC.text = $"<size=36><b><color=#38BDF8>{total}</color></b></size> <size=22><color=#94A3B8>/ 4</color></size> <size=18><color=#E2E8F0>Ending Terbuka</color></size>";
            }
        }

        // Update Header jika ada
        Transform header = card.transform.Find("HeaderGaleri");
        if (header != null)
        {
            TextMeshProUGUI tmpH = header.GetComponent<TextMeshProUGUI>();
            if (tmpH != null)
            {
                tmpH.text = $"🏆 <b>PENCAPAIAN ENDING</b>  <size=16><color=#38BDF8>[ {total} / 4 Terbuka ]</color></size>";
            }
        }

        // Update List Items jika ada
        Transform listT = card.transform.Find("ListEnding");
        if (listT != null)
        {
            UpdateItemBaris(listT, "Ending A: Kebangkrutan Total (Gocap)", ea, "#EF4444");
            UpdateItemBaris(listT, "Ending B: Realita Trader (Cut Loss)", eb, "#F59E0B");
            UpdateItemBaris(listT, "Ending C: Korban Cuci Piring (Pucuk)", ec, "#F97316");
            UpdateItemBaris(listT, "Ending D: Investor Rasional (Cerdas)", ed, "#10B981");
        }
    }

    private void UpdateItemBaris(Transform listParent, string namaEnding, bool terbuka, string hexColor)
    {
        Transform item = listParent.Find("Item_" + namaEnding);
        if (item != null)
        {
            Image img = item.GetComponent<Image>();
            if (img != null)
            {
                img.color = terbuka ? new Color(0.12f, 0.18f, 0.30f, 0.95f) : new Color(0.06f, 0.08f, 0.12f, 0.80f);
            }
            Transform txtT = item.Find("Teks");
            if (txtT != null)
            {
                TextMeshProUGUI tmp = txtT.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    if (terbuka)
                    {
                        tmp.text = $"<color={hexColor}><b>[ ✓ ] {namaEnding}</b></color>";
                        tmp.color = Color.white;
                    }
                    else
                    {
                        tmp.text = $"<color=#64748B>[ ? ] {namaEnding.Split(':')[0]}: Belum Terbuka</color>";
                        tmp.color = new Color(0.40f, 0.48f, 0.58f);
                    }
                }
            }
        }
    }

    private void HubungkanListenerModalBabak(GameObject modal)
    {
        if (modal == null) return;

        // Pasang event tutup ke semua tombol close/x di dalam modal
        Button[] allButtons = modal.GetComponentsInChildren<Button>(true);
        foreach (var b in allButtons)
        {
            string n = b.name.ToLower();
            if (n.Contains("tutup") || n.Contains("close") || n.Contains("btnclose") || n.Contains("labelx"))
            {
                b.onClick.RemoveListener(TutupModalPilihBabak);
                b.onClick.AddListener(TutupModalPilihBabak);
            }
        }

        Transform cardModal = modal.transform.Find("CardModalBabak");
        if (cardModal == null) return;

        // 3 Tombol Mainkan di ContainerBabak
        Transform container = cardModal.Find("ContainerBabak");
        if (container != null)
        {
            if (container.childCount > 0)
            {
                PasangListenerBtnMainkan(container.GetChild(0), "Fase_Heri_Datang", 5000000);
            }
            if (container.childCount > 1)
            {
                PasangListenerBtnMainkan(container.GetChild(1), "Fase_Rasional_Telepon", 5000000);
            }
            if (container.childCount > 2)
            {
                PasangListenerBtnMainkan(container.GetChild(2), "Fase_Harga_Anjlok", 3000000);
            }
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

    private void LompatKeBabak(string targetNode, int saldoAwal)
    {
        PlayerPrefs.SetString("JumpTargetNode", targetNode);
        PlayerPrefs.SetInt("JumpSaldo", saldoAwal);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Gameplay");
    }
}
