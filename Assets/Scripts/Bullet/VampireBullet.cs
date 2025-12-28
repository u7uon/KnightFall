using UnityEngine;

public class VampireBullet : EnemyBullet
{
    private Animator animator;
     protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
         if (collider2D.CompareTag("Player") && !hasHit)
        {
            PlayerStatsManager.Instance.TakeDame(GetDamge());
            direction = Vector3.zero; 
            if (animator == null)
            {
                animator = GetComponent<Animator>();    
            }
            if (animator != null)
                animator.SetTrigger("Hit");
            hasHit = true;
            Invoke(nameof(ReturnToPool), 0.2f);

        }
    }
}