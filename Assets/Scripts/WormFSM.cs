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
    private IdleState idleState = new IdleState();
    private PatrolState patrolState = new PatrolState();
    private ChaseState chaseState = new ChaseState();
    private AttackState attackState = new AttackState();
    private StunnedState stunnedState = new StunnedState();
    protected DiggingState diggingState = new DiggingState();
    private DeadState deadState = new DeadState();
    
    // and the variable holding the current state
    private EnemyState currentState;
    public Rigidbody rb;
    public LayerMask groundLayer;
    public float patrolTime;

    public float patrolRadius = 5f;
    
    public GeneralPathing aStarPathing;
    public float sightRange;
    public float attackRange;
    public float attackDamage;
    
    public UnityEvent OnPlayerHit;
    
    public GameObject player;
    public Vector3 GetRandomPointInLayerMask(LayerMask collisionLayer, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, collisionLayer);
        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found in layer " + collisionLayer.value);
            return Vector3.zero;
        }

        Collider randomCollider = colliders[Random.Range(0, colliders.Length)];
        Vector3 randomPoint = randomCollider.ClosestPoint(transform.position + Random.insideUnitSphere * randomCollider.bounds.extents.magnitude);

        return randomPoint;
    }
    
    // Start is called before the first frame update
    void Start()
    {
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
        enemy.aStarPathing.SetDirection(enemy.GetRandomPointInLayerMask(enemy.groundLayer, enemy.patrolRadius));
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
        enemy.ChangeState(State.Chase);
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
        //If the DiggingState is not on cooldown, then change state to Digging. Otherwise change state to Chase. Both should be called after the stun duration is finished
        if (enemy.diggingState.isOnCooldown <= 0)
        {
            enemy.ChangeState(State.Digging);
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
        return;
    }
}

public class DiggingState : EnemyState
{
    public bool isOnCooldown;
    public IEnumerator
    //Write cooldown function with IEnumerator
    public override void Enter(WormFSM enemy)
    {
        //Start cooldown
        StartCoroutine(enemy.InvokeAfterDelay(() =>
        {
            isOnCooldown = false;
        }, enemy.diggingCooldown));
        
        //Dig
        enemy.aStarPathing.SetDirection(enemy.player.transform.position);
    }
    
    
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


