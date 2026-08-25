using UnityEngine;

public class KameraMobile : MonoBehaviour
{
    [Header("Pengaturan Sensitivitas")]
    public float sensitivitasLayarHP = 0.2f;
    public float sensitivitasMouseEditor = 2f;

    private float rotasiX = 0f;
    private float rotasiY = 0f;

    void Update()
    {
        float geserX = 0f;
        float geserY = 0f;

        // 1. Deteksi usapan jari untuk perangkat Mobile (HP)
        if (Input.touchCount > 0)
        {
            Touch sentuhan = Input.GetTouch(0); // Mendeteksi jari pertama yang menyentuh layar

            if (sentuhan.phase == TouchPhase.Moved)
            {
                geserX = sentuhan.deltaPosition.x * sensitivitasLayarHP;
                geserY = sentuhan.deltaPosition.y * sensitivitasLayarHP;
            }
        }
        // 2. Deteksi klik-kiri & tahan (Drag) untuk uji coba di PC/Unity Editor
        else if (Input.GetMouseButton(0)) 
        {
            geserX = Input.GetAxis("Mouse X") * sensitivitasMouseEditor;
            geserY = Input.GetAxis("Mouse Y") * sensitivitasMouseEditor;
        }

        // Terapkan kalkulasi rotasi jika ada pergeseran
        if (geserX != 0 || geserY != 0)
        {
            // Menghitung rotasi atas-bawah (Sumbu Y dibalik agar arah usapan terasa natural)
            rotasiX -= geserY;
            rotasiX = Mathf.Clamp(rotasiX, -90f, 90f); // Mengunci leher agar tidak berputar ke belakang

            // Menghitung rotasi kiri-kanan
            rotasiY += geserX;

            // Menggerakkan kamera
            transform.localRotation = Quaternion.Euler(rotasiX, rotasiY, 0f);
        }
    }
}