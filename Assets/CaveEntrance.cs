using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveEntrance : MonoBehaviour
{
    private bool isActive;
    private CircleCollider2D circleCollider;
    [SerializeField] private string sceneName;
    
    private void Awake()
    {
        isActive = true;
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetActive(bool b)
    {
        this.isActive = b;
        circleCollider.enabled = b;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            //Check if the scene exists
            if (SceneManager.GetSceneByName(sceneName).IsValid())
            {
                Debug.LogError("Scene " + sceneName + " does not exist!");
                return;
            }
            
            SceneManager.LoadScene(sceneName);
        }
    }
}
