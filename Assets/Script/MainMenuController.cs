using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    [Header("Komponen Audio SFX UI")]
    public AudioSource sfxSource;
    public AudioClip clipClickUI;

    [Header("Panel Modal")]
    public GameObject panelSetting;
    public GameObject panelAbout;
    public Slider sliderVolume;
    public TextMeshProUGUI textVolumeValue;

    [Header("Tombol Utama")]
    public Button btnPlay;
    public Button btnSetting;
    public Button btnCredit;
    public Button btnAbout;

    void Awake()
    {
        if (Instance == null) Instance = this;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        // Terapkan master volume dari PlayerPrefs
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVol;
    }

    void Start()
    {
        InisialisasiKomponen();
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
                SceneManager.LoadScene("Gameplay");
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

        // 3. Hubungkan Tombol di Dalam Panel Modal (Setting & About)
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
        if (panelSetting != null)
        {
            panelSetting.SetActive(true);
            HubungkanTombolModal();
        }
        if (panelAbout != null) panelAbout.SetActive(false);
    }

    public void TutupSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    public void BukaAbout()
    {
        if (panelAbout != null)
        {
            panelAbout.SetActive(true);
            HubungkanTombolModal();
        }
        if (panelSetting != null) panelSetting.SetActive(false);
    }

    public void TutupAbout()
    {
        if (panelAbout != null) panelAbout.SetActive(false);
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
