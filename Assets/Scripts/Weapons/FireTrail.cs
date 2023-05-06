using System.Collections;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    public float timeToDie = 3f;
    public float damage = 0.1f;
    private bool isInFire;
    [SerializeField] private LayerMask enemyLayers;

    private void Start()
    {
        StartCoroutine(DieAfterTime(timeToDie));
    
    
    }
    private RaycastHit2D[] CheckForEnemies(float radius, Vector2 direction)
    {
        return Physics2D.CircleCastAll(transform.position, radius, direction, radius, enemyLayers);
    }
    private IEnumerator DieAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health enemy = collision.gameObject.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        Health enemy = collision.GetComponent<Health>();
    //        if (enemy != null)
    //        {
    //            enemy.TakeDamage(damage);
    //        }

    //    }
    //}
}
