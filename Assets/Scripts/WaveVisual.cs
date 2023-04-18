using UnityEngine;
using UnityEngine.UI;

public class WaveVisual : MonoBehaviour
{
    public Image[] waves;

    private int maxWaves;
    private int wavesLeft = 0; // Changed through Spawnsystem script

    public Sprite crackedCrystal;
    public Sprite unCrackedCrystal;

    private void Start()
    {
        maxWaves = SpawnSystem.totalWaves;

        waves[0].enabled = false;
        waves[1].enabled = false;
        waves[2].enabled = false;
        waves[3].enabled = false;
    }

    private void Update()
    {
        maxWaves = SpawnSystem.totalWaves; // Updates the maxWaves
        wavesLeft = maxWaves - SpawnSystem.currentWave;

        if (wavesLeft >= maxWaves) wavesLeft = maxWaves; // We have more waves left

        if (wavesLeft < -1) wavesLeft = -1; // Wavesleft value cannot go below

        SetWave(); // Sets the amouont of active crystals
    }

    public void WaveHalfWayThrough()
    {
        if (waves[wavesLeft - 1].sprite == crackedCrystal)
            waves[wavesLeft - 1].sprite = unCrackedCrystal;
        else
            waves[wavesLeft - 1].sprite = crackedCrystal;
    }

    public void SetWave()
    {
        switch (wavesLeft)
        {
            case 0:
                waves[0].enabled = false;
                waves[1].enabled = false;
                waves[2].enabled = false;
                waves[3].enabled = false;
                break;

            case 1:
                waves[0].enabled = true;
                waves[1].enabled = false;
                waves[2].enabled = false;
                waves[3].enabled = false;
                break;

            case 2:
                waves[0].enabled = true;
                waves[1].enabled = true;
                waves[2].enabled = false;
                waves[3].enabled = false;
                break;

            case 3:
                waves[0].enabled = true;
                waves[1].enabled = true;
                waves[2].enabled = true;
                waves[3].enabled = false;
                break;

            case 4:
                waves[0].enabled = true;
                waves[1].enabled = true;
                waves[2].enabled = true;
                waves[3].enabled = true;
                break;

            default:
                foreach (var item in waves)
                {
                    item.enabled = false;
                }
                break;
        }
    }
}