using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class EnemyEffectManager : MonoBehaviour
{
    private Enemy enemy;
    
    // Dictionary để quản lý từng loại effect riêng biệt
    private Dictionary<EffectType, StatusEffect> activeEffects = new Dictionary<EffectType, StatusEffect>();
    
    // Stack tracking cho effects có thể stack
    private Dictionary<EffectType, List<StatusEffect>> stackableEffects = new Dictionary<EffectType, List<StatusEffect>>();
    
    private Dictionary<EffectType, GameObject> effectIcons = new Dictionary<EffectType, GameObject>();
    
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    
    private void Update()
    {
        UpdateAllEffects();
    }
    
    // ============================================
    // PUBLIC API
    // ============================================
    
    /// <summary>
    /// Apply effect từ weapon
    /// </summary>
    public void ApplyEffect(EffectConfig config, float weaponDamage = 0)
    {
        if (config == null || config.type == EffectType.None) return;
        
        StatusEffect effect = CreateEffect(config, weaponDamage);
        
        if (effect == null) return;
        
        // Check if effect đã tồn tại
        if (activeEffects.ContainsKey(config.type))
        {
            HandleExistingEffect(config, effect);
        }
        else
        {
            AddNewEffect(effect);
        }
    }
    
    /// <summary>
    /// Xóa effect cụ thể (dùng cho dispel/cleanse)
    /// </summary>
    public void RemoveEffect(EffectType type)
    {
        if (activeEffects.TryGetValue(type, out StatusEffect effect))
        {
            effect.OnRemove();
            activeEffects.Remove(type);
        }
    }
    
    /// <summary>
    /// Clear tất cả effects (khi enemy chết)
    /// </summary>
    public void ClearAllEffects()
    {
        foreach (var effect in activeEffects.Values)
        {
            effect.OnRemove();
        }
        activeEffects.Clear();
        
        foreach (var icon in effectIcons.Values)
        {
            if (icon != null) Destroy(icon);
        }
        effectIcons.Clear();
    }
    
    /// <summary>
    /// Check xem có effect nào đang active không
    /// </summary>
    public bool HasEffect(EffectType type)
    {
        return activeEffects.ContainsKey(type);
    }
    
    public StatusEffect GetEffect(EffectType type)
    {
        return activeEffects.TryGetValue(type, out StatusEffect effect) ? effect : null;
    }
    
    // ============================================
    // INTERNAL LOGIC
    // ============================================
    
    private StatusEffect CreateEffect(EffectConfig config, float weaponDamage)
    {
        switch (config.type)
        {
            case EffectType.Burn:
                return new BurnEffect(config, enemy, weaponDamage);
                
            case EffectType.Poison:
                return new PoisonEffect(config, enemy, weaponDamage);
                
            case EffectType.Slow:
                return new SlowEffect(config, enemy);
                
             case EffectType.Stun:
                 return new RootEffect(config, enemy);
                
            // Thêm effect mới ở đây
            
            default:
                return null;
        }
    }
    
    private void HandleExistingEffect(EffectConfig config, StatusEffect newEffect)
    {
        StatusEffect existingEffect = activeEffects[config.type];
        
        if (config.isStackable)
        {
            // Stack logic: add new effect instance
            if (!stackableEffects.ContainsKey(config.type))
            {
                stackableEffects[config.type] = new List<StatusEffect>();
            }
            
            if (stackableEffects[config.type].Count < config.maxStacks)
            {
                stackableEffects[config.type].Add(newEffect);
                newEffect.OnApply();
            }
            else
            {
                // Đã đạt max stacks, refresh oldest
                existingEffect.Refresh();
            }
        }
        else
        {
            // Replace/Refresh existing effect
            existingEffect.Refresh();
        }
    }
    
    private void AddNewEffect(StatusEffect effect)
    {
        activeEffects[effect.Type] = effect;
        effect.OnApply();
    }
    
    private void UpdateAllEffects()
    {
        // Update main effects
        var effectsToRemove = new List<EffectType>();
        
        foreach (var kvp in activeEffects)
        {
            kvp.Value.Update(Time.deltaTime);
            
            if (!kvp.Value.IsActive)
            {
                effectsToRemove.Add(kvp.Key);
            }
        }
        
        // Remove expired effects
        foreach (var type in effectsToRemove)
        {
            RemoveEffect(type);
        }
        
        // Update stacked effects
        UpdateStackedEffects();
    }
    
    private void UpdateStackedEffects()
    {
        foreach (var kvp in stackableEffects.ToList())
        {
            var expiredEffects = new List<StatusEffect>();
            
            foreach (var effect in kvp.Value)
            {
                effect.Update(Time.deltaTime);
                if (!effect.IsActive)
                {
                    expiredEffects.Add(effect);
                }
            }
            
            foreach (var expired in expiredEffects)
            {
                kvp.Value.Remove(expired);
                expired.OnRemove();
            }
            
            if (kvp.Value.Count == 0)
            {
                stackableEffects.Remove(kvp.Key);
            }
        }
    }
}