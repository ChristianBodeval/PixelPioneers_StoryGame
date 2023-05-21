using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnim : MonoBehaviour
{
    [SerializeField] private float countdownDuration;

    private void Start()
    {
        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(countdownDuration);

        Destroy(gameObject);
    }
}
