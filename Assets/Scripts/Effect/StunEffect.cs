public class RootEffect : StatusEffect
{
    private bool wasMoving;
    private float lastSpeed ; 
    
    public RootEffect(EffectConfig config, Enemy target) 
        : base(config, target)
    {
    }
    
    public override void OnApply()
    {
        lastSpeed = targetEnemy.GetSpeed(); 
        targetEnemy.SetSpeed(0);
        // Spawn stun stars effect
    }
    
    public override void OnTick(float deltaTime)
    {
        // Maintain stun state
    }
    
    public override void OnRemove()
    {
        if (targetEnemy != null && targetEnemy.IsAlive())
        {
            targetEnemy.SetSpeed(lastSpeed);
        }
    }
}