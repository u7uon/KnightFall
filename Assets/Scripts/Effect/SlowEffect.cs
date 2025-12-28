using UnityEngine;
public class SlowEffect : StatusEffect
{
    private float originalSpeed;
    private float slowMultiplier;
    
    public SlowEffect(EffectConfig config, Enemy target) 
        : base(config, target)
    {
        slowMultiplier = 1f - config.value; // value = 0.5 → 50% slow
    }
    
    public override void OnApply()
    {
        originalSpeed = targetEnemy.GetSpeed();
        targetEnemy.SetSpeed(originalSpeed * slowMultiplier);
    }
    
    public override void OnTick(float deltaTime)
    {
        // Slow không cần tick logic, chỉ cần maintain speed
    }
    
    public override void OnRemove()
    {
        if (targetEnemy != null && targetEnemy.IsAlive())
        {
            targetEnemy.SetSpeed(originalSpeed);
        }
    }
    
    public override void Refresh(float additionalDuration = 0)
    {
        base.Refresh(additionalDuration);
        // Không reset speed, chỉ extend duration
    }
}