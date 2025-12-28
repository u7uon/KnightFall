using UnityEngine;

public class BurnEffect : DamageOverTimeEffect
{
    public BurnEffect(EffectConfig config, Enemy target, float weaponDamage) 
        : base(config, target, weaponDamage * config.value)
    {
        // Burn-specific initialization
    }
    
    public override void OnApply()
    {
        base.OnApply();
    }
    
    public override void OnRemove()
    {
        base.OnRemove();
        // Remove fire particles
    }
}