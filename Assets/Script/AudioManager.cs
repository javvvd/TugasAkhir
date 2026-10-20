using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Komponen AudioSource")]
    public AudioSource sfxSource;
    public AudioSource loopSource;

    [Header("Daftar Klip Suara (SFX)")]
    public AudioClip clipNotifChat;
    public AudioClip clipDeringTelepon;
    public AudioClip clipKetukPintu;
    public AudioClip clipOrderSukses;
    public AudioClip clipKlikTelepon;
    public AudioClip clipDramatisARB;

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
                targetClip = clipKetukPintu;
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
                break;

            case "dering":
            case "telepon":
                targetClip = clipDeringTelepon;
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
}
