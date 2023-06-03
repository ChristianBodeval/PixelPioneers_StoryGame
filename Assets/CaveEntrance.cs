using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CaveEntrance : MonoBehaviour
{
    public int caveNumber;
    
    //Make a custom setter for this and it should still be accessible from the inspector
    [SerializeField]
    private bool isAcessible;
    
    [SerializeField]
    public Transform spawnPoint;
    
    
    void OnValidate()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        SetAccessibility(isAcessible);
    }
    
    GameObject player;
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }
    void Start()
    {
        if (connectedToSceneName.Equals(ProgressManager.instance.lastSceneName))
        {
            player.transform.position = spawnPoint.position;
        }
    }

    
    [SerializeField] private CircleCollider2D circleCollider;
    public string connectedToSceneName;
    
    
    public void SetAccessibility(bool isAccessible)
    {
        this.isAcessible = isAccessible;
        circleCollider.enabled = isAccessible;
        
        foreach (Light2D light2D in GetComponentsInChildren<Light2D>())
        {
            light2D.enabled = isAccessible;
        }
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAcessible && other.CompareTag("Player"))
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
