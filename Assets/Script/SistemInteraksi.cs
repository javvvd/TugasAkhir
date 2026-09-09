using UnityEngine;
using Yarn.Unity;

public class SistemInteraksi : MonoBehaviour
{
    public Camera kameraPemain;
    public GameObject tombolUI;
    public DialogueRunner mesinCerita;

    // Sekarang kita menyimpan script BendaInteraktif, bukan GameObject-nya
    private BendaInteraktif bendaYangDitabrak;

    void Update()
    {
        // Cegah interaksi kalau dialog sedang jalan
        if (mesinCerita.IsDialogueRunning)
        {
            tombolUI.SetActive(false);
            return;
        }

        Ray laser = new Ray(kameraPemain.transform.position, kameraPemain.transform.forward);
        RaycastHit dataTabrakan;

        if (Physics.Raycast(laser, out dataTabrakan, 3f))
        {
            // Cek apakah benda yang ditabrak punya script "BendaInteraktif"
            BendaInteraktif benda = dataTabrakan.collider.GetComponent<BendaInteraktif>();

            if (benda != null)
            {
                tombolUI.SetActive(true); // Munculkan tombol UI
                bendaYangDitabrak = benda; // Ingat benda yang sedang disorot
            }
            else
            {
                tombolUI.SetActive(false);
                bendaYangDitabrak = null;
            }
        }
        else
        {
            tombolUI.SetActive(false);
            bendaYangDitabrak = null;
        }
    }

    // Fungsi ini dipanggil saat tombol UI di layar ditekan
    public void TombolDitekan()
    {
        // Pastikan ada benda yang ditabrak dan string node Yarn-nya tidak kosong
        if (bendaYangDitabrak != null && !string.IsNullOrEmpty(bendaYangDitabrak.namaNodeYarn))
        {
            tombolUI.SetActive(false);

            // Mulai dialog sesuai nama node yang ada di benda tersebut!
            mesinCerita.StartDialogue(bendaYangDitabrak.namaNodeYarn);
        }
    }
}