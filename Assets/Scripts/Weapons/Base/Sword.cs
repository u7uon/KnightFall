using System.Collections;
using UnityEngine;

public class SwordWeapon : WeaponBase
{
    [Header("Swing Settings")]
    public float swingArc = 100f;
    public float swingDuration = 0.5f;

    public float thrustThreshold = 1f; // Khoảng cách để kích hoạt đâm thay vì chém
    public float thrustSpeed = 50f;
    public float thrustOvershoot = 1f;
    public float returnSpeed = 20f;
    public float thrustDamageMultiplier = 0.8f; // Giảm 20% damage

    private bool isSwinging = false;

    protected override void TryAttack()
    {
        if (isSwinging) return; 
        Transform target = FindNearestEnemy();
        if (target == null) return; 
        
        float distToEnemy = Vector2.Distance(player.position, target.position);
        
        // ✅ Nếu enemy quá gần (<= 1f) thì đâm thay vì chém
        if (distToEnemy <= thrustThreshold)
        {
            StartCoroutine(ThrustAttack(target));
        }
        else
        {
            Vector2 dir = (target.position - player.position).normalized;
            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bool isEnemyOnLeft = dir.x < 0; 
            StartCoroutine(SwingAttack(baseAngle, isEnemyOnLeft));
        }
    }

    // === CƠ CHẾ ĐÂM KHI ENEMY GẦN ===
     private IEnumerator ThrustAttack(Transform target)
    {
        isSwinging = true;

        Vector3 startPos = transform.localPosition;
        
        // ✅ Tính hướng và khoảng cách GIỐNG THRUST
        Vector2 dir = (target.position - transform.position).normalized;
        float distToEnemy = Vector2.Distance(player.position, target.position);
        float actualDistance = distToEnemy + thrustOvershoot;
        Vector3 endPos = startPos + (Vector3)dir * actualDistance;

        // ✅ GIẢM DAMAGE 20% khi đâm
        float originalDamage = weaponStats.Damage;
        weaponStats.Damage *= thrustDamageMultiplier;

        // === PHASE 1: Đâm tới ===
        float elapsed = 0f;
        float thrustDuration = actualDistance / thrustSpeed;
        isAttacking = true;
        
        while (elapsed < thrustDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / thrustDuration);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        
        // === PHASE 2: Rút về ===
        isAttacking = false;
        elapsed = 0f;
        float returnDuration = actualDistance / returnSpeed;
        
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(endPos, startPos, elapsed / returnDuration);
            yield return null;
        }

        transform.localPosition = startPos;

        // ✅ Khôi phục damage về ban đầu
        weaponStats.Damage = originalDamage;
        
        isSwinging = false;
    }

    // === CƠ CHẾ CHÉM BÌNH THƯỜNG ===
    private IEnumerator SwingAttack(float centerAngle, bool isEnemyOnLeft)
    {
        isSwinging = true;
        isAttacking = true;
        
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        Transform target = FindNearestEnemy();
        float swingRadius = weaponStats.Range;
        
        if (target != null)
        {
            float distToEnemy = Vector2.Distance(player.position, target.position);
            swingRadius = Mathf.Clamp(distToEnemy, 1f, weaponStats.Range);
        }
        
        float startAngle, endAngle;
        if (isEnemyOnLeft)
        {
            startAngle = centerAngle - swingArc / 2f;
            endAngle = centerAngle + swingArc / 2f;
        }
        else
        {
            startAngle = centerAngle + swingArc / 2f;
            endAngle = centerAngle - swingArc / 2f;
        }

        // === PHASE 1: Di chuyển đến vị trí bắt đầu chém ===
        float moveOutDuration = swingDuration * 0.2f;
        float elapsed = 0f;
        
        Vector3 startSwingOffset = Quaternion.Euler(0, 0, startAngle) * Vector3.right * swingRadius;
        Vector3 targetStartPos = player.position + startSwingOffset;
        Quaternion targetStartRot = Quaternion.Euler(0, 0, startAngle);
        
        while (elapsed < moveOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveOutDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetStartPos, t);
            transform.rotation = Quaternion.Lerp(startRot, targetStartRot, t);

            yield return null;
        }

        // === PHASE 2: Chém qua ===
        float swingCutDuration = swingDuration * 0.4f;
        elapsed = 0f;
        
        while (elapsed < swingCutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingCutDuration;
            
            t = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            Vector3 offset = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * swingRadius;
            transform.position = player.position + offset;
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }
        
        AudioManager.Instance.PlaySFX("sword_attack");

        // === PHASE 3: Rút về vị trí ban đầu ===
        isAttacking = false;
        float returnDuration = swingDuration * 0.2f;
        elapsed = 0f;
        
        Vector3 endSwingPos = transform.position;
        Quaternion endSwingRot = transform.rotation;
        
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(endSwingPos, startPos, t);
            transform.rotation = Quaternion.Lerp(endSwingRot, startRot, t);

            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isSwinging = false; 
    }
}