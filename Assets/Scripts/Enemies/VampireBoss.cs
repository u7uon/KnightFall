using UnityEngine;
using System.Collections;

public class Vampire : Boss
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Explosion poisonExplosionPrefab;

    override protected void DoRandomSkill()
    {
        if (isDied || isAttacking) return ;
        
        if(player != null && Vector3.Distance(transform.position, player.transform.position) > 3f)
        {
            int randSkill = Random.Range(1,3);
            switch(randSkill)
            {
                case 1 : 
                    Skill1();
                    break;
                case 2 : 
                    Skill2(); 
                    break;
            }
            return;
        }
        else
        {
            Skill3();
        }
    }

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
        yield return new WaitForSeconds(1.3f);
        if (isDied) { isAttacking = false; yield break; }
        // Bắn 12 viên đạn theo hình tròn (mỗi viên cách nhau 30 độ)
        int bulletCount = 12;
        float angleStep = 360f / bulletCount;
        
        for (int i = 0; i < bulletCount; i++)
        {
            if (isDied) break;
            float angle = i * angleStep;
            
            // Chuyển góc sang radian
            float radians = angle * Mathf.Deg2Rad;
            
            // Tính hướng bắn
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            
            // Tính vị trí đích (xa hơn boss theo hướng direction)
            Vector3 targetPosition = transform.position + (Vector3)direction * 10f; // 10f = khoảng cách bắn
            
            // Spawn bullet từ pool
            Bullet bullet = BulletPool.Instance.SpawnBullet(
                bulletPrefab,
                transform.position,
                targetPosition,
                GetDamage()
            );

        }
        yield return new WaitForSeconds(0.6f);
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
        yield return new WaitForSeconds(0.5f);
        if (isDied) { isAttacking = false; yield break; }
        
        int bulletCount = 12;
        float angleStep = 360f / bulletCount;
        float rotationOffset = 15f; 

        // Bắn 3 vòng đạn, mỗi vòng lệch với vòng trước 15 độ 
        for(int j = 0; j < 2; j++)
        {
            if (isDied) break;
            float currentRotation = j * rotationOffset; 
            
            // Bắn 12 viên đạn theo hình tròn (mỗi viên cách nhau 30 độ)
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep + currentRotation; // Cộng thêm độ lệch của vòng hiện tại
                
                // Chuyển góc sang radian
                float radians = angle * Mathf.Deg2Rad;
                
                // Tính hướng bắn
                Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                
                // Tính vị trí đích (xa hơn boss theo hướng direction)
                Vector3 targetPosition = transform.position + (Vector3)direction * 10f;
                
                // Spawn bullet từ pool
                if (isDied) break;
                Bullet bullet = BulletPool.Instance.SpawnBullet(
                    bulletPrefab,
                    transform.position,
                    targetPosition,
                    GetDamage()
                );
            }
            yield return new WaitForSeconds(0.5f); 
        }
        yield return new WaitForSeconds(0.5f);
        if (!isDied) isAttacking = false; 
    }
    protected override void Skill3()
    {
        // Prevent overlapping skill calls and skip if already dead
        if (isDied) return;
        if (isAttacking) return;
        StartCoroutine(Skill3Croutine());
    }

    private IEnumerator Skill3Croutine()
    {
        isAttacking = true;
        animator?.SetTrigger("Attack");
        yield return new WaitForSeconds(0.3f);
        //spawn 8 retangle indicators for 8 directions in 1s
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f; // 0, 45, 90, ..., 315 degrees
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
            // Spawn indicator (assuming IndicatorPool and Indicator class exist)
            SkillIndicatorPool.Instance.SpawnRectangle(
                transform.position,
                direction,
                4f, // length
                0.5f, // width
                new Color(1f, 0f, 1f, 0.6f), // purple color
                0.9f // duration
            );
        }

        yield return new WaitForSeconds(1f);
        ExplosionPool.Instance.Spawn(
            poisonExplosionPrefab,
            transform.position,
            GetDamage()
        );   

        yield return new WaitForSeconds(0.5f);
        isAttacking = false ;   

    }

    protected override void Die()
    {
        base.Die();
          BulletPool.Instance.DestroyUnusedPools(bulletPrefab);
        ExplosionPool.Instance.ClearUnUsedPool(poisonExplosionPrefab);

    }


}