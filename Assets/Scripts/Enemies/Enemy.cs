using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyStatsData enemyStats;
    
    [Header("Enemy Type")]
    [SerializeField] private bool isBoss = false;
    [SerializeField] private bool isElite = false;
    protected bool isAttacking = false ; 
    private CoinPool coinPool;
    public bool IsBoss => isBoss;
    public bool IsElite => isElite;
    private float collisionCoolDown  = 1f; 
    private bool isHit = false ; 
    protected bool isDied = false ; 
    // Event khi enemy chết
    public event Action<Enemy> OnEnemyDeath;

    protected float currentHealth;
    private SpriteRenderer spriteRenderer;
    protected PlayerController player;
    protected Animator animator;

    private StageManager stageManager; 

    protected float Health;
    protected float Speed;
    protected float Damage;

    protected virtual void TryAttack() { }


    protected virtual void Start()
    {
        SetDefaultEnemyStats();
        stageManager = FindAnyObjectByType<StageManager>();
        coinPool = FindAnyObjectByType<CoinPool>();
        player = FindAnyObjectByType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = Health;
    }

    // Được gọi khi enemy được spawn từ pool
    protected virtual void OnEnable()
    {
        if (enemyStats != null)
        {
            SetDefaultEnemyStats();
            currentHealth = Health;
            isDied = false; 
        }

    }

    protected virtual void SetDefaultEnemyStats()
    {
        var StageManager = FindAnyObjectByType<StageManager>();
        if(StageManager == null )
        {
            Health = enemyStats.Health ;
            Speed = enemyStats.Speed ;
            Damage = enemyStats.Damage ;
        }
        else
        {
            int phase = StageManager.GetPhase();
            Health = enemyStats.Health * (phase <= 1 ? 1f : 1f + 3f * (phase - 1));
            Speed = enemyStats.Speed ;
            Damage = enemyStats.Damage  * Mathf.Max(1f , phase-0.5f) ;
        }
    }

    protected virtual void Update()
    {
        if (!isDied && !isAttacking ) MoveTowardsPlayer();
    }

    public virtual void TakeDamage(float damage , bool isCrit = false)  
    {
        if (isDied) return;

        currentHealth -= damage;
        DamagePopupPool.Instance.Spawn(transform.position, damage , isCrit ? Color.red : Color.white);
        AudioManager.Instance.PlaySFX("hurt");
        if (animator != null && !isAttacking)
        {
            animator.SetTrigger("Hurt");
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void TakeEffectDamage(float damage , Color visualDamageColor )
    {
        if (isDied) return;

        currentHealth -= damage;
        DamagePopupPool.Instance.Spawn(transform.position, damage , visualDamageColor);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Despawn()
    {
        isDied = true; 
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Invoke(nameof(DieCroutine), 0.5f); 
     
    }

    protected virtual void Die()
    {
        isDied = true; 
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        Invoke(nameof(DieCroutine), 1f);
        DropLoot();
        DropExp(); 
    }

    private void DieCroutine()
    {
        // Trigger death event để EnemyPool biết
        OnEnemyDeath?.Invoke(this);
    }

    protected virtual void DropLoot()
    {
        if (coinPool != null)
        {
            // Lấy coin từ pool và đặt tại vị trí enemy chết
            Coin coin = coinPool.GetCoin();
            if(stageManager == null ) stageManager = FindAnyObjectByType<StageManager>();

            coin.SetValue((int)enemyStats.Coin);
            coin.transform.position = transform.position;

        }
        if( UnityEngine.Random.Range(0f ,1f) < 0.05f )
            DropHP();

    }

    private void DropHP()
    {
        HpPool.instance.Spawn(this.transform.position);
    }
    
    protected virtual void DropExp()
    {
        FindAnyObjectByType<PlayerLevel>()?.AddXp(enemyStats.Exp);
    }

    protected void MoveTowardsPlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                Speed * Time.deltaTime
            );
            Flip();
        }
    }

    protected void Flip()
    {
        if (player != null && spriteRenderer != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isDied && !isHit)
        {
            if (player != null)
            {
                PlayerStatsManager.Instance.TakeDame(Damage);
                isHit = true;
                Invoke(nameof(ResetCoolDown),collisionCoolDown);
            }
        }
    }

    void ResetCoolDown()
    {
        isHit = false;
    }

    // Clean up khi bị disable
    protected virtual void OnDisable()
    {
        // Reset các trigger của animator
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.ResetTrigger("Die");
        }
    }

    // Public methods để lấy thông tin
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => Health;
    public float GetDamage() => Damage;
    public float GetSpeed() => Speed;
    public float SetSpeed(float s) => this.Speed = s;

    public void SetMaxHealth(float health)
    {
        this.Health = health;
        this.currentHealth = health;
    }

    public bool IsAlive() => !isDied ; 

}