using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CaveEntrance : MonoBehaviour
{
    private bool isAvailible;
    private CircleCollider2D circleCollider;
    public string connectedToSceneName;
    private void Awake()
    {
        isAvailible = true;
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetActive(bool b)
    {
        this.isAvailible = b;
        circleCollider.enabled = b;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAvailible && other.CompareTag("Player"))
        {
            //Check if the scene exists
            if (SceneManager.GetSceneByName(connectedToSceneName).IsValid())
            {
                Debug.LogError(connectedToSceneName + " does not exist!");
                return;
            }
            SceneManager.LoadScene(connectedToSceneName);
        }
    }
}
