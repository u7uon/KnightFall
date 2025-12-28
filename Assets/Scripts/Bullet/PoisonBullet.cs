using System.Linq;
using UnityEngine;

public class PoisonBullet : PlayerBullet
{

    // protected override void OnTriggerEnter2D(Collider2D collider2D)
    // {
    //     if(effectDatas == null || !effectDatas.Any())
    //         base.OnTriggerEnter2D(collider2D);

    //     else
    //     {
    //         if (collider2D.CompareTag("Enemy"))
    //         {
    //             float finalDamage = GetDamge();
    //             collider2D.GetComponent<Enemy>().TakeDamage(finalDamage, canCrit);

    //             if(collider2D.TryGetComponent<EnemyEffectManager>(out var enemyEffect) && effectDatas != null)
    //             {
    //                 foreach(var e in effectDatas){
    //                     enemyEffect.ApplyBonusDamage(finalDamage, e);
    //                 }
                
    //             }

    //             // Nếu có hút máu
    //             TryLifeSteal(finalDamage);

    //             ReturnToPool();
    //         }
    //     }

    //}
    
}