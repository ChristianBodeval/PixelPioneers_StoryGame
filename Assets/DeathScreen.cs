using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    private GameObject postProcessing;
    Vignette vignette;

    private void Awake()
    {
        postProcessing = GameObject.Find("GameManager");
        postProcessing.GetComponent<Volume>().profile.TryGet(out vignette);
    }

    private void OnEnable()
    {
        if (vignette != null)
        {
            vignette.intensity.Override(1f); // Set the intensity to 1
        }
    }

    private void OnDisable()
    {
        if (vignette != null)
        {
            vignette.intensity.Override(0.1f); // Set the intensity to 1
        }
        else
        {
            postProcessing.GetComponent<Volume>().profile.TryGet(out vignette);
            vignette.intensity.Override(0.1f); // Set the intensity to 1
        }
    }

    public void ResetLevel()
    {
        if (postProcessing.GetComponent<Volume>().profile.TryGet(out vignette))
        {
            vignette.intensity.Override(0.1f); // Set the intensity to 0.1
        }

        Time.timeScale = 1f;

        GameObject.Find("GameManager").GetComponent<SpawnSystem>().ClearLists();
        SendWave.isSent = false;

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
