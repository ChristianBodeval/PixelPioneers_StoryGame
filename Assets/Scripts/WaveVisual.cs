using UnityEngine;
using UnityEngine.UI;

public class WaveVisual : MonoBehaviour
{
    public Image[] waves;

    public int waveCount = 4;
    public int wavesLeft;

    public Sprite crackedCrystal;
    public Sprite unCrackedCrystal;

    private void Start()
    {
        waves[0].enabled = false;
        waves[1].enabled = false;
        waves[2].enabled = false;
        waves[3].enabled = false;
        wavesLeft = waveCount;
    }

    private void Update()
    {
        SetWave();

        if (wavesLeft >= waveCount)
        {
            wavesLeft = waveCount;
        }

        if (wavesLeft <= 0)
        {
            wavesLeft = 0;
        }
    }

    public void AddWave()
    {
        wavesLeft++;
    }

    public void RemoveWave()
    {
        wavesLeft--;
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
        }
    }
}