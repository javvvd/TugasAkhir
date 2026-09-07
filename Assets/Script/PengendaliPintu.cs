using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class PengendaliPintu : MonoBehaviour
{
    // Perintah ini bisa dipanggil langsung dari naskah Yarn
    [YarnCommand("buka_pintu")]
    public void BukaPintu()
    {
        StartCoroutine(ProsesBuka());
    }

    IEnumerator ProsesBuka()
    {
        float targetSudut = 90f;  // Ganti -90f jika pintunya terbuka ke arah luar/terbalik
        float kecepatan = 150f;   // Semakin besar, semakin cepat pintu terbuka
        float sudutSekarang = 0f;

        while (sudutSekarang < Mathf.Abs(targetSudut))
        {
            float putaran = kecepatan * Time.deltaTime;
            
            // Memutar pada sumbu Y
            transform.Rotate(0, targetSudut > 0 ? putaran : -putaran, 0); 
            
            sudutSekarang += putaran;
            yield return null; // Tunggu ke frame berikutnya biar mulus
        }
    }
}