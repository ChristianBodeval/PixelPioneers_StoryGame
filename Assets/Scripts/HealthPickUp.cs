using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    private List<GameObject> storagePickUps = new List<GameObject>();
    private List<GameObject> inUsePickUps = new List<GameObject>();
    private List<(Vector3, float)> toBeSpawned = new List<(Vector3, float)>();

    [SerializeField] private GameObject prefab;

    public static HealthPickUp pickUpPool { get; private set; }

    private void Awake()
    {
        //if there is an instance, and it's not me, delete myself
        //if (pickUpPool != null && pickUpPool != this)
        //{
        //    Destroy(this);
        //}
        //else
        //{
        //}
    }


    private void Start()
    {
            pickUpPool = this;
        StartCoroutine(SpawnPickUps()); // Constantly checking for healthpicks to spawn
    }

    // Adds healthpickup to list for later spawning
    public void AddHealthPickUp(Vector3 pos, float healAmount)
    {
        lock (toBeSpawned) toBeSpawned.Add((pos, healAmount)); // Add pickup values and lock the list so it cant be used while we edit
    }

    // Spawns everything from the pickup list
    private IEnumerator SpawnPickUps()
    {
        while (true)
        {
            lock (toBeSpawned)
            {
                foreach (var p in toBeSpawned)
                {
                    GameObject pickUp = DrawFromPool();
                    pickUp.transform.position = p.Item1;
                    pickUp.GetComponent<HealPickUp>().healAmount = p.Item2;
                }
            }
            toBeSpawned.Clear();
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Gets pickup from pool
    public GameObject DrawFromPool()
    {
        if (storagePickUps.Count > 0)
        {
            GameObject p = storagePickUps[0];
            storagePickUps.Remove(p);
            inUsePickUps.Add(p);
            p.SetActive(true);
            return p;
        }

        GameObject pickUp = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(-45f, 0f, 0f)); // New pickup
        inUsePickUps.Add(pickUp);
        pickUp.SetActive(true);
        return pickUp;
    }

    // Returns pickup to pool
    public void ReturnToPool(GameObject p)
    {
        p.SetActive(false);

        if (inUsePickUps.Contains(p)) inUsePickUps.Remove(p);
        storagePickUps.Add(p);
    }

    public void ClearLists()
    {
        storagePickUps.Clear();
        inUsePickUps.Clear();
    }
}
