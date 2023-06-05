using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    private bool isPaused = false;
    private int input = 0;

    [Header("Menu Images")]
    public Image resume;
    public Image sfxVolume;
    public Image musicVolume;
    public Image exit;

    public Material selected;
    public Material unSelected;

    [Header("Audio Sliders")]
    public Slider sfxSlider;
    public Slider musicSlider;
    private static float sfxTempVolume = 0.5f;
    private static float musicTempVolume = 0.5f;

    private EventSystem eventSystem;
    private MusicManager musicManager;
    private SFXManager sfxManager;

    private bool isSliderSelected = false;
    private Slider selectedSlider;

    public static Menu singleton { get; private set; }

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(GameObject.Find("UserInterface"));
            singleton = this;
        }
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
        musicManager = FindObjectOfType<MusicManager>();
        sfxManager = FindObjectOfType<SFXManager>();

        // Load saved volume settings
        LoadVolumeSettings();
        TurnMenuOff();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && !isPaused)
        {
            TurnMenuOn();
        }
        else if (Input.GetButtonDown("Cancel") && isPaused)
        {
            TurnMenuOff();
        }

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                input--;
                if (input < 0)
                    input = 3;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                input++;
                if (input > 3)
                    input = 0;
            }
        }

        if (isPaused)
        {
            switch (input)
            {
                case 0:
                    UpdateSelection(resume);
                    break;

                case 1:
                    UpdateSelection(sfxVolume);
                    break;

                case 2:
                    UpdateSelection(musicVolume);
                    break;

                case 3:
                    UpdateSelection(exit);
                    break;
            }
        }

        if (isPaused && Time.timeScale == 1)
        {
            // Pause music and SFX gradually
            MusicManager.singleton.FadeOut();
            SFXManager.singleton.FadeOut(0.5f);
        }
        else if (!isPaused && Time.timeScale == 0)
        {
            // Resume music and SFX gradually
            MusicManager.singleton.FadeIn();
            SFXManager.singleton.FadeIn(0.5f);
        }

        if (isPaused && Input.GetButtonDown("Submit"))
        {
            switch (input)
            {
                case 0:
                    TurnMenuOff();
                    break;

                case 1:
                    SaveVolumeSettings();
                    break;

                case 2:
                    ExitGame();
                    break;

                case 3:
                    // Handle exit confirmation if needed
                    break;
            }
        }

        if (isSliderSelected && isPaused)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                selectedSlider.value -= 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                selectedSlider.value += 0.1f;
            }

            sfxTempVolume = sfxSlider.value;
            musicTempVolume = musicSlider.value;
            SFXManager.masterVolume = sfxSlider.value;
            MusicManager.singleton.masterVolume = musicSlider.value;
        }
    }

    private void UpdateSelection(Image selectedImage)
    {
        eventSystem.SetSelectedGameObject(selectedImage.gameObject);

        // Reset all button and slider materials to unselected state
        resume.material = unSelected;
        resume.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        sfxVolume.material = unSelected;
        musicVolume.material = unSelected;
        exit.material = unSelected;
        exit.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        // Check if the selected image is a slider
        if (selectedImage == sfxVolume)
        {
            isSliderSelected = true;
            selectedSlider = sfxSlider;
            sfxVolume.material = selected;
        }
        else if (selectedImage == musicVolume)
        {
            isSliderSelected = true;
            selectedSlider = musicSlider;
            musicVolume.material = selected;
        }
        else
        {
            isSliderSelected = false;
            selectedSlider = null;
            selectedImage.material = selected;
            selectedImage.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        }
    }

    public void ExitGame()
    {
        SaveVolumeSettings(); // Save volume settings before quitting
        Application.Quit();
    }

    private void OnDisable()
    {
        SaveVolumeSettings(); // Save volume settings before quitting
    }

    private void TurnMenuOn()
    {
        menu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;

        UpdateSelection(resume);
        LoadVolumeSettings();
    }

    private void TurnMenuOff()
    {
        menu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;

        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        float sfxVolume = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : sfxTempVolume;
        float musicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : musicTempVolume;

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        // Apply volume settings to the MusicManager and SFXManager
        MusicManager.singleton.masterVolume = musicVolume;
        SFXManager.masterVolume = sfxVolume;
    }
}
