using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveVector = new Vector2(1f, 0f);
    Rigidbody2D rb;
    public float speed = 5;
    public Transform cone;
    private Vector2 lastFacing = new Vector2();
    private float coneRange = 2f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      Move();
      Facing();
    }
    private void Move()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical") * 0.5f;
        moveVector = moveVector.normalized; //as to not have faster movement when going diagonal

        //horizontal movement
        if (moveVector.x != 0)
        {
            rb.velocity = new Vector3(moveVector.x * speed, rb.velocity.y, 0f);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); //stops the player
        }

        //vertical movement
        if (moveVector.y != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, moveVector.y * speed, 0f);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f); //stops the player
        }
    }
    private void Facing()
    {
        //save where the player has been looking
        if (moveVector.magnitude > 0.5f)
        {
            lastFacing = new Vector2(moveVector.x, moveVector.y);
        }

        //face a cone in the direction of attack
        cone.eulerAngles = new Vector3(0f, 0f, Quaternion.LookRotation(lastFacing, Vector3.up).eulerAngles.x);
        cone.position = (Vector2)transform.position + lastFacing.normalized * coneRange;
    }
}
