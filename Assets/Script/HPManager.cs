using UnityEngine;
using Yarn.Unity;

public class HPManager : MonoBehaviour
{
    public static HPManager Instance { get; private set; }

    [Header("UI Pop-up Utama")]
    public GameObject panelHandphone;
    public GameObject latarGelap;

    [Header("Objek Fisik di Meja")]
    [Tooltip("Objek 3D HP di meja yang disembunyikan saat HP dibuka dan dimunculkan lagi saat HP ditutup")]
    public GameObject objekHpDiMeja;

    public bool IsHpActive => (panelHandphone != null && panelHandphone.activeInHierarchy) || (latarGelap != null && latarGelap.activeInHierarchy);

    void Awake()
    {
        Instance = this;

        if (objekHpDiMeja == null)
        {
            objekHpDiMeja = GameObject.Find("Handphone");
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    [YarnCommand("munculkan_hp")]
    public void MunculkanHP()
    {
        if (latarGelap != null) latarGelap.SetActive(true);
        if (panelHandphone != null) panelHandphone.SetActive(true);
        if (objekHpDiMeja != null) objekHpDiMeja.SetActive(false);
    }

    [YarnCommand("tutup_hp")]
    public void TutupHP()
    {
        if (panelHandphone != null) panelHandphone.SetActive(false);
        if (latarGelap != null) latarGelap.SetActive(false);
        if (objekHpDiMeja != null) objekHpDiMeja.SetActive(true);
    }
}
