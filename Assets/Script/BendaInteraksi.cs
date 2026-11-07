using UnityEngine;
using Yarn.Unity;

public class BendaInteraktif : MonoBehaviour
{
    [Header("Pengaturan Dialog Yarn")]
    public string namaNodeYarn;
    [Tooltip("Teks yang muncul pada tombol UI saat benda ini disorot (misal: 'Buka HP', 'Buka Pintu')")]
    public string teksTombolInteraksi = "Interaksi";

    [Header("Pengaturan Status Awal")]
    [Tooltip("Jika false, collider interaksi dinonaktifkan di awal game dan menunggu perintah dari Yarn")]
    public bool aktifSaatMulai = true;
    [Tooltip("Jika true, seluruh renderer disembunyikan di awal game dan menunggu perintah munculkan_benda")]
    public bool sembunyikanSaatMulai = false;

    [Header("Efek Layar / Lampu Panggilan")]
    [Tooltip("Objek visual/lampu yang menyala ketika ada panggilan masuk (misal: Point Light atau Plane di layar HP)")]
    public GameObject efekLayar;

    [Header("Materi Layar HP (Opsional)")]
    [Tooltip("Material lockscreen wallpaper saat ada notifikasi chat masuk")]
    public Material matLockscreen;
    [Tooltip("Material panggilan masuk saat ada telepon")]
    public Material matPanggilan;

    private Collider areaKlik;
    private Renderer[] cachedRenderers;
    private Renderer efekLayarRenderer;

    void Awake()
    {
        areaKlik = GetComponent<Collider>();
        if (areaKlik == null) areaKlik = GetComponentInChildren<Collider>();

        cachedRenderers = GetComponentsInChildren<Renderer>(true);

        if (efekLayar != null)
        {
            efekLayarRenderer = efekLayar.GetComponent<Renderer>();
            efekLayar.SetActive(false);
        }

        if (!aktifSaatMulai && areaKlik != null)
        {
            areaKlik.enabled = false;
        }

        if (sembunyikanSaatMulai)
        {
            SembunyikanBenda();
        }
    }

    [YarnCommand("matikan_interaksi")]
    public void MatikanInteraksi()
    {
        if (areaKlik != null) areaKlik.enabled = false;
    }

    [YarnCommand("aktifkan_interaksi")]
    public void AktifkanInteraksi()
    {
        if (areaKlik != null) areaKlik.enabled = true;
    }

    [YarnCommand("ubah_node")]
    public void UbahNode(string nodeBaru)
    {
        namaNodeYarn = nodeBaru;
    }

    [YarnCommand("nyalakan_layar")]
    public void NyalakanLayar(string tipe = "lockscreen")
    {
        if (efekLayar == null) return;

        string t = tipe != null ? tipe.ToLower().Trim() : "";
        bool isTelepon = t.Contains("tel") || t.Contains("panggilan") || t.Contains("call");

        if (efekLayarRenderer != null)
        {
            if (isTelepon && matPanggilan != null)
            {
                efekLayarRenderer.sharedMaterial = matPanggilan;
            }
            else if (matLockscreen != null)
            {
                efekLayarRenderer.sharedMaterial = matLockscreen;
            }
        }
        efekLayar.SetActive(true);
    }

    [YarnCommand("matikan_layar")]
    public void MatikanLayar()
    {
        if (efekLayar != null)
        {
            efekLayar.SetActive(false);
        }
    }

    [YarnCommand("sembunyikan_benda")]
    public void SembunyikanBenda()
    {
        if (cachedRenderers == null) cachedRenderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null) cachedRenderers[i].enabled = false;
        }
        if (areaKlik != null) areaKlik.enabled = false;
    }

    [YarnCommand("munculkan_benda")]
    public void MunculkanBenda()
    {
        if (cachedRenderers == null) cachedRenderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null) cachedRenderers[i].enabled = true;
        }
    }
}
