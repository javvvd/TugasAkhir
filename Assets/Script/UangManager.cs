using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class UangManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public TextMeshProUGUI teksUang;
    public TextMeshProUGUI teksLabelTabungan; // TextUang (1) - "Tabungan :"
    public TextMeshProUGUI teksFloatingMinus;
    public CanvasGroup canvasGroupFloating;
    public RectTransform rectBoxPorto; // Untuk efek getaran pada box porto

    [Header("Pengaturan Animasi")]
    [SerializeField] private float jarakTurun = 35f;
    [SerializeField] private float durasiAnimasi = 1.8f;
    [SerializeField] private float durasiHitungSaldo = 0.8f;
    [SerializeField] private Color warnaTeksMinus = new Color(1f, 0.15f, 0.15f, 1f); // Merah tajam menyala
    [SerializeField] private Color warnaMerahBangkrut = new Color(1f, 0.15f, 0.15f, 1f);

    private int saldo = 5000000; // Uang awal Raka
    private readonly CultureInfo cultureIndo = new CultureInfo("id-ID");
    private Coroutine coroutineAnimasi;
    private Vector2 posisiAwalFloating;
    private Vector2 posisiAwalBoxPorto;
    private Vector3 skalaAwalFloating = Vector3.one;
    private Color warnaAsliTeksUang = Color.white;
    private bool isBangkrut = false;

    void Awake()
    {
        if (teksUang != null)
        {
            warnaAsliTeksUang = teksUang.color;
            Transform parentBox = teksUang.transform.parent;
            if (parentBox != null)
            {
                if (rectBoxPorto == null) rectBoxPorto = parentBox.GetComponent<RectTransform>();
                if (teksLabelTabungan == null)
                {
                    Transform tLabel = parentBox.Find("TextUang (1)");
                    if (tLabel != null) teksLabelTabungan = tLabel.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        if (rectBoxPorto != null)
        {
            posisiAwalBoxPorto = rectBoxPorto.anchoredPosition;
            foreach (var g in rectBoxPorto.GetComponentsInChildren<Graphic>(true))
            {
                g.raycastTarget = false;
                if (g is TextMeshProUGUI tmp)
                {
                    tmp.margin = Vector4.zero;
                    tmp.enableWordWrapping = false;
                }
            }
        }

        if (teksFloatingMinus == null)
        {
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

    [YarnCommand("set_bangkrut")]
    public void SetBangkrut()
    {
        int saldoSebelumnya = saldo;
        saldo = 0;
        isBangkrut = true;

        if (coroutineAnimasi != null)
        {
            StopCoroutine(coroutineAnimasi);
        }

        coroutineAnimasi = StartCoroutine(AnimasiBangkrutRoutine(saldoSebelumnya));
    }

    [YarnCommand("set_saldo")]
    public void SetSaldo(int nilaiBaru)
    {
        saldo = nilaiBaru;
        isBangkrut = (saldo <= 0);
        UpdateLayar();
    }

    private IEnumerator AnimasiPotongUangRoutine(int jumlahPotongan, int saldoAwal, int saldoAkhir)
    {
        if (teksFloatingMinus != null)
        {
            teksFloatingMinus.gameObject.SetActive(true);
            teksFloatingMinus.text = "- Rp " + jumlahPotongan.ToString("N0", cultureIndo);
            teksFloatingMinus.color = warnaTeksMinus;
            teksFloatingMinus.fontSize = 26f;
            teksFloatingMinus.fontStyle = FontStyles.Bold;
            teksFloatingMinus.rectTransform.anchoredPosition = posisiAwalFloating;
            teksFloatingMinus.rectTransform.localScale = skalaAwalFloating * 1.35f;

            if (canvasGroupFloating != null)
            {
                canvasGroupFloating.alpha = 1f;
            }
        }

        Color warnaFlash = warnaTeksMinus;
        float elapsed = 0f;
        Vector2 targetPos = posisiAwalFloating + new Vector2(0f, -jarakTurun);

        while (elapsed < durasiAnimasi)
        {
            elapsed += Time.deltaTime;
            float tTotal = Mathf.Clamp01(elapsed / durasiAnimasi);

            Vector2 shakeOffset = Vector2.zero;
            if (elapsed < 0.45f)
            {
                float shakeDamp = 1f - (elapsed / 0.45f);
                float shakeX = Mathf.Sin(elapsed * 55f) * 10f * shakeDamp;
                float shakeY = Mathf.Cos(elapsed * 40f) * 5f * shakeDamp;
                shakeOffset = new Vector2(shakeX, shakeY);

                if (rectBoxPorto != null)
                {
                    rectBoxPorto.anchoredPosition = posisiAwalBoxPorto + new Vector2(shakeX * 0.4f, shakeY * 0.4f);
                }
            }
            else if (rectBoxPorto != null)
            {
                rectBoxPorto.anchoredPosition = posisiAwalBoxPorto;
            }

            if (teksFloatingMinus != null)
            {
                float tGerak = Mathf.SmoothStep(0f, 1f, tTotal);
                teksFloatingMinus.rectTransform.anchoredPosition = Vector2.Lerp(posisiAwalFloating, targetPos, tGerak) + shakeOffset;

                if (elapsed <= 0.2f)
                {
                    float tPop = elapsed / 0.2f;
                    teksFloatingMinus.rectTransform.localScale = Vector3.Lerp(skalaAwalFloating * 1.35f, skalaAwalFloating, tPop);
                }
                else
                {
                    teksFloatingMinus.rectTransform.localScale = skalaAwalFloating;
                }

                if (elapsed > 0.7f)
                {
                    float tFade = Mathf.Clamp01((elapsed - 0.7f) / (durasiAnimasi - 0.7f));
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

            if (teksUang != null)
            {
                float tSaldo = Mathf.Clamp01(elapsed / durasiHitungSaldo);
                float smoothSaldo = Mathf.SmoothStep(0f, 1f, tSaldo);
                int saldoDisplay = (int)Mathf.Lerp(saldoAwal, saldoAkhir, smoothSaldo);
                teksUang.text = saldoDisplay.ToString("N0", cultureIndo);

                if (saldoAkhir <= 0)
                {
                    teksUang.color = warnaMerahBangkrut;
                }
                else
                {
                    float tWarna = Mathf.Clamp01(elapsed / (durasiHitungSaldo * 1.3f));
                    teksUang.color = Color.Lerp(warnaFlash, warnaAsliTeksUang, tWarna);
                }
            }

            yield return null;
        }

        if (rectBoxPorto != null)
        {
            rectBoxPorto.anchoredPosition = posisiAwalBoxPorto;
        }

        if (teksFloatingMinus != null)
        {
            teksFloatingMinus.gameObject.SetActive(false);
            teksFloatingMinus.rectTransform.anchoredPosition = posisiAwalFloating;
            teksFloatingMinus.rectTransform.localScale = skalaAwalFloating;
        }

        if (teksUang != null)
        {
            teksUang.text = saldoAkhir.ToString("N0", cultureIndo);
            teksUang.color = (saldoAkhir <= 0) ? warnaMerahBangkrut : warnaAsliTeksUang;
        }

        coroutineAnimasi = null;
    }

    private IEnumerator AnimasiBangkrutRoutine(int saldoAwal)
    {
        if (teksFloatingMinus != null)
        {
            teksFloatingMinus.gameObject.SetActive(true);
            teksFloatingMinus.text = "- Rp " + saldoAwal.ToString("N0", cultureIndo) + " (LUDES)";
            teksFloatingMinus.color = warnaMerahBangkrut;
            teksFloatingMinus.fontSize = 26f;
            teksFloatingMinus.fontStyle = FontStyles.Bold;
            teksFloatingMinus.rectTransform.anchoredPosition = posisiAwalFloating;
            teksFloatingMinus.rectTransform.localScale = skalaAwalFloating * 1.4f;

            if (canvasGroupFloating != null)
            {
                canvasGroupFloating.alpha = 1f;
            }
        }

        if (teksLabelTabungan != null)
        {
            teksLabelTabungan.text = "Tabungan (LUDES) :";
            teksLabelTabungan.color = warnaMerahBangkrut;
        }

        float durasiBangkrut = 2.2f;
        float elapsed = 0f;
        Vector2 targetPos = posisiAwalFloating + new Vector2(0f, -jarakTurun);

        while (elapsed < durasiBangkrut)
        {
            elapsed += Time.deltaTime;
            float tTotal = Mathf.Clamp01(elapsed / durasiBangkrut);

            Vector2 shakeOffset = Vector2.zero;
            if (elapsed < 0.7f)
            {
                float shakeDamp = 1f - (elapsed / 0.7f);
                float shakeX = Mathf.Sin(elapsed * 65f) * 14f * shakeDamp;
                float shakeY = Mathf.Cos(elapsed * 45f) * 7f * shakeDamp;
                shakeOffset = new Vector2(shakeX, shakeY);

                if (rectBoxPorto != null)
                {
                    rectBoxPorto.anchoredPosition = posisiAwalBoxPorto + new Vector2(shakeX * 0.5f, shakeY * 0.5f);
                }
            }
            else if (rectBoxPorto != null)
            {
                rectBoxPorto.anchoredPosition = posisiAwalBoxPorto;
            }

            if (teksFloatingMinus != null)
            {
                float tGerak = Mathf.SmoothStep(0f, 1f, tTotal);
                teksFloatingMinus.rectTransform.anchoredPosition = Vector2.Lerp(posisiAwalFloating, targetPos, tGerak) + shakeOffset;

                if (elapsed > 1.2f)
                {
                    float tFade = Mathf.Clamp01((elapsed - 1.2f) / (durasiBangkrut - 1.2f));
                    if (canvasGroupFloating != null)
                    {
                        canvasGroupFloating.alpha = Mathf.Lerp(1f, 0f, tFade);
                    }
                }
            }

            if (teksUang != null)
            {
                float tSaldo = Mathf.Clamp01(elapsed / 1.0f);
                float smoothSaldo = Mathf.SmoothStep(0f, 1f, tSaldo);
                int saldoDisplay = (int)Mathf.Lerp(saldoAwal, 0, smoothSaldo);
                teksUang.text = saldoDisplay.ToString("N0", cultureIndo);
                teksUang.color = warnaMerahBangkrut;
            }

            yield return null;
        }

        if (rectBoxPorto != null) rectBoxPorto.anchoredPosition = posisiAwalBoxPorto;
        if (teksFloatingMinus != null) teksFloatingMinus.gameObject.SetActive(false);

        if (teksUang != null)
        {
            teksUang.text = "0";
            teksUang.color = warnaMerahBangkrut;
        }

        coroutineAnimasi = null;
    }

    void UpdateLayar()
    {
        if (teksUang != null)
        {
            teksUang.text = saldo.ToString("N0", cultureIndo);
            if (isBangkrut || saldo <= 0)
            {
                teksUang.color = warnaMerahBangkrut;
                if (teksLabelTabungan != null)
                {
                    teksLabelTabungan.text = "Tabungan (LUDES) :";
                    teksLabelTabungan.color = warnaMerahBangkrut;
                }
            }
            else
            {
                teksUang.color = warnaAsliTeksUang;
                if (teksLabelTabungan != null)
                {
                    teksLabelTabungan.text = "Tabungan :";
                    teksLabelTabungan.color = warnaAsliTeksUang;
                }
            }
        }
    }
}
