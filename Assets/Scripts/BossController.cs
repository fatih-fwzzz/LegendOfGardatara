using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    public float fleeSpeed = 5f;

    private bool isFleeing = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Flee()
    {
        isFleeing = true;
        if (animator != null)
            animator.SetTrigger("Flee"); 
    }

    private void Update()
    {
        if (isFleeing)
        {
            rb.linearVelocity = new Vector2(1f, 1f) * fleeSpeed;
        }
    }
}
