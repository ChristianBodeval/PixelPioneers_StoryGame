using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton;

    [Range(0f, 1f)] public float masterVolume = 0.5f;
    [SerializeField] private AudioClip caveAmbience;
    public float fadeDuration = 1f;
    private AudioSource audioSource;
    private AudioClip currentClip;
    private float clipVolume = 1f;
    private float targetVolume;
    private bool isFadingIn;
    private bool isFadingOut;
    private bool inCave = false;

    // 1st
    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            singleton = this;
        }

        audioSource = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
    }

    // 2nd
    private void OnEnable()
    {
        SceneManager.sceneLoaded += GetCaveNameWithRegex;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GetCaveNameWithRegex;
    }

    // Call this method to get the 'Cave' part of the current scene's name using regex
    public void GetCaveNameWithRegex(Scene scene, LoadSceneMode mode)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Define the regex pattern to match the 'Cave' part
        string pattern = @"Cave";

        // Create a regex object with the pattern
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Perform the regex match on the scene name
        Match match = regex.Match(sceneName);

        // Return true if found
        inCave = match.Success ? true : false;

        audioSource = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
        if (inCave) PlayMusic(caveAmbience, 0.3f);
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        clipVolume = volume;

        if (audioSource.isPlaying)
        {
            FadeOutAndPlayNew(clip);
        }
        else
        {
            audioSource.volume = clipVolume * masterVolume;
            PlayNew(clip);
        }
    }

    private void PlayNew(AudioClip clip)
    {
        currentClip = clip;
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void FadeOutAndPlayNew(AudioClip clip)
    {
        currentClip = clip;
        FadeOut();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    private void Update()
    {
        if (isFadingOut)
        {
            float deltaVolume = Time.deltaTime / fadeDuration;
            audioSource.volume -= deltaVolume;

            if (audioSource.volume <= 0.0f)
            {
                isFadingOut = false;
                PlayNew(currentClip);
                FadeIn();
            }
        }

        if (isFadingIn)
        {
            float deltaVolume = Time.deltaTime / fadeDuration;
            audioSource.volume += deltaVolume;

            if (audioSource.volume >= targetVolume)
            {
                audioSource.volume = targetVolume;
                isFadingIn = false;
            }
        }
    }

    public void FadeIn()
    {
        targetVolume = masterVolume * clipVolume;
        isFadingIn = true;
    }

    public void FadeOut()
    {
        targetVolume = 0.0f;
        isFadingOut = true;
    }
}