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

    [Header("Referensi Sistem Interaksi")]
    public SistemInteraksi sistemInteraksi;

    [Header("Pengaturan Transisi")]
    [SerializeField] private float durasiTransisiDefault = 2.0f;
    [SerializeField] private bool mulaiDariPintu = true;

    private Coroutine coroutineTransisi;

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

        if (sistemInteraksi == null)
        {
            sistemInteraksi = Object.FindObjectOfType<SistemInteraksi>();
        }
    }

    void Start()
    {
        if (mulaiDariPintu && kameraPintu != null)
        {
            AktifkanKameraLangsung(kameraPintu, kameraMeja);
        }
        else if (kameraMeja != null)
        {
            AktifkanKameraLangsung(kameraMeja, kameraPintu);
        }
    }

    private void AktifkanKameraLangsung(Camera targetAktif, Camera targetNonaktif)
    {
        if (targetNonaktif != null)
        {
            targetNonaktif.gameObject.SetActive(false);
            AudioListener al = targetNonaktif.GetComponent<AudioListener>();
            if (al != null) al.enabled = false;
        }

        if (targetAktif != null)
        {
            targetAktif.gameObject.SetActive(true);
            AudioListener al = targetAktif.GetComponent<AudioListener>();
            if (al != null) al.enabled = true;

            if (sistemInteraksi != null)
            {
                sistemInteraksi.kameraPemain = targetAktif;
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

    [YarnCommand("pindah_ke_meja")]
    public void TransisiKeMeja(float durasi = 2.0f)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);
        coroutineTransisi = StartCoroutine(ProsesTransisiKamera(kameraPintu, kameraMeja, durasi, true));
    }

    [YarnCommand("pindah_ke_pintu")]
    public void TransisiKePintu(float durasi = 2.0f)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);
        coroutineTransisi = StartCoroutine(ProsesTransisiKamera(kameraMeja, kameraPintu, durasi, false));
    }

    [YarnCommand("set_kamera_instan")]
    public void SetKameraInstan(string namaKamera)
    {
        if (coroutineTransisi != null) StopCoroutine(coroutineTransisi);

        string target = namaKamera != null ? namaKamera.ToLower().Trim() : "";
        if (target.Contains("meja") || target.Contains("desk"))
        {
            AktifkanKameraLangsung(kameraMeja, kameraPintu);
        }
        else
        {
            AktifkanKameraLangsung(kameraPintu, kameraMeja);
        }
    }

    [Header("Pengaturan Efek Shake Kamera")]
    private Coroutine coroutineShake;

    [YarnCommand("shake_kamera")]
    public void TriggerShakeKamera(float durasi = 0.6f, float kekuatan = 0.08f)
    {
        if (coroutineShake != null) StopCoroutine(coroutineShake);
        coroutineShake = StartCoroutine(ProsesShakeKamera(durasi, kekuatan));
    }

    private IEnumerator ProsesShakeKamera(float durasi, float kekuatan)
    {
        Camera camAktif = (kameraMeja != null && kameraMeja.gameObject.activeInHierarchy) ? kameraMeja : kameraPintu;
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
        if (camAwal == null || camTujuan == null) yield break;

        // Jika durasi kecil, langsung ganti instan
        if (durasi <= 0.05f)
        {
            AktifkanKameraLangsung(camTujuan, camAwal);
            yield break;
        }

        // Nonaktifkan putar kamera sementara agar tidak bertabrakan dengan pergerakan otomatis
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

            // Sedikit efek langkah kaki/head bobbing saat berjalan menuju meja
            if (menujuMeja && t < 0.85f)
            {
                float bobbing = Mathf.Sin(t * Mathf.PI * 4f) * 0.02f;
                targetPos.y += bobbing;
            }

            camAwal.transform.position = targetPos;
            camAwal.transform.rotation = Quaternion.Slerp(rotAwal, rotTujuan, smoothT);

            yield return null;
        }

        // Kembalikan posisi awal camAwal ke posisi aslinya
        camAwal.transform.position = posAwal;
        camAwal.transform.rotation = rotAwal;
        if (kmAwal != null) kmAwal.enabled = true;

        // Aktifkan kamera tujuan dan matikan kamera awal
        AktifkanKameraLangsung(camTujuan, camAwal);

        KameraMobile kmTujuan = camTujuan.GetComponent<KameraMobile>();
        if (kmTujuan != null) kmTujuan.enabled = true;

        coroutineTransisi = null;
    }
}
