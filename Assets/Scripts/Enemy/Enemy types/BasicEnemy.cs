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
    public float wanderRadius = 20f;
    public float arriveRadius = 1f; // for arrival slowing
    public float attackCooldownTime = 1.5f;
    public float attackRange = 3f; // close enough to "hit"
    public float attackPushBack = 3f;

    Transform player;
    PlayerController playerController;
    Vector3 wanderTarget;
    float stateTimer = 0f;

    private Vector3 homePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homePosition = transform.position;
        PickNewWanderTarget();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
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
      
        var toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        Vector3 desired = Vector3.zero;
        float dist = toPlayer.magnitude;
        if(dist > attackRange)
        {
            // move toward player
            desired = toPlayer.normalized * horizontalSpeed;
            MoveSimple(desired);
        }
        else
        { 
           
            // push player back
            Vector3 pushDir = toPlayer.normalized;
           
            playerController.TakeDamage(1);
            playerController.AddKnockBack(pushDir * attackPushBack);
            // enter cooldown
            currentState = State.AttackCooldown;
            stateTimer = attackCooldownTime;
            return;
        }
        if (player != null && Vector3.Distance(player.position, transform.position) >= detectRadius)
        {
            currentState = State.Idle;
            return;
        }

       
    }

    private void UpdateCoolDown()
    {
       
        if(stateTimer > 0f)
        {
            stateTimer -= Time.deltaTime;
        }
        else
        {
            // return to Attack state to check distance again
            currentState = State.Attack;
        }
    }

    private void UpdateIdle()
    {
     
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
    public void Die()
    {
        // play animation, particles, etc.
        Destroy(gameObject);
    }

}
