using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class PengendaliKamera : MonoBehaviour
{
    public static PengendaliKamera Instance { get; private set; }

    [Header("Referensi Kamera")]
    [Tooltip("Kamera di pintu masuk (perspektif saat baru pulang berdiri)")]
    public Camera kameraPintu;

    [Tooltip("Kamera di meja belajar (perspektif saat duduk menghadap laptop/HP)")]
    public Camera kameraMeja;

    [Tooltip("Kamera di kasur (perspektif saat rebahan/depresi menatap plafon)")]
    public Camera kameraKasur;

    [Tooltip("Kamera di warkop (perspektif saat nongkrong bersama Bima)")]
    public Camera kameraWarkop;

    [Header("Optimasi Lingkungan")]
    [Tooltip("Root objek AreaWarkop yang akan dimatikan otomatis saat pemain berada di kamar kos untuk menghemat resource.")]
    public GameObject areaWarkop;

    [Header("Referensi Sistem Interaksi")]
    public SistemInteraksi sistemInteraksi;

    [Header("Pengaturan Transisi")]
    [SerializeField] private float durasiTransisiDefault = 2.0f;
    [SerializeField] private bool mulaiDariPintu = true;

    private Coroutine coroutineTransisi;
    private Coroutine coroutineShake;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Auto-find kamera jika belum dihubungkan di Inspector
        if (kameraMeja == null)
        {
            Transform t = transform.Find("Main Camera");
            if (t != null) kameraMeja = t.GetComponent<Camera>();
        }

        if (kameraPintu == null)
        {
            Transform t = transform.Find("Main Camera (1)");
            if (t != null) kameraPintu = t.GetComponent<Camera>();
        }

        if (kameraKasur == null)
        {
            Transform t = transform.Find("Main Camera (2)");
            if (t != null) kameraKasur = t.GetComponent<Camera>();
        }

        if (kameraWarkop == null)
        {
            Transform t = transform.Find("KameraWarkop");
            if (t == null) t = transform.Find("Main Camera (3)");
            if (t != null) kameraWarkop = t.GetComponent<Camera>();
        }

        if (areaWarkop == null)
        {
            areaWarkop = GameObject.Find("AreaWarkop");
            if (areaWarkop == null)
            {
                // Jika AreaWarkop sedang nonaktif di scene, cari dengan Resources.FindObjectsOfTypeAll
                var allAreas = Resources.FindObjectsOfTypeAll<Transform>();
                foreach (var tr in allAreas)
                {
                    if (tr.gameObject.name == "AreaWarkop" && tr.parent == null)
                    {
                        areaWarkop = tr.gameObject;
                        break;
                    }
                }
            }
        }

        if (sistemInteraksi == null)
        {
            sistemInteraksi = Object.FindObjectOfType<SistemInteraksi>();
        }
    }

    void Start()
    {
        if (mulaiDariPintu && kameraPintu != null)
        {
            AktifkanSatuKamera(kameraPintu);
        }
        else if (kameraMeja != null)
        {
            AktifkanSatuKamera(kameraMeja);
        }
    }

    public Camera GetKameraAktif()
    {
        if (kameraMeja != null && kameraMeja.gameObject.activeInHierarchy) return kameraMeja;
        if (kameraKasur != null && kameraKasur.gameObject.activeInHierarchy) return kameraKasur;
        if (kameraPintu != null && kameraPintu.gameObject.activeInHierarchy) return kameraPintu;
        if (kameraWarkop != null && kameraWarkop.gameObject.activeInHierarchy) return kameraWarkop;
        return Camera.main;
    }

    private void AktifkanSatuKamera(Camera targetAktif)
    {
        Camera[] allCams = { kameraPintu, kameraMeja, kameraKasur, kameraWarkop };
        foreach (var c in allCams)
        {
            if (c != null && c != targetAktif)
            {
                c.gameObject.SetActive(false);
                AudioListener al = c.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
                KameraMobile km = c.GetComponent<KameraMobile>();
                if (km != null) km.enabled = false;
            }
        }

        if (targetAktif != null)
        {
            targetAktif.gameObject.SetActive(true);
            AudioListener al = targetAktif.GetComponent<AudioListener>();
            if (al != null) al.enabled = true;

            KameraMobile km = targetAktif.GetComponent<KameraMobile>();
            if (km != null) km.enabled = true;

            if (sistemInteraksi != null)
            {
                sistemInteraksi.kameraPemain = targetAktif;
            }
        }

        // Optimasi: Matikan AreaWarkop jika sedang di kamar kos, nyalakan hanya saat kamera Warkop aktif
        if (areaWarkop != null)
        {
            bool diWarkop = (targetAktif == kameraWarkop);
            if (areaWarkop.activeSelf != diWarkop)
            {
                areaWarkop.SetActive(diWarkop);
            }
        }
    }

    [YarnCommand("ganti_kamera_meja")]
    public void GantiKameraMeja()
    {
        TransisiKeMeja(durasiTransisiDefault);
    }

    [YarnCommand("ganti_kamera_pintu")]
    public void GantiKameraPintu()
    {
        TransisiKePintu(durasiTransisiDefault);
    }

    [YarnCommand("ganti_kamera_kasur")]
    public void GantiKameraKasur()
    {
        AktifkanSatuKamera(kameraKasur);
    }

    [YarnCommand("ganti_kamera_warkop")]
    public void GantiKameraWarkop()
    {
        AktifkanSatuKamera(kameraWarkop);
    }

    [YarnCommand("pindah_ke_meja")]
    public void TransisiKeMeja(float durasi = 2.0f)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);
        Camera camAwal = GetKameraAktif();
        coroutineTransisi = StartCoroutine(ProsesTransisiKamera(camAwal, kameraMeja, durasi, true));
    }

    [YarnCommand("pindah_ke_pintu")]
    public void TransisiKePintu(float durasi = 2.0f)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);
        Camera camAwal = GetKameraAktif();
        coroutineTransisi = StartCoroutine(ProsesTransisiKamera(camAwal, kameraPintu, durasi, false));
    }

    [YarnCommand("pindah_ke_kasur")]
    public void TransisiKeKasur(float durasi = 2.0f)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);
        Camera camAwal = GetKameraAktif();
        coroutineTransisi = StartCoroutine(ProsesTransisiKamera(camAwal, kameraKasur, durasi, false));
    }

    [YarnCommand("set_kamera_instan")]
    public void SetKameraInstan(string namaKamera)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);

        string target = namaKamera != null ? namaKamera.ToLower().Trim() : "";
        if (target.Contains("warkop") || target.Contains("kafe") || target.Contains("kopi"))
        {
            AktifkanSatuKamera(kameraWarkop);
        }
        else if (target.Contains("kasur") || target.Contains("bed") || target.Contains("rebahan"))
        {
            AktifkanSatuKamera(kameraKasur);
        }
        else if (target.Contains("meja") || target.Contains("desk"))
        {
            AktifkanSatuKamera(kameraMeja);
        }
        else
        {
            AktifkanSatuKamera(kameraPintu);
        }
    }

    [YarnCommand("shake_kamera")]
    public void TriggerShakeKamera(float durasi = 0.6f, float kekuatan = 0.08f)
    {
        if (coroutineShake != null) StopCoroutine(coroutineShake);
        coroutineShake = StartCoroutine(ProsesShakeKamera(durasi, kekuatan));
    }

    private IEnumerator ProsesShakeKamera(float durasi, float kekuatan)
    {
        Camera camAktif = GetKameraAktif();
        if (camAktif == null) yield break;

        Vector3 posAwal = camAktif.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < durasi)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(elapsed / durasi); // Decay peredam getaran

            float offsetX = (Random.value * 2f - 1f) * kekuatan * t;
            float offsetY = (Random.value * 2f - 1f) * kekuatan * t;

            camAktif.transform.localPosition = posAwal + new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }

        camAktif.transform.localPosition = posAwal;
        coroutineShake = null;
    }

    private IEnumerator ProsesTransisiKamera(Camera camAwal, Camera camTujuan, float durasi, bool menujuMeja)
    {
        if (camAwal == null || camTujuan == null || camAwal == camTujuan)
        {
            AktifkanSatuKamera(camTujuan);
            yield break;
        }

        if (durasi <= 0.05f)
        {
            AktifkanSatuKamera(camTujuan);
            yield break;
        }

        KameraMobile kmAwal = camAwal.GetComponent<KameraMobile>();
        if (kmAwal != null) kmAwal.enabled = false;

        Vector3 posAwal = camAwal.transform.position;
        Quaternion rotAwal = camAwal.transform.rotation;

        Vector3 posTujuan = camTujuan.transform.position;
        Quaternion rotTujuan = camTujuan.transform.rotation;

        float elapsed = 0f;

        while (elapsed < durasi)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durasi);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 targetPos = Vector3.Lerp(posAwal, posTujuan, smoothT);

            if (menujuMeja && t < 0.85f)
            {
                float bobbing = Mathf.Sin(t * Mathf.PI * 4f) * 0.02f;
                targetPos.y += bobbing;
            }

            camAwal.transform.position = targetPos;
            camAwal.transform.rotation = Quaternion.Slerp(rotAwal, rotTujuan, smoothT);

            yield return null;
        }

        camAwal.transform.position = posAwal;
        camAwal.transform.rotation = rotAwal;
        if (kmAwal != null) kmAwal.enabled = true;

        AktifkanSatuKamera(camTujuan);

        KameraMobile kmTujuan = camTujuan.GetComponent<KameraMobile>();
        if (kmTujuan != null) kmTujuan.enabled = true;

        coroutineTransisi = null;
    }
}
