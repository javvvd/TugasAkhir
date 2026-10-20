using UnityEngine;
using TMPro;
using Yarn.Unity;

public class TugasManager : MonoBehaviour
{
    [Header("Komponen UI Tugas")]
    public GameObject panelTugas;
    public TextMeshProUGUI teksIsiTugas;

    [YarnCommand("ubah_tugas")]
    public void UbahTugas(string tugasBaru)
    {
        if (teksIsiTugas != null)
        {
            teksIsiTugas.text = "- " + tugasBaru;
        }
        if (panelTugas != null && !panelTugas.activeSelf)
        {
            panelTugas.SetActive(true);
        }
    }

    [YarnCommand("tampilkan_tugas")]
    public void TampilkanTugas()
    {
        if (panelTugas != null) panelTugas.SetActive(true);
    }

    [YarnCommand("sembunyikan_tugas")]
    public void SembunyikanTugas()
    {
        if (panelTugas != null) panelTugas.SetActive(false);
    }
}
