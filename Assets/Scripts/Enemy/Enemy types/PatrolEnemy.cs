using UnityEngine;

public class PatrolEnemy : Enemy
{
    public Transform[] waypoints;
    public float arriveRadius = 1f;
    public float patrolSpeedMultiplier = 1.2f; // faster than basic enemies
    public float pushForce = 8f;

    private int currentIndex = 0;
    private bool forward = true; // waypoint order direction

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
            SelectNextWaypoint();
        }
    }

    private void SelectNextWaypoint()
    {
        if (forward)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
                currentIndex = 0; // loop
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = waypoints.Length - 1;
        }
    }

}