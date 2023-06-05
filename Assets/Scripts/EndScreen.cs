using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    private Image circle;
    // Start is called before the first frame update
    void Start()
    {
        circle = GetComponent<Image>();
        circle.fillAmount = 0;
    }
   
    // Update is called once per frame
    void FixedUpdate()
    {
        circle.fillAmount += Time.deltaTime / 4;
    }
    
}
