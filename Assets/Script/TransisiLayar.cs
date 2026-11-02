using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yarn.Unity;

public class TransisiLayar : MonoBehaviour
{
    [Header("Pengaturan Transisi")]
    public Image layarHitam;
    public float durasiFade = 2f;

    private Coroutine coroutineFade;

    void Awake()
    {
        if (layarHitam != null)
        {
            // Saat game baru mulai, layar otomatis ditutup hitam pekat di Awake
            // agar tidak berbenturan dengan perintah <<fade_in>> di Frame 0.
            layarHitam.raycastTarget = false;
            layarHitam.color = new Color(0, 0, 0, 1f);
            layarHitam.gameObject.SetActive(true);
        }
    }

    // Perintah untuk membuat layar terang (dari hitam ke transparan)
    [YarnCommand("fade_in")]
    public void FadeIn()
    {
        if (layarHitam == null) return;
        if (coroutineFade != null) StopCoroutine(coroutineFade);
        layarHitam.gameObject.SetActive(true);
        coroutineFade = StartCoroutine(ProsesFade(1f, 0f));
    }

    // Perintah untuk membuat layar gelap (dari transparan ke hitam pekat)
    [YarnCommand("fade_out")]
    public void FadeOut()
    {
        if (layarHitam == null) return;
        if (coroutineFade != null) StopCoroutine(coroutineFade);
        layarHitam.gameObject.SetActive(true);
        coroutineFade = StartCoroutine(ProsesFade(0f, 1f));
    }

    // Mesin utama untuk memudarkan warna
    IEnumerator ProsesFade(float alphaAwal, float alphaTujuan)
    {
        float waktu = 0;

        while (waktu < durasiFade)
        {
            waktu += Time.deltaTime;
            float alpha = Mathf.Lerp(alphaAwal, alphaTujuan, waktu / durasiFade);
            layarHitam.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Pastikan nilai akhir tepat
        layarHitam.color = new Color(0, 0, 0, alphaTujuan);

        // Jika layar sudah sepenuhnya terang (transparan), matikan objeknya agar bisa diklik
        if (alphaTujuan == 0f)
        {
            layarHitam.gameObject.SetActive(false);
        }
    }
}