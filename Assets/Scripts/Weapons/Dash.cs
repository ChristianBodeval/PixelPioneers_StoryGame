using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability, IUpgradeable
{
    [Header("Dash")]
    public float dashDistance = 5f; // Distance of the dash

    public float dashDuration = 0.5f; // Duration of the dash
    [HideInInspector] public float dashCooldownTime = 1.8f;
    [HideInInspector] public bool isDashing = false; // Flag to check if the player is currently dashing
    private float dashTime = 0f; // Time elapsed during the dash
    private Vector2 dashDirection; // Direction of the dash
    private bool canDash = true; //C Flag to check if the player can dash
    [HideInInspector] public float dashCooldownRemaining = 0f; // Initialize to 0 to allow dashing immediately

    private Rigidbody2D playerRb;
    private PlayerAction player;
    private WeaponCDs weaponCDs;
    private GameObject playerGO;

    private FireDashSpawn fireSpawn;
    public bool hasUpgrade1;

    // Start is called before the first frame update
    private void Start()
    {
        dashDirection = GetDashDirection();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerAction>();
        weaponCDs = GameObject.Find("WeaponCDs").GetComponent<WeaponCDs>();
        playerGO = GameObject.FindGameObjectWithTag("Player");
        fireSpawn = GetComponent<FireDashSpawn>();
    }

    // Update is called once per frame
    private void Update()
    {
        StartDash();
    }

    private void FixedUpdate()
    {
        // Check if the player is dashing
        if (isDashing)
        {
            // If the dash duration has not elapsed, move the player in the dash direction
            if (dashTime < dashDuration)
            {
                Camera.main.GetComponent<CameraScript>().StartLagBehindPlayer();
                playerRb.MovePosition(playerRb.position + dashDirection * dashDistance / dashDuration * Time.fixedDeltaTime);
                dashTime += Time.fixedDeltaTime;
                if (hasUpgrade1)
                {
                    
                fireSpawn.StartCoroutine("SpawnFire");
                }
            }
            // Otherwise, end the dash
            else
            {
                Camera.main.GetComponent<CameraScript>().StopLagBehindPlayer();
                EndDash();
            }
        }
    }

    public void CanDash()
    {
        canDash = true;
    }

    public void CannotDash()
    {
        canDash = false;
    }

    private IEnumerator DashCD()
    {
        canDash = false; // Set the player to be unable to dash
        dashCooldownRemaining = dashCooldownTime; // Reset the remaining cooldown time
        while (dashCooldownRemaining > 0f) // Count down the cooldown time
        {
            dashCooldownRemaining -= Time.deltaTime;
            yield return null; // Wait for the end of the frame
        }
        canDash = true; // Set the player to be able to dash again
    }

    private void EndDash()
    {
        isDashing = false;
        playerGO.GetComponent<PlayerHealth>().RemoveInvulnerability(); // I frames
    }

    private Vector2 GetDashDirection()
    {
        // Get the direction the player is facing
        float angle = transform.eulerAngles.y;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        return direction.normalized;
    }

    private void StartDash()
    {
        // Check if the player is not currently dashing and if they can dash
        if (!isDashing && Input.GetButtonDown("Dash") && canDash) // Dash is on 'space'
        {
            playerGO.GetComponent<PlayerHealth>().AddInvulnerability();

            // Set the player to dashing state
            isDashing = true;
            dashTime = 0f;

            // Set the dash direction to the last facing direction of the player
            dashDirection = player.lastFacing;

            // Start the dash cooldown coroutine
            weaponCDs.StartCoroutine("DashCD");
            StartCoroutine(DashCD());
        }
    }

    public void UpgradeOption1()
    {
        hasUpgrade1 = true;   
    }

    public void UpgradeOption2()
    {
        throw new System.NotImplementedException();
    }

    public void Downgrade()
    {
        throw new System.NotImplementedException();
    }
}