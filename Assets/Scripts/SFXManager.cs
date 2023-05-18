using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton;

    public float masterVolume = 0.5f; // Should be changed from settings
    public AudioMixer masterMixer;
    private float previousPitch;

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

    public void PlaySound(AudioClip clip, float volume)
    {
        GameObject obj = Pool.pool.DrawFromSFXPool();
        AudioSource source = obj.GetComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = GetUniqueRandomPitch();
        source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Master")[0]; // Set the output AudioMixer group
        source.Play();

        obj.GetComponent<SFX>().ReturnToPool(clip.length);
    }

    private float GetUniqueRandomPitch()
    {
        float pitch;
        do
        {
            pitch = Random.Range(0.8f, 1.2f);
        } while (pitch == previousPitch);

        previousPitch = pitch;
        return pitch;
    }

    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("MasterVolume", volume);
    }
}
