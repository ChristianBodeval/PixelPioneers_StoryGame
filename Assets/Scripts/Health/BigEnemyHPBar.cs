using UnityEngine;
using UnityEngine.UI;

public class BigEnemyHPBar : MonoBehaviour
{
    public Image HPFill;
    public Image DamagedBar;

    private Health health;
    private float damagedHealthShrinkTimer;

    private void Start()
    {
        health = GetComponent<Health>();
        HPFill.fillAmount = health.currentHealth;
        DamagedBar.fillAmount = HPFill.fillAmount;
    }

    // StateUpdate is called once per frame
    private void Update()
    {
        HPFill.fillAmount = health.currentHealth / health.maxHealth;
        damagedHealthShrinkTimer -= Time.deltaTime;

        if (damagedHealthShrinkTimer < 0)
        {
            if (HPFill.fillAmount <= DamagedBar.fillAmount)
            {
                float shrinkSpeed = 0.05f;
                DamagedBar.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }
}