using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager singleton;

    [Range(0f, 1f)] public float masterVolume = 0.5f;
    [SerializeField] private AudioClip enterCaveSFX;
    public AudioMixer masterMixer;
    private float previousPitch;
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
            singleton = this;
        }
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
        string pattern = @"\bCave\b";

        // Create a regex object with the pattern
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Perform the regex match on the scene name
        Match match = regex.Match(sceneName);

        // Return true if found
        inCave = match.Success ? true : false;

        if (inCave) PlaySound(enterCaveSFX, GameObject.Find("Player").transform.position, masterVolume);
    }

    public IEnumerator PlaySoundWithDelay(AudioClip clip, Vector2 pos, float volume = 1f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = Pool.pool.DrawFromSFXPool();
        AudioSource source = obj.GetComponent<AudioSource>();

        obj.transform.position = pos;

        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = GetUniqueRandomPitch();
        source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Master")[0]; // Set the output AudioMixer group
        source.Play();

        obj.GetComponent<SFX>().ReturnToPool(clip.length);
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
        //source.pitch = GetUniqueRandomPitch();
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
