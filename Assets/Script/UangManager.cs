using System.Collections;
using System.Globalization;
using UnityEngine;
using TMPro;
using Yarn.Unity; // Wajib dipanggil agar bisa berkomunikasi dengan Yarn

public class UangManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public TextMeshProUGUI teksUang;
    public TextMeshProUGUI teksFloatingMinus;
    public CanvasGroup canvasGroupFloating;

    [Header("Pengaturan Animasi")]
    [SerializeField] private float jarakTurun = 45f;
    [SerializeField] private float durasiAnimasi = 1.4f;
    [SerializeField] private float durasiHitungSaldo = 0.6f;
    [SerializeField] private Color warnaTeksMinus = new Color(0.95f, 0.15f, 0.15f, 1f); // Merah tajam

    private int saldo = 5000000; // Uang awal Raka
    private readonly CultureInfo cultureIndo = new CultureInfo("id-ID");
    private Coroutine coroutineAnimasi;
    private Vector2 posisiAwalFloating;
    private Vector3 skalaAwalFloating = Vector3.one;
    private Color warnaAsliTeksUang = Color.black;

    void Awake()
    {
        if (teksUang != null)
        {
            warnaAsliTeksUang = teksUang.color;
        }

        if (teksFloatingMinus == null)
        {
            // Coba cari TextFloatingMinus di child BoxPorto secara otomatis
            Transform boxPorto = teksUang != null ? teksUang.transform.parent : null;
            if (boxPorto != null)
            {
                Transform t = boxPorto.Find("TextFloatingMinus");
                if (t != null)
                {
                    teksFloatingMinus = t.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        if (teksFloatingMinus != null)
        {
            posisiAwalFloating = teksFloatingMinus.rectTransform.anchoredPosition;
            skalaAwalFloating = teksFloatingMinus.rectTransform.localScale;

            if (canvasGroupFloating == null)
            {
                canvasGroupFloating = teksFloatingMinus.GetComponent<CanvasGroup>();
                if (canvasGroupFloating == null)
                {
                    canvasGroupFloating = teksFloatingMinus.gameObject.AddComponent<CanvasGroup>();
                }
            }

            teksFloatingMinus.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UpdateLayar();
    }

    // [YarnCommand] membuat fungsi ini bisa dipanggil langsung dari teks naskah Yarn!
    [YarnCommand("potong_uang")]
    public void PotongUang(int jumlahPotongan)
    {
        int saldoSebelumnya = saldo;
        saldo -= jumlahPotongan;
        if (saldo < 0) saldo = 0;

        if (coroutineAnimasi != null)
        {
            StopCoroutine(coroutineAnimasi);
        }

        coroutineAnimasi = StartCoroutine(AnimasiPotongUangRoutine(jumlahPotongan, saldoSebelumnya, saldo));
    }

    private IEnumerator AnimasiPotongUangRoutine(int jumlahPotongan, int saldoAwal, int saldoAkhir)
    {
        // 1. Setup teks melayang
        if (teksFloatingMinus != null)
        {
            teksFloatingMinus.gameObject.SetActive(true);
            teksFloatingMinus.text = "- Rp " + jumlahPotongan.ToString("N0", cultureIndo);
            teksFloatingMinus.color = warnaTeksMinus;
            teksFloatingMinus.rectTransform.anchoredPosition = posisiAwalFloating;
            teksFloatingMinus.rectTransform.localScale = skalaAwalFloating;

            if (canvasGroupFloating != null)
            {
                canvasGroupFloating.alpha = 1f;
            }
        }

        // Efek visual teks tabungan berubah warna merah sebentar ("efek nyesek")
        Color warnaFlash = new Color(0.95f, 0.2f, 0.2f, 1f);

        float elapsed = 0f;
        Vector2 targetPos = posisiAwalFloating + new Vector2(0f, -jarakTurun);

        while (elapsed < durasiAnimasi)
        {
            elapsed += Time.deltaTime;
            float tTotal = Mathf.Clamp01(elapsed / durasiAnimasi);

            // Pergerakan teks melayang ke bawah secara halus
            if (teksFloatingMinus != null)
            {
                float tGerak = Mathf.SmoothStep(0f, 1f, tTotal);
                teksFloatingMinus.rectTransform.anchoredPosition = Vector2.Lerp(posisiAwalFloating, targetPos, tGerak);

                // Sedikit hentakan/pop di awal (0 - 0.15 detik)
                if (elapsed <= 0.15f)
                {
                    float tPop = elapsed / 0.15f;
                    teksFloatingMinus.rectTransform.localScale = Vector3.Lerp(skalaAwalFloating * 1.2f, skalaAwalFloating, tPop);
                }
                else
                {
                    teksFloatingMinus.rectTransform.localScale = skalaAwalFloating;
                }

                // Menghilang perlahan (fade out) setelah 0.4 detik
                if (elapsed > 0.4f)
                {
                    float tFade = Mathf.Clamp01((elapsed - 0.4f) / (durasiAnimasi - 0.4f));
                    if (canvasGroupFloating != null)
                    {
                        canvasGroupFloating.alpha = Mathf.Lerp(1f, 0f, tFade);
                    }
                    else
                    {
                        Color c = warnaTeksMinus;
                        c.a = Mathf.Lerp(1f, 0f, tFade);
                        teksFloatingMinus.color = c;
                    }
                }
            }

            // Animasi angka saldo berkurang drastis secara bergulir (roll down)
            if (teksUang != null)
            {
                float tSaldo = Mathf.Clamp01(elapsed / durasiHitungSaldo);
                float smoothSaldo = Mathf.SmoothStep(0f, 1f, tSaldo);
                int saldoDisplay = (int)Mathf.Lerp(saldoAwal, saldoAkhir, smoothSaldo);
                teksUang.text = saldoDisplay.ToString("N0", cultureIndo);

                // Transisi warna teks saldo dari merah kembali ke warna asli
                float tWarna = Mathf.Clamp01(elapsed / (durasiHitungSaldo * 1.2f));
                teksUang.color = Color.Lerp(warnaFlash, warnaAsliTeksUang, tWarna);
            }

            yield return null;
        }

        // Reset dan sembunyikan floating text setelah animasi selesai
        if (teksFloatingMinus != null)
        {
            teksFloatingMinus.gameObject.SetActive(false);
            teksFloatingMinus.rectTransform.anchoredPosition = posisiAwalFloating;
            teksFloatingMinus.rectTransform.localScale = skalaAwalFloating;
        }

        // Pastikan angka dan warna akhir tepat
        if (teksUang != null)
        {
            teksUang.text = saldoAkhir.ToString("N0", cultureIndo);
            teksUang.color = warnaAsliTeksUang;
        }

        coroutineAnimasi = null;
    }

    [YarnCommand("kembali_ke_menu")]
    public void KembaliKeMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void SetSaldo(int nilaiBaru)
    {
        saldo = nilaiBaru;
        UpdateLayar();
    }

    void UpdateLayar()
    {
        if (teksUang != null)
        {
            teksUang.text = saldo.ToString("N0", cultureIndo);
            teksUang.color = warnaAsliTeksUang;
        }
    }
}