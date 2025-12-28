using System.Collections;
using UnityEngine;

public class SlashedEnemy : Enemy
{

    [Header("Slash Attack Settings")]
    private float aimDuration = 1f; // Thời gian nhắm
    [SerializeField] private Explosion explosionPrefab;
    float explosionSpacing = 1f; // Khoảng cách giữa các explosion
    private Vector3 slashDirection;
    private  bool isSlashing = false;
    private float lastAttackTime = 0f;
    protected override void Update()
    {
        if (isDied) return;
        
        // Nếu đang tấn công => không làm gì
        if (isAttacking) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        // Kiểm tra trong tầm tấn công và đã hết cooldown
        if (distanceToPlayer <= enemyStats.Range && Time.time >= lastAttackTime + enemyStats.Cooldown)
        {
            TryAttack();
        }
        else
        {
            // Đuổi theo player bình thường
            MoveTowardsPlayer();
        }
    }

    protected override void TryAttack()
    {
        if (player?.transform == null) return;
        
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // Bắt đầu quá trình tấn công
        StartCoroutine(ChargeAttackSequence());
    }
    
    private IEnumerator ChargeAttackSequence()
    {
        // === PHASE 1: NHẮM (AIM) ===
        animator.SetBool("Aim",true);
        float aimTimer = 0f;
        while (aimTimer < aimDuration)
        {
            // Cập nhật hướng nhắm theo player (tracking)
            if (player?.transform != null)
            {
                slashDirection = (player.transform.position - transform.position).normalized;
                Flip();
            }
            aimTimer += Time.deltaTime;
            yield return null;
        }
        SkillIndicatorPool.Instance.SpawnRectangle(
            transform.position,
            slashDirection,
            enemyStats.Range,
            0.4f,
            new Color(1f, 0f, 0f),
            0.5f
        );
        animator.SetBool("Attack", true);
        animator.SetBool("Aim", false);
        yield return new WaitForSeconds(0.5f);
        // === PHASE 2: SPAWN EXPLOSIONS ===
        isSlashing = true;
        int explosionCount = Mathf.Max(1, (int)(enemyStats.Range / explosionSpacing)); // Tính từ Range
        
        for (int i = 0; i < explosionCount; i++)
        {
            // Tính vị trí explosion (dọc theo hướng nhắm, cách nhau 0.5f)
            Vector3 explosionPos = transform.position + slashDirection * (explosionSpacing * (i + 1));
            
            // Lấy explosion từ pool
            ExplosionPool.Instance.Spawn(explosionPrefab, explosionPos , GetDamage() );
            
            // Tạo delay nhỏ giữa các explosion (0.1s mỗi lần)
            yield return new WaitForSeconds(0.1f);
        }

        // === PHASE 3: KẾT THÚC ===
        isSlashing = false;
        animator.SetBool("Attack", false);
        
        // Delay nhỏ trước khi có thể di chuyển lại
        yield return new WaitForSeconds(0.3f);
        
        isAttacking = false;
    }
    
    
    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines(); 

        
        isAttacking = false;
        isSlashing = false;

        if(animator != null)
        {
            animator.SetBool("Aim", false);
            animator.SetBool("Attack", false);
        }

    }

    
}