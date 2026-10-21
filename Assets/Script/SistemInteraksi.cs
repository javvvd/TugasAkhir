using UnityEngine;
using TMPro;
using Yarn.Unity;

public class SistemInteraksi : MonoBehaviour
{
    public Camera kameraPemain;
    public GameObject tombolUI;
    public TextMeshProUGUI teksTombol;
    public DialogueRunner mesinCerita;
    public float jarakInteraksi = 5f;

    // Sekarang kita menyimpan script BendaInteraktif, bukan GameObject-nya
    private BendaInteraktif bendaYangDitabrak;

    void Awake()
    {
        if (tombolUI != null && teksTombol == null)
        {
            teksTombol = tombolUI.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (mesinCerita == null)
        {
            mesinCerita = FindFirstObjectByType<DialogueRunner>();
        }
    }

    void Update()
    {
        // Cegah interaksi kalau dialog sedang jalan
        if (mesinCerita != null && mesinCerita.IsDialogueRunning)
        {
            if (tombolUI != null && tombolUI.activeSelf)
            {
                tombolUI.SetActive(false);
            }
            bendaYangDitabrak = null;
            return;
        }

        // Cari kamera yang sedang aktif di hierarchy
        Camera cam = (kameraPemain != null && kameraPemain.gameObject.activeInHierarchy) ? kameraPemain : Camera.main;
        if (cam == null)
        {
            cam = FindFirstObjectByType<Camera>();
        }
        if (cam == null) return;

        Ray laser = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit dataTabrakan;

        if (Physics.Raycast(laser, out dataTabrakan, jarakInteraksi))
        {
            // Cek apakah benda yang ditabrak punya script "BendaInteraktif" (baik di objek atau parent-nya)
            BendaInteraktif benda = dataTabrakan.collider.GetComponent<BendaInteraktif>();
            if (benda == null)
            {
                benda = dataTabrakan.collider.GetComponentInParent<BendaInteraktif>();
            }

            if (benda != null)
            {
                if (tombolUI != null && !tombolUI.activeSelf)
                {
                    tombolUI.SetActive(true); // Munculkan tombol UI
                }

                if (teksTombol != null)
                {
                    teksTombol.text = !string.IsNullOrEmpty(benda.teksTombolInteraksi) ? benda.teksTombolInteraksi : "Interaksi";
                }

                bendaYangDitabrak = benda; // Ingat benda yang sedang disorot

                // Tambahan: bisa tekan tombol E di keyboard juga untuk PC/Editor
                if (Input.GetKeyDown(KeyCode.E))
                {
                    TombolDitekan();
                }
            }
            else
            {
                if (tombolUI != null && tombolUI.activeSelf)
                {
                    tombolUI.SetActive(false);
                }
                bendaYangDitabrak = null;
            }
        }
        else
        {
            if (tombolUI != null && tombolUI.activeSelf)
            {
                tombolUI.SetActive(false);
            }
            bendaYangDitabrak = null;
        }
    }

    // Fungsi ini dipanggil saat tombol UI di layar ditekan
    public void TombolDitekan()
    {
        // Pastikan ada benda yang ditabrak dan string node Yarn-nya tidak kosong
        if (bendaYangDitabrak != null && !string.IsNullOrEmpty(bendaYangDitabrak.namaNodeYarn))
        {
            if (tombolUI != null)
            {
                tombolUI.SetActive(false);
            }

            string nodeTarget = bendaYangDitabrak.namaNodeYarn;
            bendaYangDitabrak = null;

            // Mulai dialog sesuai nama node yang ada di benda tersebut!
            if (mesinCerita != null)
            {
                mesinCerita.StartDialogue(nodeTarget);
            }
        }
    }
}