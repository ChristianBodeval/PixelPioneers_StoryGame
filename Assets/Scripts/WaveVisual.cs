using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveVisual : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject chainPrefab;
    private List<GameObject> inUseWaveIndicators = new();
    private List<GameObject> waveIndicatorsPool = new();
    private List<GameObject> inUseWaveChain = new ();
    private List<GameObject> waveChainPool = new ();
    private float crystalWidth;
    private float spacing = 60f;
    private float waveUIWidth;

    private int maxWaves;
    private int wavesLeft = 0; // Changed through Spawnsystem script

    public Sprite currentWave;
    public Sprite unbrokenCrystal;

    private void Start()
    {
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
            float x = waveUIWidth / 2;
            int i = 0;

            // Set position of each indicator
            foreach (GameObject indicator in inUseWaveIndicators)
            {
                float objX = (i * spacing) - (x);
                indicator.transform.localPosition = new Vector3(objX, 224f, indicator.transform.localPosition.z);
                i++;
            }

            i = 0; // Reset variable

            // Set position of each chain
            foreach (GameObject chain in inUseWaveChain)
            {
                float objX = (i * spacing) - (x) + spacing * 0.5f;
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
            yield return new WaitForSeconds(0.1f);

            if (inUseWaveIndicators.Count < 1) continue; // Skip this loop iteration

            int i = inUseWaveIndicators.Count;

            lock (inUseWaveIndicators)
            {
                foreach (GameObject wave in inUseWaveIndicators)
                {
                    i--;

                    var indicator = inUseWaveIndicators[i];

                    if (i == wavesLeft && i >= 0)
                    {
                        Image image = GetComponentInChildren<RemoveFill>().GetImageComponent();
                        image.sprite = currentWave;
                        indicator.SetActive(true);
                    }
                    else if (i < wavesLeft && i >= 0)
                    {
                        Image image = GetComponentInChildren<RemoveFill>().GetImageComponent();
                        image.sprite = unbrokenCrystal;
                        indicator.SetActive(true);
                    }
                    else if (i > wavesLeft && i >= 0)
                    {
                        indicator.GetComponentInChildren<RemoveFill>().Remove(); // Deactivates fill for indicator
                        indicator.SetActive(true);
                    }
                }

                yield return null;
            }
        }
    }
}