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
    private Collider colliderTerakhir;

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
        if (Time.timeScale == 0f) return;

        // Cegah interaksi kalau HP sedang terbuka
        if (HPManager.Instance != null && HPManager.Instance.IsHpActive)
        {
            if (tombolUI != null && tombolUI.activeSelf) tombolUI.SetActive(false);
            bendaYangDitabrak = null;
            colliderTerakhir = null;
            return;
        }

        // Cegah interaksi kalau dialog sedang jalan
        if (mesinCerita != null && mesinCerita.IsDialogueRunning)
        {
            if (tombolUI != null && tombolUI.activeSelf)
            {
                tombolUI.SetActive(false);
            }
            bendaYangDitabrak = null;
            colliderTerakhir = null;
            return;
        }

        // Gunakan referensi kamera aktif dari PengendaliKamera tanpa perlu FindFirstObjectByType tiap frame
        Camera cam = (kameraPemain != null && kameraPemain.gameObject.activeInHierarchy)
            ? kameraPemain
            : (PengendaliKamera.Instance != null ? PengendaliKamera.Instance.GetKameraAktif() : Camera.main);
        if (cam == null) return;

        Ray laser = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit dataTabrakan;

        if (Physics.Raycast(laser, out dataTabrakan, jarakInteraksi))
        {
            // Hanya panggil GetComponent saat collider yang disorot berubah
            if (dataTabrakan.collider != colliderTerakhir)
            {
                colliderTerakhir = dataTabrakan.collider;
                BendaInteraktif benda = colliderTerakhir.GetComponent<BendaInteraktif>();
                if (benda == null)
                {
                    benda = colliderTerakhir.GetComponentInParent<BendaInteraktif>();
                }
                bendaYangDitabrak = benda;

                if (bendaYangDitabrak != null)
                {
                    if (tombolUI != null && !tombolUI.activeSelf)
                    {
                        tombolUI.SetActive(true);
                    }
                    if (teksTombol != null)
                    {
                        teksTombol.text = !string.IsNullOrEmpty(bendaYangDitabrak.teksTombolInteraksi)
                            ? bendaYangDitabrak.teksTombolInteraksi
                            : "Interaksi";
                    }
                }
                else
                {
                    if (tombolUI != null && tombolUI.activeSelf)
                    {
                        tombolUI.SetActive(false);
                    }
                }
            }
        }
        else
        {
            if (colliderTerakhir != null || bendaYangDitabrak != null)
            {
                colliderTerakhir = null;
                bendaYangDitabrak = null;
                if (tombolUI != null && tombolUI.activeSelf)
                {
                    tombolUI.SetActive(false);
                }
            }
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
            colliderTerakhir = null;

            // Mulai dialog sesuai nama node yang ada di benda tersebut!
            if (mesinCerita != null)
            {
                mesinCerita.StartDialogue(nodeTarget);
            }
        }
    }
}