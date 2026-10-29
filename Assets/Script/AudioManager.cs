using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Komponen AudioSource")]
    public AudioSource sfxSource;
    public AudioSource loopSource;
    public AudioSource typingSource;
    public AudioSource bgmSource;

    [Header("Background Music (BGM)")]
    public AudioClip clipBgmSantai;
    [Range(0f, 1f)]
    public float volumeBgm = 0.25f;

    [Header("Daftar Klip Suara (SFX)")]
    public AudioClip clipNotifChat;
    public AudioClip clipDeringTelepon;
    public AudioClip clipKetukPintu;
    public AudioClip clipTutupPintu;
    public AudioClip clipKetikKeyboard;
    public AudioClip clipOrderSukses;
    public AudioClip clipKlikTelepon;
    public AudioClip clipDramatisARB;
    public AudioClip clipMinum;

    private Coroutine coroutineBgm;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Buat AudioSource otomatis jika belum dipasang di Inspector
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.playOnAwake = false;
            loopSource.loop = true;
        }

        if (typingSource == null)
        {
            typingSource = gameObject.AddComponent<AudioSource>();
            typingSource.playOnAwake = false;
            typingSource.loop = true;
        }

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
        }
    }

    [YarnCommand("putar_bgm")]
    public void PutarBGM(string namaBgm = "santai")
    {
        AudioClip target = null;
        string key = namaBgm != null ? namaBgm.ToLower().Trim() : "";

        switch (key)
        {
            case "santai":
            case "calm":
            case "kamar":
            case "lofi":
            default:
                target = clipBgmSantai;
                break;
        }

        if (target != null)
        {
            if (coroutineBgm != null) StopCoroutine(coroutineBgm);
            coroutineBgm = StartCoroutine(ProsesFadeInBGM(target, 1.5f));
        }
        else
        {
            Debug.LogWarning($"[AudioManager] BGM '{namaBgm}' belum diatur klip audionya!");
        }
    }

    [YarnCommand("stop_bgm")]
    public void StopBGM()
    {
        if (coroutineBgm != null) StopCoroutine(coroutineBgm);
        coroutineBgm = StartCoroutine(ProsesFadeOutBGM(1.0f));
    }

    [YarnCommand("set_volume_bgm")]
    public void SetVolumeBGM(float vol)
    {
        volumeBgm = Mathf.Clamp01(vol);
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.volume = volumeBgm;
        }
    }

    private IEnumerator ProsesFadeInBGM(AudioClip clip, float durasiFade)
    {
        if (bgmSource == null) yield break;

        if (bgmSource.isPlaying && bgmSource.clip == clip)
        {
            yield break;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float t = 0f;
        while (t < durasiFade)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, volumeBgm, t / durasiFade);
            yield return null;
        }
        bgmSource.volume = volumeBgm;
        coroutineBgm = null;
    }

    private IEnumerator ProsesFadeOutBGM(float durasiFade)
    {
        if (bgmSource == null || !bgmSource.isPlaying) yield break;

        float startVol = bgmSource.volume;
        float t = 0f;
        while (t < durasiFade)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / durasiFade);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = volumeBgm;
        coroutineBgm = null;
    }

    [YarnCommand("putar_sfx")]
    public void PutarSFX(string namaSFX)
    {
        AudioClip targetClip = null;
        string key = namaSFX.ToLower().Trim();

        switch (key)
        {
            case "notif":
            case "chat":
            case "whatsapp":
                targetClip = clipNotifChat;
                break;

            case "ketuk_pintu":
            case "pintu":
            case "knock":
            case "ketuk":
                targetClip = clipKetukPintu;
                break;

            case "tutup_pintu":
            case "pintu_tutup":
            case "tutuppintu":
            case "close_door":
            case "door_close":
                targetClip = clipTutupPintu;
                break;

            case "keyboard":
            case "ketik":
            case "ngetik":
            case "typing":
            case "tugas":
                targetClip = clipKetikKeyboard;
                break;

            case "order":
            case "beli":
            case "koin":
            case "sukses":
                targetClip = clipOrderSukses;
                break;

            case "klik":
            case "klik_telepon":
                targetClip = clipKlikTelepon;
                break;

            case "dramatis":
            case "arb":
            case "panik":
                targetClip = clipDramatisARB;
                PengendaliKamera.Instance?.TriggerShakeKamera(0.8f, 0.08f);
                break;

            case "dering":
            case "telepon":
                targetClip = clipDeringTelepon;
                break;

            case "minum":
            case "air":
            case "gelas":
            case "drink":
                targetClip = clipMinum;
                break;
        }

        if (targetClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(targetClip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX dengan nama '{namaSFX}' tidak ditemukan.");
        }
    }

    [YarnCommand("mulai_dering")]
    public void MulaiDering()
    {
        if (loopSource != null && clipDeringTelepon != null)
        {
            loopSource.clip = clipDeringTelepon;
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    [YarnCommand("stop_dering")]
    public void StopDering()
    {
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.Stop();
        }
    }

    [YarnCommand("mulai_ngetik")]
    public void MulaiNgetik()
    {
        if (typingSource != null && clipKetikKeyboard != null)
        {
            typingSource.clip = clipKetikKeyboard;
            typingSource.loop = true;
            typingSource.Play();
        }
    }

    [YarnCommand("stop_ngetik")]
    public void StopNgetik()
    {
        if (typingSource != null && typingSource.isPlaying)
        {
            typingSource.Stop();
        }
    }
}
