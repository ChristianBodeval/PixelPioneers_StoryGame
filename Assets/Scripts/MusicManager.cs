using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager singleton;

    [Range(0f, 1f)] public float masterVolume = 1f;
    public AudioSource audioSource;
    public float fadeDuration = 1.0f;
    private AudioClip currentClip;
    private float targetVolume;
    private bool isFading;

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            FadeOutAndPlayNew(clip);
        }
        else
        {
            PlayNew(clip);
        }
    }

    private void PlayNew(AudioClip clip)
    {
        currentClip = clip;
        audioSource.clip = clip;
        audioSource.volume = masterVolume;
        audioSource.Play();
    }

    private void FadeOutAndPlayNew(AudioClip clip)
    {
        currentClip = clip;
        targetVolume = 0.0f;
        isFading = true;
    }

    private void Update()
    {
        if (isFading)
        {
            float deltaVolume = Time.deltaTime / fadeDuration;
            audioSource.volume -= deltaVolume;

            if (audioSource.volume <= 0.0f)
            {
                isFading = false;
                PlayNew(currentClip);
                FadeIn();
            }
        }
    }

    private void FadeIn()
    {
        targetVolume = 1.0f;
        isFading = true;
    }

    private void FixedUpdate()
    {
        if (isFading)
        {
            float deltaVolume = Time.deltaTime / fadeDuration;
            audioSource.volume += deltaVolume;

            if (audioSource.volume >= targetVolume)
            {
                audioSource.volume = targetVolume;
                isFading = false;
            }
        }
    }
}
