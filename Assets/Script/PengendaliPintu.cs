using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class PengendaliPintu : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    public float sudutBuka = 80f; // Ubah ke -90f jika pintu terbuka ke arah yang salah
    public float kecepatan = 2f;  // Semakin besar, semakin cepat terbuka

    private Quaternion rotasiAwalPintu; // Menyimpan posisi pintu saat tertutup rapat

    void Start()
    {
        // Menyimpan posisi 0 (tertutup) saat game baru mulai
        rotasiAwalPintu = transform.rotation;
    }

    // Perintah untuk buka pintu
    [YarnCommand("buka_pintu")]
    public void BukaPintu()
    {
        StartCoroutine(ProsesBuka());
    }

    // Perintah untuk tutup pintu
    [YarnCommand("tutup_pintu")]
    public void TutupPintu()
    {
        StartCoroutine(ProsesTutup());
    }

    IEnumerator ProsesBuka()
    {
        Quaternion rotasiAwal = transform.rotation;
        // Targetkan sudutBuka (misal: 80 derajat)
        Quaternion rotasiTarget = rotasiAwalPintu * Quaternion.Euler(0, sudutBuka, 0);

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.rotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null;
        }

        transform.rotation = rotasiTarget;
    }

    IEnumerator ProsesTutup()
    {
        Quaternion rotasiSaatIni = transform.rotation;
        // Target kembali ke posisi awal (0 derajat)
        Quaternion rotasiTarget = rotasiAwalPintu;

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.rotation = Quaternion.Slerp(rotasiSaatIni, rotasiTarget, waktu);
            yield return null;
        }

        transform.rotation = rotasiTarget;
    }
}