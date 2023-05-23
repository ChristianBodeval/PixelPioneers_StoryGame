using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//TODO Make private coroutines for each state, so that there can only be 1 of each running at a time
public enum State
{
    Idle,
    Patrol,
    Searching,
    Chase,
    Attack,
    Stunned,
    Digging,
    Dead
};

public class WormFSM : MonoBehaviour
{
    //States
    private IdleState idleState;
    private PatrolState patrolState;
    private SearchingState searchingState;
    private ChaseState chaseState;
    private AttackState attackState;
    private StunnedState stunnedState;
    [HideInInspector]  public DiggingState diggingState;
    private DeadState deadState;
    
    private EnemyState currentState;

    [Header("General")]
    public GameObject player;
    public float speed;
    [HideInInspector] public GeneralPathing aStarPathing;
    [HideInInspector] public Health healthScript;

    // and the variable holding the current state
    [HideInInspector] public Rigidbody rb;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    
    [Header("ChaseState")]
    public float sightRange;
    
    [Header("PatrolState")]
    public float patrolTime;
    public float patrolRadius = 5f;

    
    [Header("AttackState")]
    public float attackRange;
    public float attackTime;
    public float attackDamage;
    
    [Header("StunState")]
    public float stunTime;
    
    [Header("DiggingState")]
    public float diggingCooldown;
    public GameObject diggingEffect;
    public GameObject sprite;
    public float diggingSpeed;
    
    [Header("Icon & Colors")]
    public SpriteRenderer icon;
    public Color cluelessColor;
    public Color suspiciousColor;
    public Color chaseColor;

    [Header("SFX")]
    [Range(0f, 1f)] public float volume;
    public AudioClip diggingSFX;
    public GameObject obj;

    // Start is called before the first frame update



    void Start()
    {
        //Initialize all the states

        player = GameObject.Find("Player");
        idleState = this.gameObject.AddComponent<IdleState>();
        patrolState = this.gameObject.AddComponent<PatrolState>();
        searchingState = this.gameObject.AddComponent<SearchingState>();
        chaseState = this.gameObject.AddComponent<ChaseState>();
        attackState = this.gameObject.AddComponent<AttackState>();
        stunnedState = this.gameObject.AddComponent<StunnedState>();
        diggingState = this.gameObject.AddComponent<DiggingState>();
        deadState = this.gameObject.AddComponent<DeadState>();
        
        aStarPathing = GetComponent<GeneralPathing>();

        healthScript = GetComponent<Health>();
        
        rb = GetComponent<Rigidbody>();
        //set up the starting state
        ChangeState(State.Idle);
        
        healthScript.DamageTakenEvent.AddListener(OnDamageTaken);
        healthScript.Dead.AddListener(OnDamageTaken);
        aStarPathing.SetSpeed(speed); 
    }
   void Update()
    {
        if (currentState == null)
            return;
        
        currentState.StateUpdate(this);
        
        //Log currentState
        Debug.Log("Current state: " + currentState);
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
            case State.Searching:
                currentState = searchingState;
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
    public abstract void StateUpdate(WormFSM enemy);
}

public class IdleState : EnemyState
{
    
    /*private void DestinationReached()
    {
        
        Debug.Log("Destination reached");
        ChangeState(State.Patrol);
    }*/
    public override void Enter(WormFSM enemy)
    {
        //enemy.aStarPathing.OnDirectionReached.AddListener(DestinationReached);
        //Change state to patrol after 3 seconds
        enemy.icon.color = enemy.cluelessColor;
        
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            enemy.ChangeState(State.Patrol);
        }, enemy.patrolTime));
    }

    public override void Exit(WormFSM enemy)
    {
        StopAllCoroutines();
    }
    
    // Check if player is in sightRange and the view is not blocked by an obstacle
    public override void StateUpdate(WormFSM enemy)
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
        
        enemy.aStarPathing.SetDirection(HelperMethods.GetRandomPointInRadiusOnGround(enemy.patrolRadius, enemy.transform.position));
    }


    

    public override void Exit(WormFSM enemy)
    {
        //Don't move
        enemy.aStarPathing.StopMoving();
        StopAllCoroutines();

    }

    public override void StateUpdate(WormFSM enemy)
    {
        //If player is in sightRange and no obstacle is blocking the view, then change state to chase
        if (enemy.aStarPathing.GetDistanceToPlayer() < enemy.sightRange && !enemy.aStarPathing.IsObstacleBetweenPlayer())
        {
            enemy.ChangeState(State.Chase);
        }
    }
}

public class SearchingState : EnemyState
{
    Vector3 currentPlayerPosition;
    public override void Enter(WormFSM enemy)
    {
        enemy.icon.color = enemy.suspiciousColor;
        
        currentPlayerPosition = enemy.player.transform.position;
        enemy.aStarPathing.SetDirection(currentPlayerPosition);
    }

