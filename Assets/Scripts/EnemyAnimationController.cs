using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalk(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void SetAttack()
    {
        animator.SetTrigger("Attack");
    }
}
