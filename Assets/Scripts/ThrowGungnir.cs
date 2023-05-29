using System.Collections;
using UnityEngine;

public class ThrowGungnir : Ability, IUpgradeable
{
    [Header("Gungnir")]
    public GameObject gungnir;

    public IEnumerator gungnirCDCoroutine;
    public float CD;
    private Gungnir gungnirScript;
    [HideInInspector] public bool canThrowGungnir = true;
    private float gungnirCD;
    private WeaponCDs weaponCDVisual;
    private PlayerAction playerAction;
    public GameObject player;

    [Header("TriThrow Upgrade")]
    public bool hasUpgrade1;
    public GameObject triThrow;
    private TriThrow triThrowScript;

    [Header("Pin Upgrade")]
    public bool hasUpgrade2;

    [Header("SFX")]
    [Range(0, 1)] public float sfxVolume = 1f;
    [SerializeField] private AudioClip throwSFX;

    private Vector3 lastDirection = Vector3.right;

    // Start is called before the first frame update
    private void Awake()
    {
        weaponCDVisual = GameObject.Find("CDs").GetComponent<WeaponCDs>();
        playerAction = player.GetComponent<PlayerAction>();
        gungnirCDCoroutine = GungnirCD();
        gungnirScript = gungnir.GetComponent<Gungnir>();
        triThrowScript = triThrow.GetComponent<TriThrow>();
    }

    // StateUpdate is called once per frame
    private void Update()
    {
        StartThrow();
        gungnirCD = CD;

        Vector2 movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        Vector3 playerFacingDirection = playerAction.moveVector.normalized;

        if (playerFacingDirection.magnitude > 0f)
        {
            lastDirection = playerFacingDirection;
        }
    }

    public void ResetCD()
    {
        CD = 0;
        Debug.Log("RESET CD on GUNGNIR");
    }

    private void StartThrow()
    {
        if (Input.GetButtonDown("Fire3") && canThrowGungnir)
        {
            SFXManager.singleton.PlaySound(throwSFX, transform.position, sfxVolume);

            Gungnir spear = Instantiate(gungnirScript, playerAction.transform.position, Quaternion.identity);
            spear.SetDirection(playerAction.lastFacing);

            StartCoroutine("GungnirCD");
            Debug.Log("Threw gungnir");
            weaponCDVisual.StartCoroutine("GungnirCD");

           
            if (hasUpgrade1)
            {
                StartCoroutine(SFXManager.singleton.PlaySoundWithDelay(throwSFX, transform.position, sfxVolume, 0.05f));

                // Calculate angles for the additional spears
                float angle1 = GetThrowAngle() + 20f;
                float angle2 = GetThrowAngle() - 20f;

                // Instantiate and throw the additional spears
                GameObject spear1 = Instantiate(triThrow, playerAction.transform.position, Quaternion.identity);
                spear1.GetComponent<TriThrow>().damage = gungnir.GetComponent<Gungnir>().damage;
                spear1.transform.rotation = Quaternion.Euler(0f, 0f, angle1);
                spear1.GetComponent<Rigidbody2D>().AddForce(GetThrowDirection(angle1) * gungnirScript.speed, ForceMode2D.Impulse);

                GameObject spear2 = Instantiate(triThrow, playerAction.transform.position, Quaternion.identity);
                spear2.GetComponent<TriThrow>().damage = gungnir.GetComponent<Gungnir>().damage;
                spear2.transform.rotation = Quaternion.Euler(0f, 0f, angle2);
                spear2.GetComponent<Rigidbody2D>().AddForce(GetThrowDirection(angle2) * gungnirScript.speed, ForceMode2D.Impulse);
            }
        }
    }

    private float GetThrowAngle()
    {
        // Calculate the angle based on the player's facing direction
        // Calculate the angle based on the player's facing direction
        float angle = 0f;
        Vector3 playerFacingDirection = playerAction.moveVector.normalized;

        if (playerFacingDirection.magnitude > 0f)
        {
            playerFacingDirection = playerFacingDirection.normalized;
        }
        else
        {
            playerFacingDirection = lastDirection;
        }

        if (playerFacingDirection.x > 0f) // Facing right
        {
            if (playerFacingDirection.y > 0f) // Facing up-right
            {
                angle = 45f;
            }
            else if (playerFacingDirection.y < 0f) // Facing down-right
            {
                angle = -45f;
            }
            else // Facing right only
            {
                angle = 0f;
            }
        }
        else if (playerFacingDirection.x < 0f) // Facing left
        {
            if (playerFacingDirection.y > 0f) // Facing up-left
            {
                angle = 135f;
            }
            else if (playerFacingDirection.y < 0f) // Facing down-left
            {
                angle = -135f;
            }
            else // Facing left only
            {
                angle = 180f;
            }
        }
        else // Not moving horizontally (left/right)
        {
            if (playerFacingDirection.y > 0f) // Facing up only
            {
                angle = 90f;
            }
            else if (playerFacingDirection.y < 0f) // Facing down only
            {
                angle = 270f;
            }
        }

        Debug.Log("Player facing direction: " + playerFacingDirection);
        Debug.Log("Main spear direction: " + angle);

        return angle;
    }

    private Vector2 GetThrowDirection(float angle)
    {
        // Calculate the throw direction based on the specified angle
        float angleInRadians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }

    public IEnumerator GungnirCD()
    {
        canThrowGungnir = false;
        yield return new WaitForSeconds(gungnirCD);
        canThrowGungnir = true;
    }

    public void UpgradeOption1()
    {
        hasUpgrade1 = true;
        throw new System.NotImplementedException();
    }

    public void UpgradeOption2()
    {
        throw new System.NotImplementedException();
    }

    public void Downgrade()
    {
        hasUpgrade1 = false;
        throw new System.NotImplementedException();
    }

    //public void ThrowTriSpear()
    //{
    //    TriThrow triSpearPref = Instantiate(triThrowScript, playerAction.transform.position, Quaternion.identity);
    //    triSpearPref.SetDirection(playerAction.lastFacing, -45f);
    //    TriThrow triSpearPref2 = Instantiate(triThrowScript, playerAction.transform.position, Quaternion.identity);
    //    triSpearPref2.SetDirection(playerAction.lastFacing, 45f);

    //    Vector2 lastFacing = playerAction.lastFacing;

    //    //triSpearPref.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(lastFacing.y, lastFacing.x) * Mathf.Rad2Deg + triSpearPref.angleOfSpear);
    //    //triSpearPref2.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(lastFacing.y, lastFacing.x) * Mathf.Rad2Deg + triSpearPref2.angleOfSpear);

    //    // Set the spear's velocity to make it move in the desired direction
    //    //triSpearPref.GetComponent<Rigidbody2D>().velocity = playerAction.lastFacing.normalized * triThrowScript.speed;
    //}
}