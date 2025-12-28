using System.Collections;
using UnityEngine;

public class BlueVampireBoss : Boss
{
    [SerializeField] private Explosion iceExplosionPrefab;
    [SerializeField] private Bullet iceBulletPrefab;

    [SerializeField] private float skillDelay = 0.5f;

    protected override void Skill1()
    {
        if (isDied) return;
        if (animator != null) animator.SetTrigger("Attack");

        StartCoroutine(Skill1Coroutine());
    }

    private IEnumerator Skill1Coroutine()
    {
        if (isDied) yield break;
        isAttacking = true;
        yield return new WaitForSeconds(1.2f);
        if (isDied) { isAttacking = false; yield break; }
        var playerPos = player.transform.position;
        
        float range = 2f;
        float spacing = 0.5f;
        
        // Hiển thị 2 skill indicators hình chữ nhật tạo thành dấu 
        // Diagonal 1: 45 độ (từ dưới trái lên trên phải)
        Vector3 diagonal1Dir = new Vector3(1, 1, 0).normalized;
        SkillIndicatorPool.Instance.SpawnRectangle(playerPos, diagonal1Dir, 0.5f, range * 3, new Color(0, 0.5f, 1f, 0.5f), 1f);
        
        // Diagonal 2: -45 độ (từ dưới phải lên trên trái)
        Vector3 diagonal2Dir = new Vector3(1, -1, 0).normalized;
        SkillIndicatorPool.Instance.SpawnRectangle(playerPos, diagonal2Dir, 0.5f, range * 3, new Color(0, 0.5f, 1f, 0.5f), 1f);
        
        yield return new WaitForSeconds(1f);
        
        // Sinh ra explosions dọc theo 2 đường chéo
        // Đường chéo 1: 45 độ
        for (float dist = -range; dist <= range; dist += spacing)
        {
            if (isDied) break;
            Vector3 explosionPos = playerPos + new Vector3(dist, dist, 0);
            ExplosionPool.Instance.Spawn(iceExplosionPrefab, explosionPos, GetDamage());
        }
        
        // Đường chéo 2: -45 độ
        for (float dist = -range; dist <= range; dist += spacing)
        {
            if (isDied) break;
            Vector3 explosionPos = playerPos + new Vector3(dist, -dist, 0);
            ExplosionPool.Instance.Spawn(iceExplosionPrefab, explosionPos, GetDamage());
        }
        yield return new WaitForSeconds(skillDelay);
        if (!isDied) isAttacking = false ;
    }

    protected override void Skill2()
    {
        if (isDied) return;
        if (animator != null) animator.SetTrigger("Attack");
        StartCoroutine(Skill2Coroutine());
    }

    private IEnumerator Skill2Coroutine()
    {
        if (isDied) yield break;
        isAttacking = true;
        yield return new WaitForSeconds(1.3f);
        if (isDied) { isAttacking = false; yield break; }
        var playerPos = player.transform.position;
        float innerRadius = 2.5f;
        float outerRadius = 3f;
        float midRadius = (innerRadius + outerRadius) * 0.5f;
        float spacing = 1f; // distance between explosions along circumference
        float spawnDelay = 0.01f;

        // Show ring indicator centered on player
        SkillIndicatorPool.Instance.SpawnRing(playerPos, innerRadius, outerRadius, new Color(0, 0.5f, 1f, 0.4f) , 1f);

        // Wait 1s before spawning explosions
        yield return new WaitForSeconds(0.8f);

        // Calculate number of explosions based on spacing along circumference
        float circumference = 2f * Mathf.PI * midRadius;
        int count = Mathf.Max(1, Mathf.FloorToInt(circumference / spacing));

        for (int i = 0; i < count; i++)
        {
            if (isDied) break;
            float angle = (2f * Mathf.PI / count) * i;
            Vector3 spawnPos = playerPos + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * midRadius;
            ExplosionPool.Instance.Spawn(iceExplosionPrefab, spawnPos, GetDamage());
            yield return new WaitForSeconds(spawnDelay);
        }

        yield return new WaitForSeconds(0.2f);
        if (!isDied) isAttacking = false ;
    }

    protected override void Skill3()
    {
        if (isDied) return;
        if (isAttacking) return;
        StartCoroutine(Skill3Croutine());
    }

    private IEnumerator Skill3Croutine()
    {
        isAttacking = true;
        animator.speed =  2f; 
        float duration = 5f;
        float shootInterval = 0.5f;

        // Calculate exact number of shots: fire immediately at t=0, then every shootInterval
        int shotCount = Mathf.FloorToInt(duration / shootInterval);

        for (int i = 0; i < shotCount; i++)
        {
            Flip();
            animator?.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            if (player?.transform == null) break;
            if (isDied) break;
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + direction * 10f;
            BulletPool.Instance.SpawnBullet(
                iceBulletPrefab,
                transform.position,
                targetPosition,
                GetDamage()
            );
            yield return new WaitForSeconds(shootInterval);
        }
        animator.speed = 1f;
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    protected override void Die()
    {
        base.Die();
         BulletPool.Instance?.DestroyUnusedPools(iceBulletPrefab);
        ExplosionPool.Instance?.ClearUnUsedPool(iceExplosionPrefab);  
    }

    
}
