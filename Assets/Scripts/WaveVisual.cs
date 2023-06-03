using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveVisual : MonoBehaviour
{
    [Header("Music")]
    [Range(0, 1)] public float musicVolume = 1f;
    [SerializeField] private AudioClip combatTrack;

    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip startCombatSFX;
    [SerializeField] private AudioClip breakCrystalSFX;
    private bool isInCombat = false;

    [Header("UI")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject chainPrefab;
    private List<GameObject> inUseWaveIndicators = new List<GameObject>();
    private List<GameObject> waveIndicatorsPool = new List<GameObject>();
    private List<GameObject> inUseWaveChain = new List<GameObject>();
    private List<GameObject> waveChainPool = new List<GameObject>();
    private float crystalWidth;
    private float spacing = 60f;
    private float waveUIWidth;

    private int maxWaves;
    private int wavesLeft = 0; // Changed through Spawnsystem script

    public Sprite currentWave;
    public Sprite unbrokenCrystal;
    public Sprite questionSprite;

    [SerializeField] private ParticleSystem particleSystem;

    private void Start()
    {
        if (!Pool.pool.isActiveAndEnabled) return;

        isInCombat = false;
        maxWaves = SpawnSystem.totalWaves;
        wavesLeft = maxWaves - SpawnSystem.currentWave;

        StartCoroutine(SetWave()); // Sets the amouont of active crystals
        StartCoroutine(UpdateIndicatorList());
    }

    private IEnumerator UpdateIndicatorList()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            // No change in variables
            if (maxWaves == SpawnSystem.totalWaves && wavesLeft == maxWaves - SpawnSystem.currentWave) continue; // Guard clause

            // Updates variables from SpawnSystem script
            maxWaves = SpawnSystem.totalWaves;
            wavesLeft = maxWaves - SpawnSystem.currentWave;

            // Acquire more indicators
            while (inUseWaveIndicators.Count < maxWaves)
            {
                DrawFromIndicatorPool();
            }

            // Acquire more chains
            while (inUseWaveChain.Count < maxWaves - 1)
            {
                DrawFromChainPool();
            }

            // Reduce indicator amount
            while (inUseWaveIndicators.Count > maxWaves)
            {
                ReturnToIndicatorPool(inUseWaveIndicators[0]);
            }

            // Reduce chain amount
            while (inUseWaveChain.Count > maxWaves - 1 && maxWaves > 0)
            {
                ReturnToChainPool(inUseWaveChain[0]);
            }

            // Width of UI
            waveUIWidth = maxWaves * crystalWidth + (maxWaves - 1) * spacing;
            float x = waveUIWidth / 2; // Start from the right side (centered x position)
            int i = 0;

            // Set position of each indicator
            foreach (GameObject indicator in inUseWaveIndicators)
            {
                float objX = x - (i * spacing);
                indicator.transform.localPosition = new Vector3(objX, 224f, indicator.transform.localPosition.z);
                i++;
            }

            i = 0; // Reset variable

            // Set position of each chain
            foreach (GameObject chain in inUseWaveChain)
            {
                float objX = x - (i * spacing) - spacing * 0.5f;
                chain.transform.localPosition = new Vector3(objX, 224f, chain.transform.localPosition.z);
                i++;
            }
        }
    }

    private GameObject DrawFromIndicatorPool()
    {
        if (waveIndicatorsPool.Count > 0)
        {
            GameObject obj = waveIndicatorsPool[0];
            inUseWaveIndicators.Add(obj);
            waveIndicatorsPool.Remove(obj);
            obj.GetComponentInChildren<Image>().color = Color.red;
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab, transform);
            inUseWaveIndicators.Add(obj);
            return obj;
        }
    }

    private void ReturnToIndicatorPool(GameObject obj)
    {
        obj.SetActive(false);
        waveIndicatorsPool.Add(obj);
        inUseWaveIndicators.Remove(obj);
    }

    private GameObject DrawFromChainPool()
    {
        if (waveIndicatorsPool.Count > 0)
        {
            GameObject obj = waveIndicatorsPool[0];
            inUseWaveChain.Add(obj);
            waveChainPool.Remove(obj);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(chainPrefab, transform);
            inUseWaveChain.Add(obj);
            return obj;
        }
    }

    private void ReturnToChainPool(GameObject obj)
    {
        obj.SetActive(false);
        waveChainPool.Add(obj);
        inUseWaveChain.Remove(obj);
    }

    private IEnumerator SetWave()
    {
        while (true)
        {
            if (wavesLeft > 0 && !isInCombat)
            {
                isInCombat = true;

                Transform t = GameObject.Find("Player").transform;
                SFXManager.singleton.PlaySound(startCombatSFX, t.position, sfxVolume, false, t);
                MusicManager.singleton.PlayMusic(combatTrack, musicVolume);
            }

            yield return new WaitForSeconds(0.05f);

            if (inUseWaveIndicators.Count < 1) continue; // Skip this loop iteration

            int i = 0;

            lock (inUseWaveIndicators)
            {
                foreach (GameObject wave in inUseWaveIndicators)
                {
                    i++;

                    Image image = wave.activeSelf ? wave.GetComponentInChildren<RemoveFill>().GetImageComponent() : null;
                    Image backDrop = wave.activeSelf ? wave.GetComponent<Image>() : null;

                    // Reset wave variables
                    image.sprite = questionSprite;
                    backDrop.enabled = true;
                    RectTransform rt = image.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(80f, 20f);
                    wave.SetActive(true);

                    // Last wave / boss
                    if (i == 1)
                    {
                        // Assign your unique image sprite here
                        image.sprite = questionSprite;
                        backDrop.enabled = false;
                        rt.sizeDelta = new Vector2(50f, 50f);
                    }
                    // Current wave
                    else if (i == wavesLeft)
                    {
                        image.sprite = currentWave;
                        Color orange = new Color(245,96,0);
                        image.color = orange;
                    }
                    // Coming wave
                    else if (i < wavesLeft)
                    {
                        image.sprite = unbrokenCrystal;
                        image.color = Color.red;
                    }
                    // Completed wave
                    else if (i > wavesLeft)
                    {
                        var script = wave.GetComponentInChildren<RemoveFill>();
                        if (script.IsActive()) ExplodeCrystal(wave);
                        script.Remove(); // Deactivates fill for indicator
                    }
                }

                yield return null;
            }
        }
    }

    private void ExplodeCrystal(GameObject obj)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(obj.transform.position);
        GameObject player = GameObject.Find("Player");
        ParticleSystem ps = Instantiate(particleSystem, worldPosition, player.transform.rotation, player.transform);
        SFXManager.singleton.PlaySound(breakCrystalSFX, player.transform.position, sfxVolume, false, player.transform);
    }
}