using UnityEngine;

public class BowWeapon : WeaponBase
{
    [Header("Bow Settings")]
    [SerializeField] private Bullet bulletPrefab; // Loại đạn của bow này
    private Animator bowAnimator;

    void Awake()
    {
        bowAnimator = GetComponent<Animator>();
    }


    protected override void TryAttack()
    {
        Transform target = FindNearestEnemy();
        if (target == null) return;

        if (bowAnimator != null)
            bowAnimator?.SetTrigger("Shoot");

        var (damage , canCrit) = CalculateFinalDamage() ; 

        // Sử dụng BulletPool chung thay vì tạo pool riêng
        Bullet bullet = BulletPool.Instance.SpawnBullet(
            bulletPrefab,
            transform.position,
            target.position,
            damage,
            weaponStats.effects,
            canCrit
        );
        //AudioManager.Instance.PlaySFX("bow_attack");
    }
}