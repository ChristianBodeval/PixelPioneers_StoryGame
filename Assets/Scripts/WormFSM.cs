using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public enum State
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Stunned,
    Digging,
    Dead
};

public class WormFSM : MonoBehaviour
{
    //Intialise all the states of EnemyState
    private IdleState idleState;
    private PatrolState patrolState;
    private ChaseState chaseState;
    private AttackState attackState;
    private StunnedState stunnedState;
    [HideInInspector]  public DiggingState diggingState;
    private DeadState deadState;
    
    // and the variable holding the current state
    private EnemyState currentState;
    [HideInInspector] public Rigidbody rb;
    public LayerMask groundLayer;
    public float patrolTime;

    public float patrolRadius = 5f;
    [HideInInspector] public GeneralPathing aStarPathing;
    public float sightRange;
    public float attackRange;
    public float attackDamage;
    
    public GameObject player;
    public float diggingCooldown;
    public float stunTime;
    public float attackTime;


    
    
    //Make a similar function as GetRandomPointInLayerMask, but get a random point from the camera's view
    
    
    
    public Vector3 GetRandomPointInTriggerRadius(GameObject gameObject, LayerMask layerMask, float radius)
    {
        // Get all trigger colliders in a sphere around the gameobject
        Collider[] triggerColliders = Physics.OverlapSphere(gameObject.transform.position, radius, layerMask, QueryTriggerInteraction.Collide);

        // Keep trying to find a point until it's inside a trigger collider
        bool foundValidPoint = false;
        Vector3 point = Vector3.zero;
        while (!foundValidPoint)
        {
            // Get a random point within the sphere and multiply it by the radius
            point = Random.insideUnitSphere * radius;

            // Check if the point is inside any of the trigger colliders
            int overlaps = Physics.OverlapSphereNonAlloc(gameObject.transform.position + point, 0.1f, triggerColliders, layerMask, QueryTriggerInteraction.Collide);
            foundValidPoint = (overlaps > 0);
        }

        Debug.Log("Random point: " + (transform.position + point));
        //Draw the transform.position + point in the scene view
        Debug.DrawLine(transform.position, transform.position + point, Color.red, 5f);

        
        return gameObject.transform.position + point;
    }
    
    //Call GetRandomPointInCameraView when pressing space
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetRandomPointInTriggerRadius(this.gameObject, groundLayer, patrolRadius);
        }
    }
    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize all the states
        idleState = this.gameObject.AddComponent<IdleState>();
        patrolState = this.gameObject.AddComponent<PatrolState>();
        chaseState = this.gameObject.AddComponent<ChaseState>();
        attackState = this.gameObject.AddComponent<AttackState>();
        stunnedState = this.gameObject.AddComponent<StunnedState>();
        diggingState = this.gameObject.AddComponent<DiggingState>();
        deadState = this.gameObject.AddComponent<DeadState>();
        
        aStarPathing = GetComponent<GeneralPathing>();

        rb = GetComponent<Rigidbody>();
        //set up the starting state
        ChangeState(State.Idle);
        
        this.GetComponent<Health>().DamageTakenEvent.AddListener(OnDamageTaken);
        this.GetComponent<Health>().Dead.AddListener(OnDamageTaken);
    }
   void Update()
    {
        currentState.Update(this);
        
        
    }
   
   //Make function that subscribes to an event from the playerHealth script and is called when the player takes damage
    public void OnDamageTaken()
    {
         ChangeState(State.Stunned);
    }
    public void OnDead()
    {
        ChangeState(State.Dead);
    }
   
   public IEnumerator InvokeAfterDelay(Action action, float delay)
   {
       yield return new WaitForSeconds(delay);
       action();
   }

    public void ChangeState(State state)
    {
        //call exit behaviour of current state
        if (currentState != null)
            currentState.Exit(this);
        //Add all the states to the switch statement
        switch (state)
        {
            case State.Idle:
                currentState = idleState;
                break;
            case State.Patrol:
                currentState = patrolState;
                break;
            case State.Chase:
                currentState = chaseState;
                break;
            case State.Attack:
                currentState = attackState;
                break;
            case State.Stunned:
                currentState = stunnedState;
                break;
            case State.Digging:
                currentState = diggingState;
                break;
            case State.Dead:
                currentState = deadState;
                break;
            
        }
        
        //Debug that the state has changed to x state
        Debug.Log("Changed state to " + state);
        currentState.Enter(this);
    }
}
public abstract class EnemyState : MonoBehaviour
{
    public abstract void Enter(WormFSM enemy);
    public abstract void Exit(WormFSM enemy);
    public abstract void Update(WormFSM enemy);
}

