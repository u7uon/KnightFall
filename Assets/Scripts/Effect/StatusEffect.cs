using UnityEngine;

public abstract class StatusEffect
{
    public EffectType Type { get; protected set; }
    public float Duration { get; protected set; }
    public float RemainingTime { get; protected set; }
    public bool IsActive => RemainingTime > 0;
    
    protected EffectConfig config;
    protected Enemy targetEnemy;


    
    public StatusEffect(EffectConfig config, Enemy target)
    {
        this.config = config;
        this.targetEnemy = target;
        this.Type = config.type;
        this.Duration = config.duration;
        this.RemainingTime = config.duration;
    }
    
    // Lifecycle methods
    public abstract void OnApply();
    public abstract void OnTick(float deltaTime);
    public abstract void OnRemove();
    
    // Common update logic
    public virtual void Update(float deltaTime)
    {
        if (!IsActive) return;
        
        RemainingTime -= deltaTime;
        OnTick(deltaTime);
        
        if (RemainingTime <= 0)
        {
            OnRemove();
        }
    }
    
    // Refresh effect khi apply lại
    public virtual void Refresh(float additionalDuration = 0)
    {
        RemainingTime = Mathf.Max(RemainingTime, Duration);
        if (additionalDuration > 0)
        {
            RemainingTime += additionalDuration;
        }
    }
}