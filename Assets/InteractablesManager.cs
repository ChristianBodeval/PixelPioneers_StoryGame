using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : MonoBehaviour
{
    public List<Interactable> interactables;
    private Transform playerTransform;


    Interactable closestInteractable = null;
    

    
    //Make it a singleton
    public static InteractablesManager instance { get; private set; }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        //Set playerTransform
        playerTransform = GameObject.Find("Player").transform;
        interactables = new List<Interactable>();
    }
    
    public void Subscribe(Interactable interactable)
    {
        interactable.OnInRange += AddToInteractibles;
        interactable.OnOutOfRange += RemoveFromInteractibles;
    }
    
    public void UnSubscribe(Interactable interactable)
    {
        interactable.OnInRange -= AddToInteractibles;
        interactable.OnOutOfRange -= RemoveFromInteractibles;
    }

    private void AddToInteractibles(object sender, Interactable interactable)
    {
        //If list does not already contain the interactable
        if (!interactables.Contains(interactable))
        {
            interactables.Add(interactable);
        }
    }
    
    private void RemoveFromInteractibles(object sender, Interactable interactable)
    {
        interactable.HideIndicatorForInRange();
        interactables.Remove(interactable);
    }


    private Interactable GetClosestInteractable()
    {
        float currentClosest = 0;
        
        Interactable currentClosestInteractable = null;
        foreach (Interactable interactable in interactables)
        {
            float range = (playerTransform.position - interactable.transform.position).magnitude;
            //Debug.Log("Range " + range);
            if (currentClosest < range)
            {
                currentClosestInteractable = interactable;
                currentClosest = range;
                Debug.Log("currentClosest" + currentClosest);
            }
        }
        
        return currentClosestInteractable;
    }
    void Update()
    {
        //If there is no interactables in range skip
        if (interactables.Count == 0)
        {
            return;
        }
        
        //Sort interactables by distance to player, the closest one is the first one
        interactables.Sort((x, y) => (playerTransform.position - x.transform.position).magnitude.CompareTo((playerTransform.position - y.transform.position).magnitude));
        
        //Distable indicator for all interactables
        foreach (Interactable interactable in interactables)
        {
            interactable.HideIndicatorForInRange();
        }
        
        //Enable indicator for the first (e.g. the closest) interactable
        if (interactables[0] != null)
            interactables[0].ShowIndicatorForInRange();
        
        //Interact with the closest interactable by pressing E
        if (interactables[0] != null && Input.GetKeyDown(KeyCode.E))
        {
            interactables[0].Interact();
        }
    }
}