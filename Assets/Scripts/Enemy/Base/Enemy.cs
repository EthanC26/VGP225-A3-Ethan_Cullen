using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    protected CharacterController cc;
    public float horizontalSpeed = 5f;
    [HideInInspector] public Vector3 velocity;


    protected virtual void Awake()
    {
        cc = GetComponent<CharacterController>();
    }


    protected void MoveSimple(Vector3 desiredVelocity)
    {
        // movement on XZ plane + gravity handled here
        Vector3 v = desiredVelocity;
        v.y = velocity.y; // preserve vertical
                          // gravity
        if (!cc.isGrounded) v.y += Physics.gravity.y * Time.deltaTime;
        else if (v.y < 0) v.y = -1f; // keep grounded


        velocity = v;
        cc.Move(velocity * Time.deltaTime);
    }
}

