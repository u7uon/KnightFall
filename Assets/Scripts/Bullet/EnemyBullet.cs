using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
         if (collider2D.CompareTag("Player"))
        {
            PlayerStatsManager.Instance.TakeDame(GetDamge());
            ReturnToPool();
        }
    }
}