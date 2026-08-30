using UnityEngine;
using Yarn.Unity; // Wajib ada agar bisa memanggil cerita

public class SistemInteraksi : MonoBehaviour
{
    public Camera kameraPemain;
    public GameObject tombolUI;
    public DialogueRunner mesinCerita; // Penarik naskah Yarn

    private GameObject bendaYangDitabrak; 

    void Update()
    {
        // 1. Tembakkan laser ke tengah layar sejauh 3 meter
        Ray laser = new Ray(kameraPemain.transform.position, kameraPemain.transform.forward);
        RaycastHit dataTabrakan;

        if (Physics.Raycast(laser, out dataTabrakan, 3f))
        {
            // 2. Kalau laser kena benda dengan Tag "Handphone"
            if (dataTabrakan.collider.CompareTag("Handphone"))
            {
                tombolUI.SetActive(true); // Munculkan tombol
                bendaYangDitabrak = dataTabrakan.collider.gameObject; // Ingat HP-nya
            }
            else
            {
                tombolUI.SetActive(false); // Kalau nengok ke benda lain, matikan tombol
            }
        }
        else
        {
            tombolUI.SetActive(false); // Kalau nengok ke ruang kosong, matikan tombol
        }
    }

    // 3. Fungsi ini akan dipanggil kalau tombol di layar ditekan
    public void TombolDitekan()
    {
        if (bendaYangDitabrak != null && bendaYangDitabrak.CompareTag("Handphone"))
        {
            tombolUI.SetActive(false); // Sembunyikan tombolnya lagi
            
            // Mulai naskah Yarn Spinner (Pastikan nama nodenya benar)
            if (mesinCerita.IsDialogueRunning == false)
            {
                mesinCerita.StartDialogue("Fase_0_Pembuka");
            }
        }
    }
}