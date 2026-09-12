using UnityEngine;

public class KameraMobile : MonoBehaviour
{
    [Header("Pengaturan Sensitivitas")]
    public float sensitivitasLayarHP = 0.2f;
    public float sensitivitasMouseEditor = 2f;

    private float rotasiX = 0f;
    private float rotasiY = 0f;

    void Start()
    {
        // 1. Ambil rotasi awal kamera dari Unity Editor saat game dimulai
        Vector3 rotasiAwal = transform.localEulerAngles;
        rotasiX = rotasiAwal.x;
        rotasiY = rotasiAwal.y;

        // 2. Koreksi sumbu X (Atas-Bawah)
        // Unity menyimpan sudut negatif (misal menunduk -10 derajat) sebagai 350 derajat.
        // Kita harus mengembalikannya ke format minus agar fungsi Clamp (-90 sampai 90) tidak rusak.
        if (rotasiX > 180f)
        {
            rotasiX -= 360f;
        }
    }

    void Update()
    {
        float geserX = 0f;
        float geserY = 0f;

        // Deteksi usapan jari untuk perangkat Mobile (HP)
        if (Input.touchCount > 0)
        {
            Touch sentuhan = Input.GetTouch(0); 

            if (sentuhan.phase == TouchPhase.Moved)
            {
                geserX = sentuhan.deltaPosition.x * sensitivitasLayarHP;
                geserY = sentuhan.deltaPosition.y * sensitivitasLayarHP;
            }
        }
        // Deteksi klik-kiri & tahan (Drag) untuk uji coba di PC/Unity Editor
        else if (Input.GetMouseButton(0)) 
        {
            geserX = Input.GetAxis("Mouse X") * sensitivitasMouseEditor;
            geserY = Input.GetAxis("Mouse Y") * sensitivitasMouseEditor;
        }

        // Terapkan kalkulasi rotasi jika ada pergeseran
        if (geserX != 0 || geserY != 0)
        {
            rotasiX -= geserY;
            rotasiX = Mathf.Clamp(rotasiX, -90f, 90f); 

            rotasiY += geserX;

            transform.localRotation = Quaternion.Euler(rotasiX, rotasiY, 0f);
        }
    }
}