public class IdleState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        //Change state to patrol after 3 seconds
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            enemy.ChangeState(State.Patrol);
        }, enemy.patrolTime));
    }

    public override void Exit(WormFSM enemy)
    {
        return;
    }
    
    // Check if player is in sightRange and the view is not blocked by an obstacle
    public override void Update(WormFSM enemy)
    {
        //If player is in sightRange
        if (enemy.aStarPathing.GetDistanceToPlayer() < enemy.sightRange && !enemy.aStarPathing.IsObstacleBetweenPlayer())
        {
            enemy.ChangeState(State.Chase);
        }
    }
}

public class PatrolState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        // Change state to idle after 3 seconds
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            enemy.ChangeState(State.Idle);
        }, enemy.patrolTime));
        
        //Move to random point in layer mask in patrol radius
        //enemy.aStarPathing.SetDirection(enemy.GetRandomPointInLayerMask(enemy.groundLayer, enemy.patrolRadius));
    }


    

    public override void Exit(WormFSM enemy)
    {
        //Don't move
        enemy.aStarPathing.SetDirection(transform.position);
    }

    public override void Update(WormFSM enemy)
    {
        //If player is in sightRange and no obstacle is blocking the view, then change state to chase
        if (enemy.aStarPathing.GetDistanceToPlayer() < enemy.sightRange && !enemy.aStarPathing.IsObstacleBetweenPlayer())
        {
            enemy.ChangeState(State.Chase);
        }
    }
}

public class ChaseState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        return;
    }

    public override void Exit(WormFSM enemy)
    {
        return;
    }

    public override void Update(WormFSM enemy)
    {
        //If player is in attackRange, and no obstacle is blocking the view, then change state to attack
        if (enemy.aStarPathing.GetDistanceToPlayer() < enemy.attackRange && !enemy.aStarPathing.IsObstacleBetweenPlayer())
        {
            enemy.ChangeState(State.Attack);
        }
    }
}

public class AttackState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        //Damage the player
        enemy.player.GetComponent<PlayerHealth>().TakeDamage(enemy.attackDamage);
        
        //Change state to chase after x seconds
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            enemy.ChangeState(State.Chase);
        }, enemy.attackTime));
        
    }

    //Draw a circle in game around the enemy to show the sightrange
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetComponent<WormFSM>().sightRange);
    }
    
    public override void Exit(WormFSM enemy)
    {
        return;
    }

    public override void Update(WormFSM enemy)
    {
        return;
    }
}

public class StunnedState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        //Change state to chase after x seconds
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            enemy.ChangeState(State.Chase);
        }, enemy.stunTime));
    }

    public override void Exit(WormFSM enemy)
    {
        return;
    }

    public override void Update(WormFSM enemy)
    {
        return;
    }
}

public class DiggingState : EnemyState
{
    public bool isOnCooldown;
    public bool isDigging;


    private Vector3 currentPlayerPosition;
    //Write cooldown function with IEnumerator
    public override void Enter(WormFSM enemy)
    {
        if (!isOnCooldown)
        {
            //Start cooldown
            isOnCooldown = true;
            isDigging = true;
            
            currentPlayerPosition = enemy.player.transform.position;
            enemy.aStarPathing.SetDirection(currentPlayerPosition);

            //Disable collider & set to isKinematic
            enemy.GetComponent<Collider2D>().enabled = false;
            enemy.GetComponent<Rigidbody2D>().isKinematic = true;
            
            
            StartCoroutine(enemy.InvokeAfterDelay(() =>
            {
                isOnCooldown = false;
            }, enemy.diggingCooldown));
            
            
        }
        else
        {
            enemy.ChangeState(State.Chase);
        }

    }
    
    public override void Exit(WormFSM enemy)
    {
        return;
    }

    public override void Update(WormFSM enemy)
    {
        //When the digging is finished
        if(isDigging && currentPlayerPosition == transform.position)
        {
            enemy.ChangeState(State.Chase);
            isDigging = false;
        }
    }
}

public class DeadState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        return;
    }

    public override void Exit(WormFSM enemy)
    {
        return;
    }

    public override void Update(WormFSM enemy)
    {
        return;
    }
}


