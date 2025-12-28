using UnityEngine;

[System.Serializable]
public class EffectConfig
{
    public EffectType type;
    public float value;           // Damage multiplier hoặc slow percent
    public float duration;
    public float tickInterval;    // Cho DoT effects
    
    // Có thể thêm
    public bool isStackable;
    public int maxStacks;

    public Color visualColor =>GetVisualColor() ;


    private Color GetVisualColor()
    {
        switch (type)
        {
            case EffectType.Burn :
                return Color.orangeRed;
            case  EffectType.Poison :
                return Color.darkViolet; 
            default : 
                return Color.white;    
        }
    }
}

public enum EffectType
{
    None  ,
    Burn,
    Poison,
    Slow,
    Stun,
    Freeze
    // Dễ dàng thêm effect mới
}



