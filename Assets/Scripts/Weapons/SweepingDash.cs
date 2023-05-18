using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SweepingDash : MonoBehaviour
{
    public GameObject slashCollider;


    public void TurnAreaOn()
    {
        slashCollider.SetActive(true);

    } 
    public void TurnAreaOff()
    {
        slashCollider.SetActive(false);

    }
}
