using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BigEnemyHPBar : MonoBehaviour
{
    public Slider HP;
    public Gradient gradient;
    public Image HPFill;
    private Health health;


    private void Start()
    {
        health = GetComponent<Health>();
        HP.value = health.currentHealth;
        HPFill.color = gradient.Evaluate(HP.normalizedValue);
    }

    // StateUpdate is called once per frame
     private void Update()
    {
        HP.value = health.currentHealth;
        HPFill.color = gradient.Evaluate(HP.normalizedValue);
    }
}