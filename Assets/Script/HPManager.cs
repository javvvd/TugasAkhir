using UnityEngine;
using Yarn.Unity;

public class HPManager : MonoBehaviour
{
    [Header("UI Pop-up Utama")]
    public GameObject panelHandphone;
    public GameObject latarGelap;

    [YarnCommand("munculkan_hp")]
    public void MunculkanHP()
    {
        if (latarGelap != null) latarGelap.SetActive(true);
        if (panelHandphone != null) panelHandphone.SetActive(true);
    }

    [YarnCommand("tutup_hp")]
    public void TutupHP()
    {
        if (panelHandphone != null) panelHandphone.SetActive(false);
        if (latarGelap != null) latarGelap.SetActive(false);
    }
}