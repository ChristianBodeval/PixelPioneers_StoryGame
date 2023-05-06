using System;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    public event EventHandler<Interactable> OnInRange;
    public event EventHandler<Interactable> OnOutOfRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnInRange?.Invoke(this, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnOutOfRange?.Invoke(this, this);
        }
    }

    private IEnumerator SetInteractablesManager()
    {
        if (InteractablesManager.instance == null)
        {
            //Wait until not null
            yield return new WaitUntil(() => InteractablesManager.instance != null);
        }
        
        InteractablesManager.instance.Subscribe(this);
            
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }
    
    

    public void Subscribe()
    {
        StartCoroutine(SetInteractablesManager());
    }

    public void UnSubscribe()
    {
        InteractablesManager.instance.UnSubscribe(this);
    }

    public abstract void ShowIndicatorForInRange();

    public abstract void HideIndicatorForInRange();

    public abstract void Interact();
}