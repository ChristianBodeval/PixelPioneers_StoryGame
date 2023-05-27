using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Music")]
    [Range(0, 1)] public float musicVolume = 0.5f;
    [SerializeField] protected AudioClip casualTrack;

    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] protected AudioClip deathSFX;
    [SerializeField] protected AudioClip bossBloodSFX;

    public float currentHealth;
    public float maxHealth;
    [SerializeField] private Shader dissolve;
    [SerializeField] protected float deathAnimDuration;
    protected Coroutine deathCoroutine;
    protected Coroutine blinkCoroutine;
    protected Coroutine shakeCoroutine;
    protected static readonly float freezeDurationOnDmgTaken = 0.15f;
    private bool isBlinking = false;
    public bool canTakeDamage = true;

    [SerializeField] protected SpriteRenderer sr;
    public UnityEvent DamageTakenEvent;
    public UnityEvent Dead;

    // Constructor
    public Health(float health, float maxHealth)
    {
        this.currentHealth = health;
        this.maxHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if(this.isActiveAndEnabled)
        {
            if (!canTakeDamage) return;
            
            this.currentHealth -= damage;

            if (this.currentHealth <= 0) return;

            // Hermes moves when haven taken damage
            if (gameObject.CompareTag("Boss"))
            {
                GetComponent<Hermes_Pathing>().MoveOnHitTaken();

                if (Random.Range(0,2) == 0)
                {
                    SFXManager.singleton.PlaySound(bossBloodSFX, transform.position, sfxVolume);

                    // Create blood and pickup
                    HealthPickUp.pickUpPool.AddHealthPickUp(transform.position, 1f); // Spawn health pickup
                    GameObject blood = Pool.pool.DrawFromBloodPool();
                    blood.transform.position = transform.position;
                    blood.transform.Rotate(new Vector3(0f, 0f, Random.Range(0, 4) * 90f)); // Random rotation
                    float size = Random.Range(2f, 3f);
                    blood.transform.localScale = new Vector3(size, size, 1f);
                }
            }

            // Freeze frame enemies
            if (gameObject.CompareTag("Enemy")) GetComponent<Crowd_Control>().FreezeFrame(freezeDurationOnDmgTaken);

            // Spawn blood
            GameObject bloodSplatter = Pool.pool.DrawFromBloodSpatterPool();
            bloodSplatter.transform.position = transform.position;

            // Blink white
            if (blinkCoroutine != null || isBlinking) StopCoroutine(blinkCoroutine); // Stops blink coroutine
            blinkCoroutine = StartCoroutine(BlinkOnDmgTaken(freezeDurationOnDmgTaken));

            // Shake sprite
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(SpriteShake(0.12f, freezeDurationOnDmgTaken));
            //PrintDmgToScreen(damage, Color.red);

            DamageTakenEvent.Invoke();
        }
    }

    
    public virtual void HealDamage(float heal)
    {
        if (currentHealth + heal > maxHealth)
        {
            currentHealth = maxHealth;
            return;
        }

        currentHealth += heal;

        //TODO Add this as a listener to the DamageTakenEvent
        PrintDmgToScreen(heal, Color.green);
    }

    // Changes material and color for a duration
    public virtual IEnumerator BlinkOnDmgTaken(float duration = 0.15f)
    {
        if (deathCoroutine != null) yield break;
        isBlinking = true;

        Material blinkMat = Instantiate(MaterialManager.singleton.blinkMaterial);
        sr.material = blinkMat;
        sr.material.color = Color.white;

        yield return new WaitForSeconds(duration);

        sr.material = MaterialManager.singleton.baseMaterial;
        sr.material.color = Color.white;
        isBlinking = false;
    }

    // Removes enemy from active pool, plays death anim and spawns pickup
    public virtual IEnumerator Die()
    {
        if (gameObject.CompareTag("Enemy"))
        {

            SFXManager.singleton.PlaySound(deathSFX, transform.position);

            Dead.Invoke();
            // Create blood and pickup
            HealthPickUp.pickUpPool.AddHealthPickUp(transform.position, maxHealth / 8); // Spawn health pickup
            GameObject blood = Pool.pool.DrawFromBloodPool();
            blood.transform.position = transform.position;
            blood.transform.Rotate(new Vector3(0f, 0f, Random.Range(0, 4) * 90f)); // Random rotation
            float size = Random.Range(0.8f, 1.2f);
            blood.transform.localScale = new Vector3(size, size, 1f);

            // Stop movement
            gameObject.GetComponent<Crowd_Control>().Stun();
            GetComponentInChildren<Animator>().SetBool("CanMove", false);
            gameObject.GetComponent<Collider2D>().enabled = false;

            // Stop animation and play dissipation shader
            GetComponentInChildren<Animator>().speed = 0f;

            // Set material
            if (blinkCoroutine != null || isBlinking) StopCoroutine(blinkCoroutine); // Stops blink coroutine
            Material deathMat = Instantiate(MaterialManager.singleton.deathMaterial);
            sr.material = deathMat;
            sr.material.color = Color.white;
            
            SpriteRenderer srShadow = null;
            
            if(GetComponentInChildren<UnityEngine.Rendering.Universal.ShadowCaster2D>().GetComponent<SpriteRenderer>() != null)
                srShadow = GetComponentInChildren<UnityEngine.Rendering.Universal.ShadowCaster2D>().GetComponent<SpriteRenderer>();

            float timeStep = deathAnimDuration / 6;
            float t = 1f;
            float alpha = srShadow.color.a;

            // Play dissipation shader
            if (MaterialHasShader(sr.material, dissolve))
            {
                while (sr.material.GetFloat("_FadeTime") > 0f)
                {
                    t -= timeStep;
                    alpha -= timeStep;
                    srShadow.color = new Color(srShadow.color.r, srShadow.color.g, srShadow.color.b, alpha);
                    sr.material.SetFloat("_FadeTime", t);
                    yield return new WaitForSeconds(timeStep);
                }
            }

            //TESTING if it works from waves
            
            //TimelineManager timelineManager = TimelineManager.timelineManager;
            //if (timelineManager.tutorialIsStarted && timelineManager.T1Done && timelineManager.T2Done && !timelineManager.T3Done && !timelineManager.T4Done && !timelineManager.T5Done)
            //{
            //    timelineManager.ResumeTL();
            //}

            // Deactivate enemy and return to pool
            GameObject.Find("GameManager").GetComponent<SpawnSystem>().RemoveFromWaitDeathList(gameObject);
            Pool.pool.ReturnToEnemyPool(gameObject);

        }
        else if (gameObject.CompareTag("Boss"))
        {
            if (GetComponent<WeaponAbility>().bossHealthBar != null) GetComponent<WeaponAbility>().bossHealthBar.SetActive(false);
            gameObject.SetActive(false);
            Destroy(GameObject.Find("Parent_Mjoelnir(Clone)"));
            MusicManager.singleton.PlayMusic(casualTrack, musicVolume);
            // TODO Trigger cutscene 
        }

       
    }

    public void SetCanTakeDamage(bool b)
    {
        canTakeDamage = b;
    }

    public bool MaterialHasShader(Material material, Shader desiredShader)
    {
        if (material != null && desiredShader != null)
        {
            return material.shader == desiredShader;
        }

        return false;
    }

    private void PrintDmgToScreen(float number, Color color)
    {
        //**
    }

    protected IEnumerator SpriteShake(float magnitude, float duration)
    {
        int iterations = 8;
        float timeBetweenShakes = duration / iterations;
        float magnitudeReductionOverDuration = magnitude / iterations;
        GameObject spriteObject = GetComponentInChildren<SpriteRenderer>().gameObject;
        duration += Time.time;

        // Shake to either left or right first, at random
        Vector3 dir = Random.Range(0, 2) == 0 ? Vector3.left : Vector3.right; // Ternary operator '?' is just a short if statement - outputs left or right vector

        while (duration > Time.time)
        {
            spriteObject.transform.localPosition = dir * magnitude; // Displace sprite
            dir = dir * -1f; // Swap direction
            magnitude -= magnitudeReductionOverDuration; // Reduce shake over the duration
            yield return new WaitForSeconds(timeBetweenShakes);
        }

        spriteObject.transform.localPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        canTakeDamage = true;
        this.currentHealth = maxHealth;

        if (sr == null) return;
        sr.material = MaterialManager.singleton.baseMaterial;
        sr.color = Color.white;
    }

    private void OnDisable()
    {
        Mjoelnir.cannotHitList.Remove(gameObject); // Remove this enemy from the list
        if (deathCoroutine != null) StopCoroutine(deathCoroutine);
        deathCoroutine = null;
        gameObject.GetComponent<Collider2D>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().gameObject.transform.localPosition = Vector3.zero; // Resets sprite position
        if (CompareTag("Enemy") && GetComponentInChildren<Animator>() != null) GetComponentInChildren<Animator>().speed = 1f;
    }

    protected virtual void Update()
    {
        if (this.currentHealth <= 0 && deathCoroutine == null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            deathCoroutine = StartCoroutine(Die());
        }
    }
}
