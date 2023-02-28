using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pool : MonoBehaviour
{
    private List<GameObject> storageEnemies = new List<GameObject>();
    private List<GameObject> inUseEnemies = new List<GameObject>();
    [Header("Spawner")]
    [SerializeField] private float spawnFrequency = 5f;
    [SerializeField] private GameObject prefab;
    private Camera camera;
    private Tilemap obstacleTileMap;
    private Tilemap groundTileMap;

    public static Pool pool { get; private set; }

    private void Awake()
    {
        //if there is an instance, and it's not me, delete myself
        if (pool != null && pool != this)
        {
            Destroy(this);
        }
        else
        {
            pool = this;
        }
    }

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnFrequency);
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        obstacleTileMap = GameObject.FindGameObjectWithTag("Obstacles").GetComponent<Tilemap>();
        groundTileMap = GameObject.FindGameObjectWithTag("Ground").GetComponent<Tilemap>();
    }

    public GameObject DrawFromPool()
    {
        if (storageEnemies.Count > 0)
        {
            GameObject enemy = storageEnemies[0];
            storageEnemies.Remove(enemy);
            inUseEnemies.Add(enemy);
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            //**spawn outside screen
            GameObject enemy = Instantiate(prefab, new Vector3(0f,0f,0f), Quaternion.identity); //new enemy
            inUseEnemies.Add(enemy);
            enemy.SetActive(true);
            return enemy;
        }
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);

        if (inUseEnemies.Contains(enemy))
        {
            inUseEnemies.Remove(enemy);
        }

        storageEnemies.Add(enemy);
    }

    private void SpawnEnemy()
    {
        GameObject enemy = DrawFromPool();
        enemy.transform.position = FindSpawnPoint();
    }

    //returns a point eligible for spawning an enemy outside of the screen
    private Vector2 FindSpawnPoint()
    {
        float width = camera.pixelWidth / 96f + 0.5f; //half the width of the screen
        float height = camera.pixelHeight / 96f + 0.5f; //half the height of the screen

        Vector2 point = new Vector2();

        //random edge of the screen
        switch (Random.Range(1,5))
        {
            case 1:
                point = new Vector2(-width - 0.5f, Random.Range(0, height)); //left
                break;
            case 2:
                point = new Vector2(Random.Range(0, width), height + 0.5f); //top
                break;
            case 3:
                point = new Vector2(width + 0.5f, Random.Range(0, height)); //right
                break;
            case 4:
                point = new Vector2(Random.Range(0, width), -height -0.5f); //bottom
                break;
            default:
                return new Vector2(Random.Range(-width, width), Random.Range(-height, height));
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(point + Vector2.up, Vector2.down);

        //checks if we have ground and no obstacles in the way
        bool hasGround = false;
        foreach (RaycastHit2D hit in hits)
        {
            /*
            if (hit.collider.gameObject.CompareTag("Obstacles")) //obstaclecheck
            {
                Debug.Log("Obstacle" + point);
                point = new Vector2();
            }
            else if (hit.collider.gameObject.CompareTag("Ground") && !hasGround) //groundcheck
            {
                hasGround = true;
            }
            */
        }

        return point + (Vector2)GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
