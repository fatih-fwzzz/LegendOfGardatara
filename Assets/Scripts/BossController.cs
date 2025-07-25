using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Boss Fly Config")]
    public float bossFlyWait = 0.5f;
    public float fleeSpeed = 5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Flee()
    {
        if (animator != null)
        {
            animator.SetTrigger("Flee");
            StartCoroutine(FleeAfterDelay());
        }
    }

    private IEnumerator FleeAfterDelay()
    {
        yield return new WaitForSeconds(bossFlyWait);
        rb.linearVelocity = Vector2.right * fleeSpeed;
    }
}
