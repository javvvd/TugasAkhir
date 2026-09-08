using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class PengendaliPintu : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    public float sudutBuka = 80f; // Ubah ke -90f jika pintu terbuka ke arah yang salah
    public float kecepatan = 2f;  // Semakin besar, semakin cepat terbuka

    // Perintah ini akan dipanggil oleh Yarn Spinner
    [YarnCommand("buka_pintu")]
    public void BukaPintu()
    {
        StartCoroutine(ProsesBuka());
    }

    IEnumerator ProsesBuka()
    {
        Quaternion rotasiAwal = transform.rotation;
        // Menargetkan rotasi pada sumbu Y (kiri-kanan)
        Quaternion rotasiTarget = transform.rotation * Quaternion.Euler(0, sudutBuka, 0);

        float waktu = 0f;
        while (waktu < 1f)
        {
            waktu += Time.deltaTime * kecepatan;
            // Memutar pintu secara halus dari posisi awal ke target
            transform.rotation = Quaternion.Slerp(rotasiAwal, rotasiTarget, waktu);
            yield return null; 
        }
        
        // Memastikan posisi akhir tepat 100%
        transform.rotation = rotasiTarget; 
    }
}