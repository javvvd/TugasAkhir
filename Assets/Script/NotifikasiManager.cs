using UnityEngine;
using Yarn.Unity; 

public class NotifikasiManager : MonoBehaviour
{
    [Header("UI Pop-up")]
    public GameObject panelHandphone;
    public GameObject latarGelap; // <-- Tambahkan baris ini

    [YarnCommand("munculkan_hp")]
    public void MunculkanHP()
    {
        latarGelap.SetActive(true); // <-- Nyalakan latar gelap
        panelHandphone.SetActive(true);
    }

    [YarnCommand("tutup_hp")]
    public void TutupHP()
    {
        panelHandphone.SetActive(false);
        latarGelap.SetActive(false); // <-- Matikan latar gelap
    }
}