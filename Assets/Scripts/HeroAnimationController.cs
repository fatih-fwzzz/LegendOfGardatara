using UnityEngine;

public class HeroAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Mengatur animasi berjalan.
    /// </summary>
    public void SetWalk(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    /// <summary>
    /// Trigger animasi menyerang.
    /// </summary>
    public void SetAttack()
    {
        animator.SetTrigger("isAttacking");
    }

    /// <summary>
    /// Trigger animasi terkena hit.
    /// </summary>
    public void TriggerHit()
    {
        animator.SetTrigger("isHit");
    }

    /// <summary>
    /// Trigger animasi defeated.
    /// </summary>
    public void SetDefeated()
    {
        animator.SetTrigger("isDefeated");
    }
    
}