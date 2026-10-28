using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class PengendaliLaptop : MonoBehaviour
{
    [Header("Pengaturan Layar Laptop")]
    [Tooltip("Sudut rotasi lokal saat laptop TERBUKA")]
    public Vector3 eulerBuka = Vector3.zero;

    [Tooltip("Perubahan sudut sumbu X saat laptop DITUTUP")]
    public float sudutTutupX = -89f;

    [Tooltip("Kecepatan animasi membuka / menutup")]
    public float kecepatan = 1.2f;

    [Tooltip("Jika true, saat game dimulai laptop langsung dalam posisi tertutup")]
    public bool mulaiTertutup = true;

    private Quaternion rotasiBuka;
    private Quaternion rotasiTutup;
    private Coroutine coroutineAnimasi;

    void Awake()
    {
        rotasiBuka = Quaternion.Euler(eulerBuka);
        rotasiTutup = rotasiBuka * Quaternion.Euler(sudutTutupX, 0, 0);

        if (mulaiTertutup)
        {
            transform.localRotation = rotasiTutup;
        }
        else
        {
            transform.localRotation = rotasiBuka;
        }
    }

    [YarnCommand("buka_laptop")]
    public void BukaLaptop()
    {
        if (coroutineAnimasi != null) StopCoroutine(coroutineAnimasi);
        coroutineAnimasi = StartCoroutine(ProsesBuka());
    }

    [YarnCommand("tutup_laptop")]
    public void TutupLaptop()
    {
        if (coroutineAnimasi != null) StopCoroutine(coroutineAnimasi);
        coroutineAnimasi = StartCoroutine(ProsesTutup());
    }

    IEnumerator ProsesTutup()
    {
        Quaternion rotasiAwal = transform.localRotation;
        Quaternion rotasiTarget = rotasiTutup;

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.localRotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null; 
        }
        transform.localRotation = rotasiTarget;
        coroutineAnimasi = null;
    }

    IEnumerator ProsesBuka()
    {
        Quaternion rotasiAwal = transform.localRotation;
        Quaternion rotasiTarget = rotasiBuka;

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.localRotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null; 
        }
        transform.localRotation = rotasiTarget;
        coroutineAnimasi = null;
    }
}
