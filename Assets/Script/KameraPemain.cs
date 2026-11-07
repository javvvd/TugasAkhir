using UnityEngine;
using UnityEngine.EventSystems;

public class KameraMobile : MonoBehaviour
{
    [Header("Pengaturan Sensitivitas")]
    public float sensitivitasLayarHP = 0.2f;
    public float sensitivitasMouseEditor = 2f;

    private float rotasiX = 0f;
    private float rotasiY = 0f;

    void OnEnable()
    {
        // 1. Ambil rotasi awal kamera dari Unity Editor saat game dimulai / kamera aktif
        Vector3 rotasiAwal = transform.localEulerAngles;
        rotasiX = rotasiAwal.x;
        rotasiY = rotasiAwal.y;

        // 2. Koreksi sumbu X (Atas-Bawah) agar fungsi Clamp (-90 sampai 90) akurat
        if (rotasiX > 180f)
        {
            rotasiX -= 360f;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        // Cegah perputaran kamera jika HP sedang dibuka
        if (HPManager.Instance != null && HPManager.Instance.IsHpActive) return;

        float geserX = 0f;
        float geserY = 0f;

        // Deteksi usapan jari untuk perangkat Mobile (HP)
        if (Input.touchCount > 0)
        {
            Touch sentuhan = Input.GetTouch(0);

            // Jangan putar kamera jika sedang menyentuh tombol UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(sentuhan.fingerId))
            {
                return;
            }

            if (sentuhan.phase == TouchPhase.Moved)
            {
                geserX = sentuhan.deltaPosition.x * sensitivitasLayarHP;
                geserY = sentuhan.deltaPosition.y * sensitivitasLayarHP;
            }
        }
#if UNITY_EDITOR
        // Deteksi klik-kiri & tahan (Drag) khusus untuk uji coba di Unity Editor
        else if (Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            geserX = Input.GetAxis("Mouse X") * sensitivitasMouseEditor;
            geserY = Input.GetAxis("Mouse Y") * sensitivitasMouseEditor;
        }
#endif

        // Terapkan kalkulasi rotasi jika ada pergeseran
        if (geserX != 0f || geserY != 0f)
        {
            rotasiX = Mathf.Clamp(rotasiX - geserY, -90f, 90f);
            rotasiY += geserX;

            transform.localRotation = Quaternion.Euler(rotasiX, rotasiY, 0f);
        }
    }
}
