using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class PengendaliPintu : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    public float sudutBuka = 80f;
    public float kecepatan = 2f;

    private Quaternion rotasiAwalPintu;
    private Coroutine coroutinePintu;

    void Awake()
    {
        rotasiAwalPintu = transform.rotation;
    }

    [YarnCommand("buka_pintu")]
    public void BukaPintu()
    {
        if (coroutinePintu != null) StopCoroutine(coroutinePintu);
        coroutinePintu = StartCoroutine(ProsesBuka());
    }

    [YarnCommand("tutup_pintu")]
    public void TutupPintu()
    {
        if (coroutinePintu != null) StopCoroutine(coroutinePintu);
        coroutinePintu = StartCoroutine(ProsesTutup());
    }

    IEnumerator ProsesBuka()
    {
        Quaternion rotasiAwal = transform.rotation;
        Quaternion rotasiTarget = rotasiAwalPintu * Quaternion.Euler(0f, sudutBuka, 0f);

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.rotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null;
        }

        transform.rotation = rotasiTarget;
        coroutinePintu = null;
    }

    IEnumerator ProsesTutup()
    {
        Quaternion rotasiSaatIni = transform.rotation;
        Quaternion rotasiTarget = rotasiAwalPintu;

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.rotation = Quaternion.Slerp(rotasiSaatIni, rotasiTarget, waktu);
            yield return null;
        }

        transform.rotation = rotasiTarget;
        coroutinePintu = null;

        AudioManager.Instance?.PutarSFX("tutup_pintu");
    }
}
