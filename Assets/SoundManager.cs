using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager singleton;
    public AudioMixer masterMixer;

    private void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Vector2 pos, float volume = 1f, Transform parentTransform = null)
    {
        GameObject soundObj = new GameObject("Sound");
        soundObj.transform.position = pos;
        soundObj.transform.parent = parentTransform ?? null; // Set parent such that sound follows the parent
        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Master")[0]; // Set the output AudioMixer group
        source.Play();

        Destroy(soundObj, clip.length);
    }

    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("MasterVolume", volume);
    }
}
