using System;
using UnityEngine;

public interface IInteractable
{
    Transform transform { get; set; }
    InteractablesManager interactablesManager { get; set; }
    
    event EventHandler<IInteractable> OnInRange;
    event EventHandler<IInteractable> OnOutOfRange;
    
    void Subscribe(InteractablesManager interactablesManager);
    void UnSubscribe(InteractablesManager interactablesManager);
    
    void ShowIndicator();
    void HideIndicator();
    void Interact();
    
    
}