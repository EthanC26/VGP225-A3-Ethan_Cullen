using UnityEngine;

public class PatrolEnemy : Enemy
{
    public Transform[] waypoints;
    public float arriveRadius = 1f;
    public float patrolSpeedMultiplier = 1.2f; // faster than basic enemies
    public float pushForce = 8f;

    private int currentIndex = 0;

    public float hitCooldown = 0.5f; // half a second between hits
    private float lastHitTime = -10f;

    protected override void Awake()
    {
        base.Awake();
        horizontalSpeed *= patrolSpeedMultiplier;
    }

    private void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPos = waypoints[currentIndex].position;
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0;

        float dist = toTarget.magnitude;

        // Arrival behavior
        float slowDownFactor = Mathf.Clamp01(dist / arriveRadius);
        Vector3 desired = toTarget.normalized * horizontalSpeed * slowDownFactor;

        MoveSimple(desired);

        // Reached waypoint
        if (dist < arriveRadius)
        {
            Debug.Log("Reached waypoint " + currentIndex);
            SelectNextWaypoint();
        }
    }

    private void SelectNextWaypoint()
    {
        if(waypoints.Length <= 1) return;

        int next;
        do
        {
            next = Random.Range(0, waypoints.Length);
        }
        while (next == currentIndex);

        currentIndex = next;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.TryGetComponent<PlayerController>(out var player))
        {
            if (Time.time - lastHitTime >= hitCooldown)
            {
                player.TakeDamage(1);

                // Push player away
                Vector3 dir = (player.transform.position - transform.position).normalized;
                player.AddKnockBack(dir * pushForce);

                lastHitTime = Time.time;
            }
        }

        // Collision with other enemies → reverse path
        //if (hit.collider.TryGetComponent<Enemy>(out var other) && other != this)
        //{
        //    forward = !forward; // reverse direction
        //}
    }
}
