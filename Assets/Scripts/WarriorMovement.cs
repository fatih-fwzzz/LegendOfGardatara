using UnityEngine;

public class WarriorMovement : MonoBehaviour
{
    public Rigidbody2D heroRb;
    public float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        heroRb.linearVelocity = Vector2.right * speed;
    }
}
