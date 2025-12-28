using UnityEngine; 
public class PlayerBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Enemy"))
        {

            Enemy enemy = collider2D.GetComponent<Enemy>();
            if (enemy == null) return;

            if(!enemy.IsAlive()) return; 
            
            // 1. Tính damage
            float finalDamage = GetDamge();
            
            // 2. Gây damage cho enemy
            enemy.TakeDamage(finalDamage, canCrit);
            
            // 3. Apply effects (CÁCH MỚI)
            ApplyEffectsToEnemy(enemy, finalDamage);
            
            // 4. Life steal
            TryLifeSteal(finalDamage);
            
            // 5. Return bullet về pool
            ReturnToPool();
        }
    }
    
    /// <summary>
    /// Apply tất cả effects lên enemy 
    /// </summary>
    private void ApplyEffectsToEnemy(Enemy enemy, float damageDealt)
    {
        // Kiểm tra null
        if (effectDatas == null || effectDatas.Count == 0)
            return;
        
        // Lấy EnemyEffectManager component
        EnemyEffectManager effectManager = enemy.GetComponent<EnemyEffectManager>();
        
        if (effectManager == null)
        {
            return;
        }
        
        // Apply từng effect
        foreach (var effectConfig in effectDatas)
        {
            if (effectConfig != null && effectConfig.type != EffectType.None)
            {
                // Magic happens here! 
                effectManager.ApplyEffect(effectConfig, damageDealt);
            }
        }
    }
    
    /// <summary>
    /// Hút máu từ damage gây ra
    /// </summary>
    private void TryLifeSteal(float dealtDamage)
    {
        var stats = PlayerStatsManager.Instance;
        if (stats == null) return;
        
        float lifeSteal = stats.GetLifeSteal();
        
        if (lifeSteal > 0f)
        {
            float healAmount = dealtDamage * lifeSteal;
            stats.HealthUp(healAmount);
        }
    }
}