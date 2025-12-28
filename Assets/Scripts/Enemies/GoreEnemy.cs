using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class GoreEnemy : Enemy
{
    [Header("Gore Attack Settings")]
    [SerializeField] private float aimDuration = 0.2f;      
    [SerializeField] private float chargeSpeed = 8f;        // tốc độ húc cố định
    
    private bool isCharging = false;
    private Vector3 chargeDirection;
    private float lastAttackTime = 0f;

    protected override void Update()
    {
        if (isDied) return;

        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= enemyStats.Range && Time.time >= lastAttackTime + enemyStats.Cooldown)
        {
            TryAttack();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    protected override void TryAttack()
    {
        if (player?.transform == null) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        StartCoroutine(ChargeAttackSequence());
    }

    private IEnumerator ChargeAttackSequence()
    {
        // ===== PHASE 1: AIM =====
        animator.SetBool("Aim", true);
        float aimTimer = 0f;
        while (aimTimer < aimDuration)
        {
            if (player != null)
            {
                chargeDirection = (player.transform.position - transform.position).normalized;
                Flip();
            }

            aimTimer += Time.deltaTime;
            yield return null;
        }

        SkillIndicatorPool.Instance.SpawnRectangle(
            transform.position,
            chargeDirection,
            enemyStats.Range,
            0.4f,
            new Color(1f, 0f, 0f),
            0.5f
        );

        animator.SetBool("Aim", false);
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.5f);

        // ===== BẮT ĐẦU HÚC =====
        isCharging = true;


        float travelDistance = 0f;

        while (travelDistance < enemyStats.Range)
        {
            if (isDied) break;

            float moveStep = chargeSpeed * Time.deltaTime;

            transform.position += chargeDirection * moveStep;

            travelDistance += moveStep;

            yield return null;
        }

        // ===== KẾT THÚC =====
        isCharging = false;
        animator.SetBool("Attack", false);

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();

        isAttacking = false;
        isCharging = false;

        if (animator != null)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Attack", false);
        }
    }
}

