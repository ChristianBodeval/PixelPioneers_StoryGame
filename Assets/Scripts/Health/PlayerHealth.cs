using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Slider HP;

    // Constructor
    public PlayerHealth(float startingHealth, float maxHealth) : base(startingHealth, maxHealth)
    {
        this.currentHealth = startingHealth;
        this.maxHealth = maxHealth;
    }

    private void Update()
    {
        HP.value = currentHealth;
        if (this.currentHealth <= 0)
        {
            Die();
        }
    }
}