    public override void Exit(WormFSM enemy)
    {
    }

    public override void StateUpdate(WormFSM enemy)
    {
        if (enemy.aStarPathing.GetDistanceToPlayer() < enemy.sightRange && !enemy.aStarPathing.IsObstacleBetweenPlayer())
        {
            enemy.ChangeState(State.Chase);
        }
        
        else if (Vector2.Distance(enemy.transform.position, currentPlayerPosition) < 0.1f)
        {
            enemy.ChangeState(State.Idle);
        }
    }
}

public class ChaseState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        //Move towards player
        enemy.icon.color = enemy.chaseColor;
    }

    public override void Exit(WormFSM enemy)
    {
    }

    public override void StateUpdate(WormFSM enemy)
    {
        
        //If player is in attackRange, and no obstacle is blocking the view, then change state to attack
        if (enemy.aStarPathing.GetDistanceToPlayer() < enemy.attackRange && !enemy.aStarPathing.IsObstacleBetweenPlayer())
        {
            enemy.ChangeState(State.Attack);
        }
        
        if (enemy.aStarPathing.IsObstacleBetweenPlayer() || enemy.aStarPathing.GetDistanceToPlayer() > enemy.sightRange)
        {
            enemy.ChangeState(State.Searching);
        }
        enemy.aStarPathing.SetDirection(enemy.player.transform.position);
    }
}

public class AttackState : EnemyState
{
    private IEnumerator attackCoroutine;
    public override void Enter(WormFSM enemy)
    {
        bool isOnCooldown = false;
        bool hasAttacked = false;
        
        //Initialize coroutine
        
        attackCoroutine = enemy.InvokeAfterDelay(() =>
        {
            //Damage the player
            enemy.player.GetComponent<PlayerHealth>().TakeDamage(enemy.attackDamage);
            hasAttacked = true;
        }, enemy.attackTime);

        
        if (!isOnCooldown)
        {
            StartCoroutine(attackCoroutine);
        }
        else
        {
            enemy.ChangeState(State.Chase);
        }
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

    public override void StateUpdate(WormFSM enemy)
    {
        return;
    }
}

public class StunnedState : EnemyState
{
    public override void Enter(WormFSM enemy)
    {
        enemy.aStarPathing.StopMoving();

        //Change state to chase after x seconds
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            enemy.ChangeState(State.Digging);
        }, enemy.stunTime));
    }

    public override void Exit(WormFSM enemy)
    {
        return;
    }

    public override void StateUpdate(WormFSM enemy)
    {
        //Call log on pressing space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
        }
    }
}

public class DiggingState : EnemyState
{
    [Header("Digging")]
    public bool isOnCooldown;
    public bool isDigging;

    //Initialize digging cooldown coroutine
    private IEnumerator diggingCooldownCoroutine;
    private Vector3 currentPlayerPosition;
    //Write cooldown function with IEnumerator
    public override void Enter(WormFSM enemy)
    {
        diggingCooldownCoroutine = enemy.InvokeAfterDelay(() =>
        {
            isOnCooldown = false;
        }, enemy.diggingCooldown);

        if (!isOnCooldown)
        {
            WormFSM temp = gameObject.GetComponent<WormFSM>();
            temp.obj = SFXManager.singleton.PlayLoop(temp.diggingSFX, transform.position, temp.volume, true, transform);

            //Start cooldown
            isOnCooldown = true;
            isDigging = true;
            
            enemy.aStarPathing.SetSpeed(enemy.diggingSpeed);
            enemy.diggingEffect.SetActive(true);
            enemy.sprite.SetActive(false);
            enemy.healthScript.SetCanTakeDamage(false);
            
            currentPlayerPosition = enemy.player.transform.position;
            
            //Add the players velocity to the direction of the digging
            currentPlayerPosition += (Vector3)enemy.player.GetComponent<Rigidbody2D>().velocity.normalized * 3f;
            
            enemy.aStarPathing.SetDirection(currentPlayerPosition);

            StartCoroutine(diggingCooldownCoroutine);
        }
        else
        {
            enemy.ChangeState(State.Chase);
        }

        Pool.pool.ReturnToSFXPool(gameObject.GetComponent<WormFSM>().obj);
    }
    
    public override void Exit(WormFSM enemy)
    {
        enemy.diggingEffect.SetActive(false);
        enemy.sprite.SetActive(true);
        enemy.healthScript.SetCanTakeDamage(true);
        enemy.aStarPathing.SetSpeed(enemy.speed);
        isDigging = false;
    }

    public override void StateUpdate(WormFSM enemy)
    {
        //If the currentPlayerPosition is reached, then change state to chase and reset cooldown
        if (Vector2.Distance(enemy.transform.position, currentPlayerPosition) < 0.1f)
        {
            enemy.ChangeState(State.Chase);
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

    public override void StateUpdate(WormFSM enemy)
    {
        return;
    }
}


