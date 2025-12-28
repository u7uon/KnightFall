using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireVampire : Boss
{
    [SerializeField] private Explosion fireBreathExplosion;
    [SerializeField] private Explosion fireExplosion;

    [SerializeField] private Bullet shurikenPrefab;

    protected override void DoRandomSkill()
    {
        if(player != null && Vector3.Distance(transform.position, player.transform.position) > 5f)
        {
            if(Random.value < 0.5f)
                Skill2();
            else
                Skill3();
            return;
        }
        else
        {
            StartCoroutine(Skill1Coroutine());
        }
    }

    protected override void Skill1()
    {
            StartCoroutine(Skill1Coroutine());
    }

    IEnumerator Skill1Coroutine()
    {
        animator?.SetTrigger("Attack");
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        // Calculate direction from current position to player
        Vector3 direction = (player.transform.position - transform.position).normalized;
        
        // Spawn indicator rectangle from pool (5f length, 1f width)
        SkillIndicatorPool.Instance.SpawnCone(
            transform.position,
            direction,
            5f,
            50f,
            new Color(1f, 0f, 0f, 0.6f),
            1f
        );
        
        yield return new WaitForSeconds(1.2f);
        // Spawn fireBreathExplosion from pool
        Explosion explosion = ExplosionPool.Instance.Spawn(
            fireBreathExplosion,
            transform.position,
            GetDamage()
        );
        // Apply rotation to match indicator direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        explosion.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    protected override void Skill2()
    {
        StartCoroutine(Skill2Coroutine());
    }
     IEnumerator Skill2Coroutine()
    {
        isAttacking = true;
        animator.speed = 2f;
        for (int i = 0; i < 3; i++)
        {   
            if (player == null) break;
            animator?.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            BulletPool.Instance.SpawnBullet(
                shurikenPrefab,
                transform.position,
                player.transform.position,
                GetDamage()
            );
            yield return new WaitForSeconds(0.5f);
        }

        animator.speed = 1f;
        isAttacking = false;
    }


    protected override void Skill3()
    {
        animator?.SetTrigger("Attack");
        StartCoroutine(Skill3Coroutine());
    }
    IEnumerator Skill3Coroutine()
    {
        isAttacking = true;
        if (player == null)
        {
            isAttacking = false;
            yield break;
        }

        // Parameters
        float indicatorRadius = 0.3f;
        float explosionDelay = 1f; // delay between indicator and explosion
        float minDistance = 1.5f; // minimum distance between positions
        float areaRadius = 4f; // search area around player
        int targetCount = 15; // number of explosions
        Color indicatorColor = new Color(1f, 0.5f, 0.1f, 0.6f);

        // Center at player's current position
        Vector3 center = player.transform.position;

        // Generate positions using Poisson disk sampling approach
        List<Vector3> positions = new List<Vector3>();
        int maxAttempts = 30;

        for (int i = 0; i < targetCount; i++)
        {
            Vector3 newPos = Vector3.zero;
            bool found = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Random point in circle
                Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * areaRadius;
                newPos = center + new Vector3(randomPoint.x, randomPoint.y, 0f);

                // Check distance from all existing positions
                bool valid = true;
                foreach (Vector3 existingPos in positions)
                {
                    if (Vector3.Distance(newPos, existingPos) < minDistance)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                positions.Add(newPos);
            }
        }
        yield return new WaitForSeconds(0.5f);
        // Spawn all indicators simultaneously
        foreach (Vector3 pos in positions)
        {
            SkillIndicatorPool.Instance?.SpawnCircle(pos, indicatorRadius, indicatorColor, explosionDelay);
        }

        // Wait for indicators to finish
        yield return new WaitForSeconds(explosionDelay);

        // Spawn all explosions simultaneously (no delay between them)
        foreach (Vector3 pos in positions)
        {
            if (ExplosionPool.Instance != null && fireExplosion != null)
            {
                Explosion e = ExplosionPool.Instance.Spawn(fireExplosion, pos, GetDamage());
                if (e != null) e.transform.rotation = Quaternion.identity;
            }
        }
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }



    protected override void Die()
    {
        base.Die();
         BulletPool.Instance?.DestroyUnusedPools(shurikenPrefab);
        ExplosionPool.Instance?.ClearUnUsedPool(fireExplosion);  
        ExplosionPool.Instance?.ClearUnUsedPool(fireBreathExplosion);
    }

}
