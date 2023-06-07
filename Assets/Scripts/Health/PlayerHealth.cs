using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 0.5f;
    [FormerlySerializedAs("HP")] public Slider hpSlider;
    [FormerlySerializedAs("DamagedBar")] public Slider damagedSlider;
    private float damagedHealthShrinkTimer;
    private bool invulnerable = false;
    private Coroutine invulnerableCoroutine;
    public CameraShake cameraShake;
    public Gradient gradient;
    public Image HPFill;
    private GameObject deathScreen;

    [SerializeField] private AudioClip damageTaken;
    
    public UnityEvent playerDeathEvent;
    
    // Constructor
    public PlayerHealth(float startingHealth, float maxHealth) : base(startingHealth, maxHealth)
    {
        this.currentHealth = startingHealth;
        this.maxHealth = maxHealth;
    }

    private void Awake()
    {
        
        damagedSlider = GameObject.Find("DamagedSlider").GetComponent<Slider>();

        hpSlider = GameObject.Find("Healthbar").GetComponent<Slider>();

        HPFill = GameObject.Find("Fill-HP").GetComponent<Image>();
        
        sr = GetComponentInChildren<SpriteRenderer>();

        damagedSlider.value = hpSlider.value;
    }

    private void Start()
    {
        cameraShake = GameObject.Find("Camera").GetComponent<CameraShake>();


        Debug.Log(GameObject.Find("UserInterface"));
        
        if (GameObject.Find("UserInterface") != null)
        {
            foreach (var item in GameObject.Find("UserInterface").GetComponentsInChildren<Transform>())
            {
                if (item.name.Equals("PlayerDeath")) deathScreen = item.gameObject;
            }
        }
         
            
        
        if (deathScreen != null) deathScreen.SetActive(false);

        
        
        
        
    }

    public override void TakeDamage(float damage)
    {
        damagedHealthShrinkTimer = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        if (!invulnerable)
        {
            SFXManager.singleton.PlaySound(damageTaken, transform.position, sfxVolume);

            cameraShake.TakesDamage();
            this.currentHealth -= damage;
            if (invulnerableCoroutine != null) StopCoroutine(invulnerableCoroutine);
            invulnerableCoroutine = StartCoroutine(MomentaryInvulnerability());

            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));
        }
    }

    // Changes material and color for a duration
    public override IEnumerator BlinkOnDmgTaken(float duration = 0.15f)
    {
        if (deathCoroutine != null) yield break;

        Material blinkMat = Instantiate(MaterialManager.singleton.blinkMaterial);
        sr.material = blinkMat;
        sr.material.color = Color.white;

        yield return new WaitForSeconds(duration);

        sr.material = MaterialManager.singleton.baseMaterial;
        sr.material.color = Color.white;

        blinkCoroutine = null;
    }

    public override void HealDamage(float healAmount)
    {
        this.currentHealth += healAmount;
        if (this.currentHealth > maxHealth)
        {
            this.currentHealth = maxHealth;
        }
        damagedSlider.value = hpSlider.value;
    }

    public void AddInvulnerability()
    {
        invulnerable = true;
    }

    public void RemoveInvulnerability()
    {
        invulnerable = false;
    }

    private IEnumerator MomentaryInvulnerability()
    {
        invulnerable = true;

        yield return new WaitForSeconds(0.05f);

        invulnerable = false;
    }

    protected override void Update()
    {
        // Use this for the Boss Healthbar aswell
        if (this.currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        
        if(hpSlider == null) return;
        hpSlider.value = currentHealth;
        HPFill.color = gradient.Evaluate(hpSlider.normalizedValue);
        
        damagedHealthShrinkTimer -= Time.deltaTime;
        if (damagedHealthShrinkTimer < 0)
        {
            if (hpSlider.value <= damagedSlider.value) {
                float shrinkSpeed = 10f;
                damagedSlider.value-= shrinkSpeed * Time.deltaTime;
            }
        }
    }

    public override IEnumerator Die()
    {
        SFXManager.singleton.PlaySound(deathSFX, transform.position, sfxVolume);
        MusicManager.singleton.StopMusic();
        gameObject.SetActive(false);
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForEndOfFrame();
    }
}
