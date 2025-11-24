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
    public float arriveRadius = 1f;
    public float attackCooldownTime = 1.5f;
    public float attackPushBack = 3f;

    Transform player;
    PlayerController playerController;
    Vector3 wanderTarget;
    float stateTimer = 0f;

    private Vector3 homePosition;

    protected override void Awake()
    {
        base.Awake();
        homePosition = transform.position;
    }

    void Start()
    {
        PickNewWanderTarget();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

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
                UpdateCooldown();
                break;
        }
    }

    private void UpdateIdle()
    {
        // wander
        Vector3 toTarget = wanderTarget - transform.position;
        toTarget.y = 0;
        float dist = toTarget.magnitude;
        Vector3 desired = Vector3.zero;

        if (dist < 0.5f) PickNewWanderTarget();
        else
        {
            float speed = horizontalSpeed * (dist > arriveRadius ? 1f : dist / arriveRadius);
            desired = toTarget.normalized * speed;
        }

        // transition to attack if player in range
        if (player != null && Vector3.Distance(player.position, transform.position) <= detectRadius)
        {
            currentState = State.Attack;
        }

        MoveSimple(desired);
    }

    private void UpdateAttack()
    {
        if (player == null) return;

        // move toward player
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        MoveSimple(toPlayer.normalized * horizontalSpeed);

        // if player moves out of detect radius, go back to idle
        if (Vector3.Distance(player.position, transform.position) > detectRadius)
            currentState = State.Idle;
    }

    private void UpdateCooldown()
    {
        if (stateTimer > 0f)
            stateTimer -= Time.deltaTime;
        else
            currentState = State.Attack;
    }

    private void PickNewWanderTarget()
    {
        Vector2 r = Random.insideUnitCircle * wanderRadius;
        wanderTarget = homePosition + new Vector3(r.x, 0, r.y);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Player")) return;

        if (playerController != null)
        {
            // push player and apply damage
            Vector3 pushDir = (playerController.transform.position - transform.position).normalized;
            playerController.TakeDamage(1);
            playerController.AddKnockBack(pushDir * attackPushBack);

            // enter attack cooldown
            currentState = State.AttackCooldown;
            stateTimer = attackCooldownTime;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
