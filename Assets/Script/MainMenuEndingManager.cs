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

    public void RefreshProgress()
    {
        InisialisasiUI();
    }

    private void InisialisasiUI()
    {
        // 1. Cari otomatis jika belum di-drag ke Inspector
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
                    Transform t = canvasT.Find("CardGaleriEnding");
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

        // 2. Perbarui tampilan data progress ending dari PlayerPrefs
        if (cardGaleri != null)
        {
            UpdateTampilanGaleri();
        }

        // 3. Hubungkan tombol pembuka modal babak
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
            HubungkanListenerModalBabak();
            panelModalBabak.SetActive(false);
        }
    }

    private void UpdateTampilanGaleri()
    {
        bool endingA = PlayerPrefs.GetInt("Ending_A_Unlocked", 0) == 1;
        bool endingB = PlayerPrefs.GetInt("Ending_B_Unlocked", 0) == 1;
        bool endingC = PlayerPrefs.GetInt("Ending_C_Unlocked", 0) == 1;
        bool endingD = PlayerPrefs.GetInt("Ending_D_Unlocked", 0) == 1;
        int total = (endingA ? 1 : 0) + (endingB ? 1 : 0) + (endingC ? 1 : 0) + (endingD ? 1 : 0);

        // Header Progress
        Transform header = cardGaleri.transform.Find("HeaderGaleri");
        if (header != null)
        {
            TextMeshProUGUI tmpH = header.GetComponent<TextMeshProUGUI>();
            if (tmpH != null)
            {
                tmpH.text = $"🏆 <b>PENCAPAIAN ENDING</b>  <size=16><color=#38BDF8>[ {total} / 4 Terbuka ]</color></size>";
            }
        }

        // List 4 Ending
        Transform listT = cardGaleri.transform.Find("ListEnding");
        if (listT != null)
        {
            UpdateItemBaris(listT, "Ending A: Kebangkrutan Total (Gocap)", endingA, "#EF4444");
            UpdateItemBaris(listT, "Ending B: Realita Trader (Cut Loss)", endingB, "#F59E0B");
            UpdateItemBaris(listT, "Ending C: Korban Cuci Piring (Pucuk)", endingC, "#F97316");
            UpdateItemBaris(listT, "Ending D: Investor Rasional (Cerdas)", endingD, "#10B981");
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

    private void HubungkanListenerModalBabak()
    {
        Transform cardModal = panelModalBabak.transform.Find("CardModalBabak");
        if (cardModal == null) return;

        // Tombol Close X
        Transform header = cardModal.Find("Header");
        if (header != null)
        {
            Transform btnClose = header.Find("BtnCloseX");
            if (btnClose != null)
            {
                Button b = btnClose.GetComponent<Button>();
                if (b != null)
                {
                    b.onClick.RemoveListener(TutupModalPilihBabak);
                    b.onClick.AddListener(TutupModalPilihBabak);
                }
            }
        }

        // 3 Tombol Mainkan di ContainerBabak
        Transform container = cardModal.Find("ContainerBabak");
        if (container != null)
        {
            // Babak 1
            if (container.childCount > 0)
            {
                PasangListenerBtnMainkan(container.GetChild(0), "Fase_Heri_Datang", 5000000);
            }
            // Babak 2
            if (container.childCount > 1)
            {
                PasangListenerBtnMainkan(container.GetChild(1), "Fase_Rasional_Telepon", 5000000);
            }
            // Babak 3
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
        if (panelModalBabak != null) panelModalBabak.SetActive(true);
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
