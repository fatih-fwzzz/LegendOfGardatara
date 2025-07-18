using UnityEngine;

public class WarriorMovement : MonoBehaviour

{
    public Rigidbody2D robotRb;
    public float speed;

 

    // Update is called once per frame
    void FixedUpdate()
    {
        robotRb.linearVelocity = Vector2.right * speed;
    }
}
