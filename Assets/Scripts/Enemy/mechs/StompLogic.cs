using UnityEngine;

public class StompLogic : MonoBehaviour
{
    public BasicEnemy parentEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // Check if player is falling down
        if (pc.VerticalVelocity < -1f)
        {
            parentEnemy.Die();
            pc.BounceFromEnemy();
        }
    }
}
