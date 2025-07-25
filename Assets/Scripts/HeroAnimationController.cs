using UnityEngine;

public class HeroAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

  
    public void SetWalk(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
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
        Debug.Log("Play Kancil Dead animation");
        animator.SetTrigger("Defeated");
    }
}
