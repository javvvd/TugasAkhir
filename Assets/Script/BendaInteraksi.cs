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

    private Collider areaKlik;

    [Header("Efek Layar / Lampu Panggilan")]
    [Tooltip("Objek visual/lampu yang menyala ketika ada panggilan masuk (misal: Point Light atau Plane di layar HP)")]
    public GameObject efekLayar;

    [Tooltip("Jika dicentang, efek layar akan berkedip-kedip saat menyala seperti telepon berdering")]
    public bool kedipSaatMenyala = true;
    public float intervalKedip = 0.5f;
    private float timerKedip = 0f;
    private bool sedangMenyala = false;

    void Awake()
    {
        areaKlik = GetComponent<Collider>();
    }

    void Start()
    {
        if (areaKlik == null)
        {
            areaKlik = GetComponent<Collider>();
        }

        if (!aktifSaatMulai && areaKlik != null)
        {
            areaKlik.enabled = false;
        }

        if (efekLayar != null)
        {
            efekLayar.SetActive(false);
        }
    }

    void Update()
    {
        if (sedangMenyala && kedipSaatMenyala && efekLayar != null)
        {
            timerKedip -= Time.deltaTime;
            if (timerKedip <= 0f)
            {
                efekLayar.SetActive(!efekLayar.activeSelf);
                timerKedip = intervalKedip;
            }
        }
    }

    [YarnCommand("matikan_interaksi")]
    public void MatikanInteraksi()
    {
        if (areaKlik == null) areaKlik = GetComponent<Collider>();
        if (areaKlik != null) areaKlik.enabled = false;
    }

    [YarnCommand("aktifkan_interaksi")]
    public void AktifkanInteraksi()
    {
        if (areaKlik == null) areaKlik = GetComponent<Collider>();
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

    // Menyalakan efek layar HP (misal ada panggilan masuk)
    [YarnCommand("nyalakan_layar")]
    public void NyalakanLayar()
    {
        sedangMenyala = true;
        timerKedip = intervalKedip;
        if (efekLayar != null)
        {
            efekLayar.SetActive(true);
        }
    }

    // Mematikan efek layar HP
    [YarnCommand("matikan_layar")]
    public void MatikanLayar()
    {
        sedangMenyala = false;
        if (efekLayar != null)
        {
            efekLayar.SetActive(false);
        }
    }
}