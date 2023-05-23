using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pool : MonoBehaviour
{
    [Header("Enemy Pool")]
    [SerializeField] private GameObject[] enemyPrefabs = new GameObject[6];
    private List<GameObject> storageEnemies = new List<GameObject>();
    private List<GameObject> inUseEnemies = new List<GameObject>();

    [Header("Pickup Pool")]
    [SerializeField] private GameObject pickupPrefab;
    private List<GameObject> storagePickUps = new List<GameObject>();
    private List<GameObject> inUsePickUps = new List<GameObject>();
    private List<(Vector3, float)> toBeSpawned = new List<(Vector3, float)>();

    [Header("Projectile Pool")]
    [SerializeField] private GameObject projectilePrefab;
    private List<GameObject> storageProjectiles = new List<GameObject>();
    private List<GameObject> inUseProjectiles = new List<GameObject>();

    [Header("SFX Pool")]
    [SerializeField] private GameObject sfxPrefab;
    private List<GameObject> storageSFX = new List<GameObject>();
    private List<GameObject> inUseSFX = new List<GameObject>();

    [Header("Blood Pool")]
    [SerializeField] private GameObject BloodPrefab;
    [SerializeField] private GameObject BloodSplatterPSPrefab;
    private List<GameObject> storageBloods = new List<GameObject>();
    private List<GameObject> inUseBloods = new List<GameObject>();
    private List<GameObject> storageBloodSplatters = new List<GameObject>();
    private List<GameObject> inUseBloodSplatters = new List<GameObject>();


    public static Pool pool { get; private set; } // Singleton

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself
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
        StartCoroutine(SpawnPickUps()); // Constantly checking for healthpicks to spawn
    }


    /// Enemy pool

    public GameObject DrawFromEnemyPool(WaveObject.EnemyType type)
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

    public void ReturnToEnemyPool(GameObject enemy)
    {
        enemy.SetActive(false);

        if (inUseEnemies.Contains(enemy)) inUseEnemies.Remove(enemy); // Enemy is in use > remove from use

        if (!storageEnemies.Contains(enemy)) storageEnemies.Add(enemy); // Enemy is not in storage > move to storage
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

    public void ClearEnemyPool()
    {
        storageEnemies.Clear();
    }

    /// Pickup pool

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
                    GameObject pickUp = DrawFromProjectilePool();
                    pickUp.transform.position = p.Item1;
                    pickUp.GetComponent<HealPickUp>().healAmount = p.Item2;
                }
            }
            toBeSpawned.Clear();
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Gets pickup from pool
    public GameObject DrawFromPickupPool()
    {
        if (storagePickUps.Count > 0)
        {
            GameObject p = storagePickUps[0];
            storagePickUps.Remove(p);
            inUsePickUps.Add(p);
            p.SetActive(true);
            return p;
        }

        GameObject pickUp = Instantiate(pickupPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(-45f, 0f, 0f)); // New pickup
        inUsePickUps.Add(pickUp);
        pickUp.SetActive(true);
        return pickUp;
    }

    // Returns pickup to pool
    public void ReturnToPickupPool(GameObject p)
    {
        p.SetActive(false);

        if (inUsePickUps.Contains(p)) inUsePickUps.Remove(p);
        storagePickUps.Add(p);
    }

    /// Projectile pool

    // Gets pickup from pool
    public GameObject DrawFromProjectilePool()
    {
        if (storagePickUps.Count > 0)
        {
            GameObject p = storageProjectiles[0];
            storageProjectiles.Remove(p);
            inUseProjectiles.Add(p);
            p.SetActive(true);
            return p;
        }

        GameObject projectile = Instantiate(projectilePrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(-45f, 0f, 0f)); // New projectile
        inUseProjectiles.Add(projectile);
        projectile.SetActive(true);
        return projectile;
    }

    // Returns pickup to pool
    public void ReturnToProjectilePool(GameObject p)
    {
        p.SetActive(false);

        if (inUseProjectiles.Contains(p)) inUseProjectiles.Remove(p);
        storageProjectiles.Add(p);
    }

    /// SFX pool

    // Gets pickup from pool
    public GameObject DrawFromSFXPool()
    {
        if (storageSFX.Count > 0)
        {
            GameObject sfx = storageSFX[0];
            storageSFX.Remove(sfx);
            inUseSFX.Add(sfx);
            sfx.SetActive(true);
            return sfx;
        }

        GameObject s = Instantiate(sfxPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f)); // New projectile
        inUseSFX.Add(s);
        s.SetActive(true);
        return s;
    }

    // Returns pickup to pool
    public void ReturnToSFXPool(GameObject sfx)
    {
        sfx.SetActive(false);
        sfx.GetComponent<AudioSource>().loop = false;

        if (inUseSFX.Contains(sfx)) inUseSFX.Remove(sfx);
        storageSFX.Add(sfx);
    }

    /// Blood pool

    // Gets pickup from pool
    public GameObject DrawFromBloodPool()
    {
        if (storageBloods.Count > 0)
        {
            GameObject s = storageBloods[0];
            storageBloods.Remove(s);
            inUseBloods.Add(s);
            s.SetActive(true);
            return s;
        }

        GameObject blood = Instantiate(BloodPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f)); // New projectile
        inUseBloods.Add(blood);
        blood.SetActive(true);
        return blood;
    }

    // Returns pickup to pool
    public void ReturnToBloodPool(GameObject s)
    {
        s.SetActive(false);
        s.transform.localScale = new Vector3(1f, 1f, 1f); // Reset size

        if (inUseBloods.Contains(s)) inUseBloods.Remove(s);
        storageBloods.Add(s);
    }

    /// Bloodsplatter particlesystem

    // Gets pickup from pool
    public GameObject DrawFromBloodSpatterPool()
    {
        if (storageBloodSplatters.Count > 0)
        {
            GameObject ps = storageBloodSplatters[0];
            storageBloodSplatters.Remove(ps);
            inUseBloodSplatters.Add(ps);
            ps.SetActive(true);
            return ps;
        }

        GameObject bloodSplatter = Instantiate(BloodSplatterPSPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f)); // New projectile
        inUseBloodSplatters.Add(bloodSplatter);
        bloodSplatter.SetActive(true);
        return bloodSplatter;
    }

    // Returns pickup to pool
    public void ReturnToBloodSplatterPool(GameObject ps)
    {
        ps.SetActive(false);

        if (inUseBloodSplatters.Contains(ps)) inUseBloodSplatters.Remove(ps);
        storageBloodSplatters.Add(ps);
    }
}