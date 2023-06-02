using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    private GameObject postProcessing;

    private void Start()
    {
        postProcessing = GameObject.Find("PostProcessing");

        Vignette vignette;
        if (postProcessing.GetComponent<Volume>().profile.TryGet(out vignette))
        {
            vignette.intensity.Override(1f); // Set the intensity to 1
        }
    }

    public void ResetLevel()
    {
        Vignette vignette;
        if (postProcessing.GetComponent<Volume>().profile.TryGet(out vignette))
        {
            vignette.intensity.Override(0.1f); // Set the intensity to 0.1
        }

        GameObject.Find("GameManager").GetComponent<SpawnSystem>().wavesToSpawn.Clear();

        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
