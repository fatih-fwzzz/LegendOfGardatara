using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public Rigidbody2D robotRb;
    public float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        robotRb.linearVelocity = Vector2.left * speed;
    }
}
