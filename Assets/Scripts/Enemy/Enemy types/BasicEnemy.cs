using System;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public enum State
    {
        Idle,
        Attack,
        AttackCooldown
    }
    public State currentState = State.Idle;


    [Header("AI params")]
    public float detectRadius = 8f;
    public float wanderRadius = 6f;
    public float arriveRadius = 1f; // for arrival slowing
    public float attackCooldownTime = 1.5f;
    public float attackRange = 1.2f; // close enough to "hit"
    public float attackPushBack = 3f;

    Transform player;
    Vector3 wanderTarget;
    float stateTimer = 0f;

    private Vector3 homePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homePosition = transform.position;
        PickNewWanderTarget();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.AttackCooldown:
                UpdateCoolDown();
                break;
        }
    }

    private void UpdateAttack()
    {
       Debug.Log("Attack state not implemented yet");
        
        if (player != null && Vector3.Distance(player.position, transform.position) >= detectRadius)
        {
            currentState = State.Attack;
            return;
        }
    }

    private void UpdateCoolDown()
    {
        Debug.Log("Cooldown state not implemented yet");
    }

    private void UpdateIdle()
    {
        Debug.Log("Idle state");
        // wander: seek small random target, pick new once close
        Vector3 toTarget = wanderTarget - transform.position;
        toTarget.y = 0;
        Vector3 desired = Vector3.zero;
        float dist = toTarget.magnitude;
        if (dist < 0.5f) PickNewWanderTarget();
        else
        {
            // arrive behavior to avoid instantaneous stops
            float speed = horizontalSpeed * (dist > arriveRadius ? 1f : dist / arriveRadius);
            desired = toTarget.normalized * speed;
        }


        // transition to Attack
        if (player != null && Vector3.Distance(player.position, transform.position) <= detectRadius)
        {
            currentState = State.Attack;
            return;
        }


        MoveSimple(desired);
    }
    void PickNewWanderTarget()
    {
        Vector2 r = UnityEngine.Random.insideUnitCircle * wanderRadius;
        wanderTarget = homePosition + new Vector3(r.x, 0, r.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
