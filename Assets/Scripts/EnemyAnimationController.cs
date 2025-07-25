using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalk(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void SetAttack(bool isAttack)
    {
        animator.SetBool("isAttacking", isAttack);
    }

    public void TriggerHit()
    {
        animator.SetTrigger("Hit");
    }

    public void SetDefeated()
    {
        Debug.Log("Play Octopus Dead animation");
        animator.SetTrigger("Defeated");
    }
}
