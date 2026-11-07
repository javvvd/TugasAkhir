using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    [Header("Komponen Audio")]
    public AudioSource sfxSource;
    public AudioClip clipClickUI;
    public AudioSource bgmSource;
    public AudioClip clipBGM;
    [Range(0f, 1f)]
    public float volumeBgm = 0.25f;

    [Header("Panel Modal")]
    public GameObject panelSetting;
    public GameObject panelAbout;
    public GameObject panelDisclaimer;
    public Slider sliderVolume;
    public TextMeshProUGUI textVolumeValue;

    [Header("Tombol Utama")]
    public Button btnPlay;
    public Button btnSetting;
    public Button btnCredit;
    public Button btnAbout;
    public Button btnSayaMengerti;

    [Header("Transisi Layar")]
    public Image fadeOverlay;
    public float durasiFade = 1.0f;
    private bool sedangTransisi = false;

    void Awake()
    {
        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
        }

        // Terapkan master volume dari PlayerPrefs
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVol;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        InisialisasiKomponen();
        PutarBGM();
    }

    public void PutarBGM()
    {
        if (clipBGM != null && bgmSource != null)
        {
            if (bgmSource.isPlaying && bgmSource.clip == clipBGM) return;
            bgmSource.clip = clipBGM;
            bgmSource.loop = true;
            bgmSource.volume = volumeBgm;
            bgmSource.Play();
        }
    }

    public void InisialisasiKomponen()
    {
        // 1. Setup Slider Volume
        if (sliderVolume != null)
        {
            float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            sliderVolume.value = savedVol;
            UpdateTextVolume(savedVol);

            sliderVolume.onValueChanged.RemoveAllListeners();
            sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
        }

        // 2. Hubungkan Tombol Menu Utama
        if (btnPlay != null)
        {
            btnPlay.onClick.RemoveAllListeners();
            btnPlay.onClick.AddListener(() =>
            {
                PlaySFXClick();
                BukaDisclaimer();
            });
        }

        if (btnSetting != null)
        {
            btnSetting.onClick.RemoveAllListeners();
            btnSetting.onClick.AddListener(() =>
            {
                PlaySFXClick();
                BukaSetting();
            });
        }

        if (btnCredit != null)
        {
            btnCredit.onClick.RemoveAllListeners();
            btnCredit.onClick.AddListener(() =>
            {
                PlaySFXClick();
                BukaAbout();
            });
        }

        if (btnAbout != null)
        {
            btnAbout.onClick.RemoveAllListeners();
            btnAbout.onClick.AddListener(() =>
            {
                PlaySFXClick();
                KeluarGame();
            });
        }

        if (btnSayaMengerti != null)
        {
            btnSayaMengerti.onClick.RemoveAllListeners();
            btnSayaMengerti.onClick.AddListener(() =>
            {
                PlaySFXClick();
                MulaiGameplay();
            });
        }

        SetupFadeOverlay();

        // 3. Hubungkan Tombol di Dalam Panel Modal (Setting, About, Disclaimer)
        HubungkanTombolModal();
    }

    public void HubungkanTombolModal()
    {
        // Panel Setting
        if (panelSetting != null)
        {
            Button[] settingButtons = panelSetting.GetComponentsInChildren<Button>(true);
            foreach (var b in settingButtons)
            {
                string n = b.name.ToLower();
                if (n.Contains("close") || n.Contains("tutup") || n.Contains("save") || n.Contains("simpan") || n.Contains("btn"))
                {
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() =>
                    {
                        PlaySFXClick();
                        TutupSetting();
                    });
                }
            }
        }

        // Panel About
        if (panelAbout != null)
        {
            Button[] aboutButtons = panelAbout.GetComponentsInChildren<Button>(true);
            foreach (var b in aboutButtons)
            {
                string n = b.name.ToLower();
                if (n.Contains("close") || n.Contains("tutup") || n.Contains("kembali") || n.Contains("btn"))
                {
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() =>
                    {
                        PlaySFXClick();
                        TutupAbout();
                    });
                }
            }
        }

        // Panel Disclaimer
        if (panelDisclaimer != null)
        {
            Button[] disclaimerButtons = panelDisclaimer.GetComponentsInChildren<Button>(true);
            foreach (var b in disclaimerButtons)
            {
                string n = b.name.ToLower();
                if (n.Contains("close") || n.Contains("tutup") || n.Contains("btnclose") || n.Contains("backdrop"))
                {
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() =>
                    {
                        PlaySFXClick();
                        TutupDisclaimer();
                    });
                }
                else if (n.Contains("mengerti") || n.Contains("lanjut") || n.Contains("setuju") || n.Contains("start"))
                {
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() =>
                    {
                        PlaySFXClick();
                        MulaiGameplay();
                    });
                }
            }
        }
    }

    public void PlaySFXClick()
    {
        if (sfxSource != null && clipClickUI != null)
        {
            sfxSource.PlayOneShot(clipClickUI);
        }
    }

    public void OnVolumeChanged(float val)
    {
        AudioListener.volume = val;
        PlayerPrefs.SetFloat("MasterVolume", val);
        PlayerPrefs.Save();
        UpdateTextVolume(val);
    }

    private void UpdateTextVolume(float val)
    {
        if (textVolumeValue != null)
        {
            textVolumeValue.text = Mathf.RoundToInt(val * 100f) + "%";
        }
    }

    public void BukaSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(true);
        if (panelAbout != null) panelAbout.SetActive(false);
        if (panelDisclaimer != null) panelDisclaimer.SetActive(false);
    }

    public void TutupSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    public void BukaAbout()
    {
        if (panelAbout != null) panelAbout.SetActive(true);
        if (panelSetting != null) panelSetting.SetActive(false);
        if (panelDisclaimer != null) panelDisclaimer.SetActive(false);
    }

    public void TutupAbout()
    {
        if (panelAbout != null) panelAbout.SetActive(false);
    }

    public void BukaDisclaimer()
    {
        // Pastikan ketika klik Play biasa, status lompat babak direset bersih
        KamusManager.TargetNodeLompatan = "";

        if (panelDisclaimer != null) panelDisclaimer.SetActive(true);
        if (panelSetting != null) panelSetting.SetActive(false);
        if (panelAbout != null) panelAbout.SetActive(false);
    }

    public void TutupDisclaimer()
    {
        if (panelDisclaimer != null) panelDisclaimer.SetActive(false);
    }

    public void MulaiGameplay()
    {
        if (sedangTransisi) return;
        sedangTransisi = true;

        StartCoroutine(ProsesFadeDanPindahScene("Gameplay"));
    }

    private System.Collections.IEnumerator ProsesFadeDanPindahScene(string sceneName)
    {
        if (fadeOverlay == null)
        {
            SetupFadeOverlay();
        }

        float startBgmVol = (bgmSource != null && bgmSource.isPlaying) ? bgmSource.volume : 0f;

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.transform.SetAsLastSibling();
            fadeOverlay.raycastTarget = true;

            float waktu = 0f;
            while (waktu < durasiFade)
            {
                waktu += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(waktu / durasiFade);
                float alpha = Mathf.SmoothStep(0f, 1f, t);
                fadeOverlay.color = new Color(0f, 0f, 0f, alpha);
                if (bgmSource != null && startBgmVol > 0f)
                {
                    bgmSource.volume = Mathf.Lerp(startBgmVol, 0f, t);
                }
                yield return null;
            }
            fadeOverlay.color = new Color(0f, 0f, 0f, 1f);
        }

        SceneManager.LoadScene(sceneName);
    }

    public void SetupFadeOverlay()
    {
        if (fadeOverlay != null) return;

        // Cari objek FadeOverlay yang mungkin sudah ada di scene
        GameObject existingObj = GameObject.Find("FadeOverlay");
        if (existingObj != null)
        {
            fadeOverlay = existingObj.GetComponent<Image>();
            return;
        }

        // Buat otomatis di bawah Canvas
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            GameObject overlayObj = new GameObject("FadeOverlay");
            overlayObj.transform.SetParent(canvas.transform, false);

            fadeOverlay = overlayObj.AddComponent<Image>();
            fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
            fadeOverlay.raycastTarget = false;

            RectTransform rt = overlayObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            overlayObj.transform.SetAsLastSibling();
            overlayObj.SetActive(false);
        }
    }

    public void KeluarGame()
    {
        Debug.Log("[MainMenu] Keluar dari permainan.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
