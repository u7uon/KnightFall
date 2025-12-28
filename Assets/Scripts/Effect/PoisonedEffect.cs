public class PoisonEffect : DamageOverTimeEffect
{
    public PoisonEffect(EffectConfig config, Enemy target, float weaponDamage) 
        : base(config, target, weaponDamage * config.value)
    {
    }
    
    public override void OnApply()
    {
        base.OnApply();
        // Spawn poison clouds
        // Change enemy tint to green
    }
}