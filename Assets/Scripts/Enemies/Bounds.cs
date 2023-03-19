using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask enemyLayers;
    private RaycastHit2D[] enemies;

    private void Start()
    {
        StartCoroutine(CheckMobBounds()); 
    }

    // Checks if mobs are out of bounds, in a wall or outside the playable area
    private IEnumerator CheckMobBounds()
    {
        while (true)
        {
            enemies = Physics2D.CircleCastAll(Vector2.zero, 500f, Vector2.zero, 500f, enemyLayers);

            yield return new WaitForSeconds(5f);

            foreach (RaycastHit2D e in enemies)
            {
                if (!Physics2D.OverlapPoint(e.transform.position, groundLayer) || Physics2D.OverlapPoint(e.transform.position, obstacleLayer))
                {
                    e.transform.position = Vector3.zero;
                }
                yield return null;
            }
        }
    }
}
