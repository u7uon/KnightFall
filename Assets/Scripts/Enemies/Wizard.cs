using UnityEngine;

public class Wizard : RangedEnemy
{
    [Header("Wizard Settings")]
    [SerializeField] private float spreadAngle = 15f; // Góc giữa các tia đạn (độ)

    protected override void FireBullet()
    {
        if (player?.transform == null) return;

        // Tính hướng chính về phía player
        Vector3 mainDirection = (player.transform.position - transform.position).normalized;
        
        // Tính góc của hướng chính
        float mainAngle = Mathf.Atan2(mainDirection.y, mainDirection.x) * Mathf.Rad2Deg;
        
        // Bắn 3 tia: trái, giữa, phải
        float[] angles = new float[] 
        {
            mainAngle + spreadAngle,  // Tia trái
            mainAngle,                 // Tia giữa (nhắm player)
            mainAngle - spreadAngle   // Tia phải
        };
        
        foreach (float angle in angles)
        {
            // Spawn bullet
            Bullet bullet = BulletPool.Instance.SpawnBullet(
                bulletPrefab,
                transform.position,
                player.transform.position,
                GetDamage()
            );
            
            bullet.transform.position = transform.position;
            
            // Tính target position dựa trên góc
            float rad = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
            Vector3 targetPosition = transform.position + direction * 10f; // 10f là khoảng cách xa
            
            // Setup bullet
            bullet.Direction(targetPosition, GetDamage());
        }
        
        // Kết thúc bắn sau shootRecoveryTime
        Invoke(nameof(EndShooting), shootRecoveryTime);
    }
}
