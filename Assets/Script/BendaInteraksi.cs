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

    private Collider areaKlik;

    [Header("Efek Layar / Lampu Panggilan")]
    [Tooltip("Objek visual/lampu yang menyala ketika ada panggilan masuk (misal: Point Light atau Plane di layar HP)")]
    public GameObject efekLayar;

    [Header("Materi Layar HP (Opsional)")]
    [Tooltip("Material lockscreen wallpaper saat ada notifikasi chat masuk")]
    public Material matLockscreen;
    [Tooltip("Material panggilan masuk saat ada telepon")]
    public Material matPanggilan;

    void Awake()
    {
        areaKlik = GetComponent<Collider>();
        if (areaKlik == null) areaKlik = GetComponentInChildren<Collider>();

        if (!aktifSaatMulai && areaKlik != null)
        {
            areaKlik.enabled = false;
        }

        if (sembunyikanSaatMulai)
        {
            SembunyikanBenda();
        }

        if (efekLayar != null)
        {
            efekLayar.SetActive(false);
        }
    }

    [YarnCommand("matikan_interaksi")]
    public void MatikanInteraksi()
    {
        if (areaKlik == null) areaKlik = GetComponent<Collider>();
        if (areaKlik == null) areaKlik = GetComponentInChildren<Collider>();
        if (areaKlik != null) areaKlik.enabled = false;
    }

    [YarnCommand("aktifkan_interaksi")]
    public void AktifkanInteraksi()
    {
        if (areaKlik == null) areaKlik = GetComponent<Collider>();
        if (areaKlik == null) areaKlik = GetComponentInChildren<Collider>();
        if (areaKlik != null) areaKlik.enabled = true;
    }

    // Mengganti target node dari dalam Yarn secara dinamis
    [YarnCommand("ubah_node")]
    public void UbahNode(string nodeBaru)
    {
        namaNodeYarn = nodeBaru;
    }

    [YarnCommand("ubah_teks_interaksi")]
    public void UbahTeksInteraksi(string teksBaru)
    {
        teksTombolInteraksi = teksBaru;
    }

    // Menyalakan efek layar HP (misal ada panggilan masuk atau chat masuk)
    [YarnCommand("nyalakan_layar")]
    public void NyalakanLayar(string tipe = "lockscreen")
    {
        string t = tipe != null ? tipe.ToLower().Trim() : "";
        bool isTelepon = t.Contains("tel") || t.Contains("panggilan") || t.Contains("call");

        if (efekLayar != null)
        {
            var mr = efekLayar.GetComponent<Renderer>();
            if (mr != null)
            {
                if (isTelepon && matPanggilan != null)
                {
                    mr.sharedMaterial = matPanggilan;
                }
                else if (matLockscreen != null)
                {
                    mr.sharedMaterial = matLockscreen;
                }
            }
            efekLayar.SetActive(true);
        }
    }

    // Mematikan efek layar HP
    [YarnCommand("matikan_layar")]
    public void MatikanLayar()
    {
        if (efekLayar != null)
        {
            efekLayar.SetActive(false);
        }
    }

    // Menyembunyikan tampilan visual objek (misal: botol saat diangkat/diminum)
    [YarnCommand("sembunyikan_benda")]
    public void SembunyikanBenda()
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in allRenderers)
        {
            r.enabled = false;
        }
        if (areaKlik == null) areaKlik = GetComponent<Collider>();
        if (areaKlik == null) areaKlik = GetComponentInChildren<Collider>();
        if (areaKlik != null) areaKlik.enabled = false;
    }

    // Memunculkan kembali tampilan visual objek (misal: botol ditaruh kembali ke meja)
    [YarnCommand("munculkan_benda")]
    public void MunculkanBenda()
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in allRenderers)
        {
            r.enabled = true;
        }
    }
}
