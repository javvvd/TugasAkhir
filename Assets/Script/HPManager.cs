using UnityEngine;
using Yarn.Unity;

public class HPManager : MonoBehaviour
{
    [Header("UI Pop-up Utama")]
    public GameObject panelHandphone;
    public GameObject latarGelap;

    [Header("Objek Fisik di Meja")]
    [Tooltip("Objek 3D HP di meja yang disembunyikan saat HP dibuka dan dimunculkan lagi saat HP ditutup")]
    public GameObject objekHpDiMeja;

    void Awake()
    {
        if (objekHpDiMeja == null)
        {
            objekHpDiMeja = GameObject.Find("Handphone");
        }
    }

    [YarnCommand("munculkan_hp")]
    public void MunculkanHP()
    {
        if (latarGelap != null) latarGelap.SetActive(true);
        if (panelHandphone != null) panelHandphone.SetActive(true);
        SembunyikanHpMeja();
    }

    [YarnCommand("tutup_hp")]
    public void TutupHP()
    {
        if (panelHandphone != null) panelHandphone.SetActive(false);
        if (latarGelap != null) latarGelap.SetActive(false);
        MunculkanHpMeja();
    }

    [YarnCommand("sembunyikan_hp_meja")]
    public void SembunyikanHpMeja()
    {
        if (objekHpDiMeja != null)
        {
            objekHpDiMeja.SetActive(false);
        }
    }

    [YarnCommand("munculkan_hp_meja")]
    public void MunculkanHpMeja()
    {
        if (objekHpDiMeja != null)
        {
            objekHpDiMeja.SetActive(true);
        }
    }
}