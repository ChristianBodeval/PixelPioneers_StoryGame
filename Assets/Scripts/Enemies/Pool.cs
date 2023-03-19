using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pool : MonoBehaviour
{
    private List<GameObject> storageEnemies = new List<GameObject>();
    private List<GameObject> inUseEnemies = new List<GameObject>();
    [SerializeField] private GameObject[] enemyPrefabs = new GameObject[6];

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

    public GameObject DrawFromPool(WaveObject.EnemyType type)
    {
        if (storageEnemies.Count > 0)
        {
            foreach (GameObject e in storageEnemies)
            {
                if (e.name.Contains(type + ""))
                {
                    storageEnemies.Remove(e);
                    inUseEnemies.Add(e);
                    e.SetActive(true);
                    return e;
                }
            }
        }

        GameObject enemy = Instantiate(enemyPrefabs[(int)type], new Vector3(0f, 0f, 0f), Quaternion.identity); // New enemy
        inUseEnemies.Add(enemy);
        enemy.SetActive(true);
        return enemy;
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

    // Instansiate enemies before they fully spawn such that we don't lag during combat
    public IEnumerator PrimeEnemies(WaveObject wave)
    {
        /*
        foreach (var item in collection)
        {
            // Spawn enemies offscreen
            // Deactivate
            yield return null;
        }
        */
        yield return null;
    }

    public void ClearPool()
    {
        storageEnemies.Clear();
    }
}