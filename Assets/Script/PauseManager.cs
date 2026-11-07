using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Referensi UI Pause Gameplay")]
    public Button tombolPause;
    public GameObject panelPauseModal;
    public Slider sliderVolumePause;
    public TextMeshProUGUI textVolumePause;
    public Button tombolLanjutPause;
    public Button tombolMenuPause;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        InisialisasiPauseUI();
    }

    private void InisialisasiPauseUI()
    {
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = savedVol;

        if (panelPauseModal != null)
        {
            panelPauseModal.SetActive(false);
        }

        if (sliderVolumePause != null)
        {
            sliderVolumePause.value = savedVol;
            UpdateLabelVolumePause(savedVol);
            sliderVolumePause.onValueChanged.RemoveAllListeners();
            sliderVolumePause.onValueChanged.AddListener(UbahVolumePause);
        }

        if (tombolPause != null)
        {
            tombolPause.onClick.RemoveAllListeners();
            tombolPause.onClick.AddListener(BukaPause);
        }

        if (tombolLanjutPause != null)
        {
            tombolLanjutPause.onClick.RemoveAllListeners();
            tombolLanjutPause.onClick.AddListener(TutupPause);
        }

        if (tombolMenuPause != null)
        {
            tombolMenuPause.onClick.RemoveAllListeners();
            tombolMenuPause.onClick.AddListener(KembaliKeMenu);
        }
    }

    public void BukaPause()
    {
        AudioManager.Instance?.PutarSFX("klik");
        if (panelPauseModal != null)
        {
            panelPauseModal.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void TutupPause()
    {
        AudioManager.Instance?.PutarSFX("klik");
        if (panelPauseModal != null)
        {
            panelPauseModal.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void UbahVolumePause(float val)
    {
        AudioListener.volume = val;
        PlayerPrefs.SetFloat("MasterVolume", val);
        PlayerPrefs.Save();
        UpdateLabelVolumePause(val);
    }

    private void UpdateLabelVolumePause(float val)
    {
        if (textVolumePause != null)
        {
            textVolumePause.text = $"Volume Suara: {Mathf.RoundToInt(val * 100f)}%";
        }
    }

    public void SembunyikanTombolPause()
    {
        if (panelPauseModal != null) panelPauseModal.SetActive(false);
        if (tombolPause != null) tombolPause.gameObject.SetActive(false);
    }

    public void KembaliKeMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

