
using System;
using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    public static Action<float> OnBossSpawn; 
    public static Action<float> OnHpChange ; 

    public static Action OnBossDie ; 


    private float lastSkillTime;
    protected override void Update()
    {
        base.Update() ; 

        if (!isDied && Time.time >= lastSkillTime + enemyStats.Cooldown)
        {
            TryAttack();
            lastSkillTime = Time.time;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnBossSpawn?.Invoke(GetMaxHealth());
    }



    protected override void TryAttack()
    {
        if(isDied || isAttacking) return ;
        DoRandomSkill();
    }

    public override void TakeDamage(float damage, bool isCrit = false)
    {
        base.TakeDamage(damage, isCrit);

        OnHpChange?.Invoke(GetCurrentHealth());

    }

    public override void TakeEffectDamage(float damage, Color visualDamageColor)
    {
        base.TakeEffectDamage(damage, visualDamageColor);
        
        OnHpChange?.Invoke(GetCurrentHealth());
    }

    protected override void Die()
    {
        isDied = true; 
        StartCoroutine(BossDieCoroutine());
    }

    private IEnumerator BossDieCoroutine()
    {
         if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        yield return new WaitForSeconds(2f);
        
        DropLoot();
        DropExp(); 
        OnBossDie?.Invoke() ; 
        Destroy(gameObject);
    }

    protected virtual void DoRandomSkill()
    {
        int randSkill = UnityEngine.Random.Range(1,4); 

        switch(randSkill)
        {
            case 1 : 
                Skill1();
                break;
            case 2 : 
                Skill2(); 
                break ; 
            case 3 : 
                Skill3() ; 
                break; 
        }

    }

    protected override void DropLoot()
    {
        var inven = FindAnyObjectByType<PlayerInventory>() ; 
        var stage = FindAnyObjectByType<StageManager>(); 
        if(inven != null && stage !=null)
        {
            inven.AddCoin((int)enemyStats.Coin * stage.GetPhase()  );
        }
    }

     protected virtual void Skill1() {}

     protected virtual void Skill2() {}

     protected virtual void Skill3(){ }
    



}