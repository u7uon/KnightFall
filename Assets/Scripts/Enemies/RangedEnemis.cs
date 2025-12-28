using UnityEngine;

public class RangedEnemy : Enemy
{
   
    [Header("Bullet Settings")]
    [SerializeField] protected Bullet bulletPrefab;
    
    [Header("Shooting Timing")]
    [SerializeField] protected float shootAnimationDelay = 0.7f;  // Delay trước khi bắn đạn
    [SerializeField] protected float shootRecoveryTime = 0.5f;    // Thời gian recovery sau khi bắn

    private float lastAttackTime = 0f;
    private bool isInAttackRange = false;
    private bool isShooting = false; // Trạng thái đang bắn
    private Vector3 targetShootPosition; // Vị trí player lúc bắt đầu bắn


    protected override void Update()
    {
        if (isDied) return;
        
        // Nếu đang bắn => KHÔNG làm gì cả, chờ bắn xong
        if (isShooting) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        isInAttackRange = distanceToPlayer <= enemyStats.Range;

        if (isInAttackRange)
        {
            // Trong tầm => dừng lại nếu đủ gần
            if (distanceToPlayer <= enemyStats.Range-2)
            {
                // Dừng lại, thử bắn
                TryAttack();
            }
            else
            {
                // Tiến lại gần hơn
                MoveTowardsPlayer();
            }
        }
        else
        {
            // Ngoài tầm => đuổi theo player
            MoveTowardsPlayer();
        }
    }

    protected override void TryAttack()
    {
        // Check cooldown
        if (Time.time < lastAttackTime + enemyStats.Cooldown)
            return;

        if (player?.transform == null) return;

        // Bắt đầu bắn => lưu vị trí player hiện tại
        isShooting = true;
        targetShootPosition = player.transform.position;
        
        animator.SetTrigger("Shoot");
        
        // Update attack time
        lastAttackTime = Time.time;
        // Gọi hàm bắn sau shootAnimationDelay (0.5s)
        Invoke(nameof(FireBullet), shootAnimationDelay);
    }

    protected virtual void FireBullet()
    {

        // Spawn bullet từ pool
        // Sử dụng cùng BulletPool với Player
        Bullet bullet = BulletPool.Instance.SpawnBullet(
            bulletPrefab,
            transform.position,
            player.transform.position,
            GetDamage()
        );
        
        Vector3 spawnPosition = transform.position;
        bullet.transform.position = spawnPosition;
        
        // Setup bullet - BẮN VỀ VỊ TRÍ ĐÃ LƯU (không quan tâm player còn ở đó không)
        bullet.Direction(targetShootPosition, GetDamage());
        
        // Kết thúc bắn sau shootRecoveryTime (0.5s)
        Invoke(nameof(EndShooting), shootRecoveryTime);
    }

    protected void EndShooting()
    {
        isShooting = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable(); 
        // Hủy tất cả Invoke khi object bị disable
        CancelInvoke();
        isShooting = false;
    }
}
