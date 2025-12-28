using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("General Weapon Settings")]
    public WeaponStats weaponStats;
    protected Transform player;
    protected LayerMask enemyLayer;
    protected float cooldownTimer;

    protected bool isAttacking = false;
    protected Vector3 originalLocalPos;
    protected Vector3 localScale;
    protected bool isThrustLocked = false;

    #region Bobbing Settings
    [Header("Bobbing Effect")]
    [SerializeField] private bool enableBobbing = true;
    [SerializeField] private float bobbingSpeed = 2f;
    [SerializeField] private float bobbingAmount = 0.08f;
    private float bobbingTimer = 0f;
    #endregion

    public virtual void Init(Transform playerTransform, LayerMask layer)
    {
        bobbingAmount += Random.Range(0.01f, 0.015f); 
        bobbingSpeed += Random.Range(0.1f, 0.15f);

        player = playerTransform;
        enemyLayer = layer;
        originalLocalPos = transform.localPosition;
        localScale = transform.localScale;

        
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isAttacking) return;

        if (collider.CompareTag("Enemy"))
        {
            var (finalDamage, crit  ) = CalculateFinalDamage();
            collider.GetComponent<Enemy>().TakeDamage(finalDamage,crit);

            if(collider.TryGetComponent<EnemyEffectManager>(out var enemyEffect) && weaponStats.effects !=null )
            {
                foreach(var e in weaponStats.effects)
                    enemyEffect.ApplyEffect(e,finalDamage);
            }

            TryLifeSteal(finalDamage);
        }
    }

    protected (float, bool) CalculateFinalDamage()
{
    var stats = PlayerStatsManager.Instance;

    // 🟩 Damage gốc của vũ khí
    float totalDamage = weaponStats.Damage * stats.GetPlayerDameMutilplier();

    // 🍀 Luck tăng Crit Chance (mỗi 1% Luck = +0.5% Crit Chance)
    float baseCritChance = stats.GetPlayerDameCriticialChance();
    float luckBonus = stats.GetPlayerLuck() * 0.01f; // Conversion rate: 50%
    float finalCritChance = baseCritChance + luckBonus;
    
    // Clamp crit chance trong khoảng hợp lý (0% - 100%)
    finalCritChance = Mathf.Clamp01(finalCritChance);

    // 🎯 Check chí mạng với Crit Chance đã được buff bởi Luck
    var canCrit = Random.value < finalCritChance;

    // 🎯 Nếu chí mạng, nhân thêm Critical Damage
    if (canCrit)
    {
        totalDamage *= stats.GetPlayerCriticialDame();
    }

    return (totalDamage, canCrit);
}

    private void TryLifeSteal(float dealtDamage)
    {
        float lifeSteal = PlayerStatsManager.Instance.GetLifeSteal() ; 
        if (lifeSteal > 0f)
        {
            float healAmount = dealtDamage * lifeSteal;
            PlayerStatsManager.Instance.HealthUp(healAmount);
        }
    }
    public virtual void Tick(float deltaTime)
    {
        // Bobbing hiệu ứng idle
        if (enableBobbing && !isAttacking)
        {
            bobbingTimer += deltaTime * bobbingSpeed;
            float offsetY = Mathf.Sin(bobbingTimer) * bobbingAmount;
            Vector3 pos = originalLocalPos;
            pos.y += offsetY;
            transform.localPosition = pos;
        }

        cooldownTimer -= deltaTime;
        if (cooldownTimer <= 0f)
        {
            TryAttack();
            cooldownTimer = weaponStats.Cooldown  / PlayerStatsManager.Instance.GetAttackSpeed() ;
        }
        RotateTowardEnemy();

    }

    protected virtual void TryAttack() { }

    protected Transform FindNearestEnemy(float ex = 0)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.position, weaponStats.Range + ex, enemyLayer);
        Transform nearest = null;
        float minDist = Mathf.Infinity;
        foreach (var e in enemies)
        {
            Enemy enemy = e.GetComponent<Enemy>();
            if(enemy != null && enemy.IsAlive())
            {
                float dist = (e.transform.position - player.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = e.transform;
                }
            }
            
        }
        return nearest;
    }


    private void RotateTowardEnemy() { 
        // ✅ CHỈ rotate khi KHÔNG đang thrust (không attack) 
        if (isAttacking) return;
        Transform target = FindNearestEnemy(2);
        if (target == null)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (!FindAnyObjectByType<PlayerController>().flipx) { transform.localScale = Vector3.one; }
            else { transform.localScale = new Vector3(-1, 1, 1); }
            return;
        }
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;



        // Flip Y axis nếu cần 
        if (angle > 90 || angle < -90)
        {
            transform.localScale = new Vector3(localScale.x, -Mathf.Abs(localScale.y), localScale.z);
            transform.rotation = Quaternion.Euler(0, 0, angle + 45);
        }
        else
        {
            transform.localScale = new Vector3(localScale.x, Mathf.Abs(localScale.y), localScale.z);
            transform.rotation = Quaternion.Euler(0, 0, angle - 45);
        } 
        }
}
