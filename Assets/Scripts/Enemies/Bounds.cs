using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask enemyLayers;
    private RaycastHit2D[] enemies;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        StartCoroutine(CheckMobBounds()); 
    }

    // Checks if mobs are out of bounds, in a wall or outside the playable area
    private IEnumerator CheckMobBounds()
    {
        while (true)
        {
            enemies = Physics2D.CircleCastAll(Vector2.zero, 500f, Vector2.zero, 500f, enemyLayers);

            yield return new WaitForSeconds(0.4f);

            foreach (RaycastHit2D e in enemies)
            {
                if (!Physics2D.OverlapPoint(e.transform.position, groundLayer) || Physics2D.OverlapPoint(e.transform.position, obstacleLayer))
                {
                    StartCoroutine(FindClosestValidPosition(e.transform.gameObject));
                }
                yield return null;
            }

            // Check if player is out of bounds
            if (!Physics2D.OverlapPoint(player.transform.position, groundLayer) || Physics2D.OverlapPoint(player.transform.position, obstacleLayer))
            {
                StartCoroutine(FindClosestValidPosition(player));
            }
        }
    }

    private IEnumerator FindClosestValidPosition(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        Vector3 dir;

        for (int i = 1; i < 20; i++)
        {
            dir = GetRandomDirection();

            // Does the position have ground and is it unobscured?
            if (Physics2D.CircleCast(pos + dir * i * 0.2f, 0.3f, Vector3.right, 0.3f, groundLayer) && !Physics2D.CircleCast(pos + dir * i * 0.2f, 0.3f, Vector3.right, 0.3f, obstacleLayer))
            {
                StartCoroutine(LerpToNewPosition(obj, pos + dir * i * 0.2f)); // New position
                yield break;
            }
        }

        obj.transform.position = Vector3.zero; // Default case
    }

    private IEnumerator LerpToNewPosition(GameObject obj, Vector3 newPos)
    {
        // Return animator if it isnt null (coalescing expression)
        Animator animator = obj.GetComponentInChildren<Animator>() ?? null;
        Vector3 startPos = obj.transform.position;
        Vector2 dir = (newPos - startPos).normalized;
        float distance = Vector2.Distance(obj.transform.position, newPos);
        float t = 0;
        float startTime = Time.time;

        while (distance > 0.1f && startTime < Time.time - 1f)
        {
            obj.transform.position = Vector2.Lerp(startPos, newPos, t);
            t += 0.1f;
            yield return new WaitForSeconds(0.02f);
        }

        obj.transform.position = newPos;
    }

    private Vector2 GetRandomDirection()
    {
        float x = Random.Range(-1, 2);
        float y = Random.Range(-1, 2);
        return new Vector2(x, y).normalized;
    }
}
