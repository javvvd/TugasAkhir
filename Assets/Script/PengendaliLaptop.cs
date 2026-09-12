using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class PengendaliLaptop : MonoBehaviour
{
    [Header("Pengaturan Layar Laptop")]
    [Tooltip("Sudut rotasi saat laptop DITUTUP. Sesuaikan dengan sumbu putar model 3D-mu.")]
    public float sudutTutupX = 90f; // Bisa dicoba 90f, -90f, 100f, dsb.
    public float kecepatan = 3f;

    private Quaternion rotasiBuka; // Menyimpan posisi saat layar TERBUKA (kondisi awal)

    void Start()
    {
        // Karena awal game laptop sudah terbuka, kita simpan posisi ini sebagai "rotasiBuka"
        rotasiBuka = transform.localRotation;
    }

    [YarnCommand("buka_laptop")]
    public void BukaLaptop()
    {
        StartCoroutine(ProsesBuka());
    }

    [YarnCommand("tutup_laptop")]
    public void TutupLaptop()
    {
        StartCoroutine(ProsesTutup());
    }

    IEnumerator ProsesTutup()
    {
        Quaternion rotasiAwal = transform.localRotation;
        // Target saat menutup adalah posisi awal (buka) ditambah sudutTutupX
        Quaternion rotasiTarget = rotasiBuka * Quaternion.Euler(sudutTutupX, 0, 0);

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.localRotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null; 
        }
        transform.localRotation = rotasiTarget; 
    }

    IEnumerator ProsesBuka()
    {
        Quaternion rotasiAwal = transform.localRotation;
        // Target saat membuka adalah kembali ke posisi rotasiBuka (0)
        Quaternion rotasiTarget = rotasiBuka;

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            transform.localRotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null; 
        }
        transform.localRotation = rotasiTarget; 
    }
}