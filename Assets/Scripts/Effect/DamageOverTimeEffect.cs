using UnityEngine;

public abstract class DamageOverTimeEffect : StatusEffect
{
    protected float damagePerTick;
    protected float tickTimer;
    protected float totalDamageDealt;
    
    public DamageOverTimeEffect(EffectConfig config, Enemy target, float initialDamage) 
        : base(config, target)
    {
        // initialDamage = damage từ weapon * config.value
        damagePerTick = initialDamage / (config.duration / config.tickInterval);
        tickTimer = 0f;
    }
    
    public override void OnApply()
    {
    }
    
    public override void OnTick(float deltaTime)
    {
        tickTimer += deltaTime;
        
        if (tickTimer >= config.tickInterval)
        {
            if (targetEnemy != null && targetEnemy.IsAlive() )
            {
                targetEnemy.TakeEffectDamage(damagePerTick, config.visualColor);
                totalDamageDealt += damagePerTick;
            }
            tickTimer = 0f;
        }
    }
    
    public override void OnRemove()
    {

    }
}