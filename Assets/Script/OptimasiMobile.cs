using UnityEngine;

/// <summary>
/// Mengoptimalkan performa game di perangkat mobile (Android/iOS)
/// secara otomatis sebelum scene pertama dimuat.
/// Menurunkan resolusi render ke 720p (HD+) untuk menghemat beban GPU hingga >50%
/// dan mengunci frame rate di 60 FPS agar mulus dan HP tidak cepat panas.
/// </summary>
public static class OptimasiMobile
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InisialisasiOptimasi()
    {
        // 1. Kunci frame rate ke 60 FPS agar gameplay stabil dan tidak patah-patah
        Application.targetFrameRate = 60;

        // 2. Matikan VSync agar batas 60 FPS dapat diterapkan dengan presisi
        QualitySettings.vSyncCount = 0;

        // 3. Mencegah layar HP mati/redup otomatis saat pemain sedang membaca dialog
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 4. Optimasi resolusi render khusus saat dijalankan di perangkat mobile asli
        #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        TerapkanResolusiOptimal();
        #endif

        Debug.Log("[OptimasiMobile] Optimasi aktif: Target 60 FPS, VSync Off, Resolusi hemat GPU.");
    }

    /// <summary>
    /// Menyesuaikan resolusi render ke 720p (HD+) dengan tetap menjaga rasio aspek asli layar HP.
    /// </summary>
    public static void TerapkanResolusiOptimal()
    {
        int lebarAsli = Screen.currentResolution.width;
        int tinggiAsli = Screen.currentResolution.height;

        if (lebarAsli <= 0 || tinggiAsli <= 0)
        {
            lebarAsli = Screen.width;
            tinggiAsli = Screen.height;
        }

        // Tentukan apakah layar sedang Landscape atau Portrait
        bool isLandscape = lebarAsli >= tinggiAsli;

        int targetLebar;
        int targetTinggi;

        if (isLandscape)
        {
            // Untuk orientasi Landscape: patokan tinggi adalah 720p
            targetTinggi = 720;
            float rasio = (float)lebarAsli / tinggiAsli;
            targetLebar = Mathf.RoundToInt(targetTinggi * rasio);
        }
        else
        {
            // Untuk orientasi Portrait: patokan lebar adalah 720p
            targetLebar = 720;
            float rasio = (float)tinggiAsli / lebarAsli;
            targetTinggi = Mathf.RoundToInt(targetLebar * rasio);
        }

        // Hanya terapkan jika resolusi asli lebih besar dari target (mencegah upscaling jika HP sudah 720p)
        if (lebarAsli > targetLebar || tinggiAsli > targetTinggi)
        {
            Screen.SetResolution(targetLebar, targetTinggi, true);
            Debug.Log($"[OptimasiMobile] Resolusi dioptimasi dari {lebarAsli}x{tinggiAsli} menjadi {targetLebar}x{targetTinggi}.");
        }
    }
}
