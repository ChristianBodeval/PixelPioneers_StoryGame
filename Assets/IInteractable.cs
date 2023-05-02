using System;
using UnityEngine;

public interface IInteractable
{
    Transform transform { get; set; }
    InteractablesManager interactablesManager { get; set; }
    
    public event EventHandler<IInteractable> OnInRange;
    public event EventHandler<IInteractable> OnOutOfRange;
    
    void Subscribe(InteractablesManager interactablesManager);
    void UnSubscribe(InteractablesManager interactablesManager);
    
    void ShowIndicator();
    void HideIndicator();
    void Interact();
    
    
}