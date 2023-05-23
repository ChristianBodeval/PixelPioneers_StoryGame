using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager singleton;

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

    public void PlaySound(AudioClip clip, Vector2 pos, float volume = 1f, bool isLooping = false, Transform parent = null)
    {
        GameObject obj = Pool.pool.DrawFromSFXPool();
        AudioSource source = obj.GetComponent<AudioSource>();

        obj.transform.position = pos;
        obj.transform.parent = parent ?? null; // If parent isn't null, set parent to parent transform

        source.loop = isLooping;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = GetUniqueRandomPitch();
        source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Master")[0]; // Set the output AudioMixer group
        source.Play();

        obj.GetComponent<SFX>().ReturnToPool(clip.length);
    }

    public GameObject PlayLoop(AudioClip clip, Vector2 pos, float volume = 1f, bool isLooping = false, Transform parent = null)
    {
        GameObject obj = Pool.pool.DrawFromSFXPool();
        AudioSource source = obj.GetComponent<AudioSource>();

        obj.transform.position = pos;
        obj.transform.parent = parent ?? null; // If parent isn't null, set parent to parent transform

        source.loop = isLooping;
        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = GetUniqueRandomPitch();
        source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Master")[0]; // Set the output AudioMixer group
        source.Play();

        return obj;
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
