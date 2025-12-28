using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    private GenericObjectPool<Bullet> pool;
    protected float damage = 0 ;
    protected List<EffectConfig> effectDatas ; 
    private float lifeTime = 5f;
    protected Vector3 direction;
    [SerializeField]protected float Speed = 15f;
    protected bool hasHit = false ;

    protected bool canCrit = false ; 

    public float GetDamge() => damage; 

    public void SetPool(GenericObjectPool<Bullet> p)
    {
        pool = p;
    }

    protected virtual void  OnTriggerEnter2D(Collider2D collision)
    {
        
    }
    
    void Update()
    {
        if(direction == Vector3.zero) return;
        transform.position += direction * Time.deltaTime * Speed;
    }

    public void Direction(Vector3 vector3 , float Damage ,List<EffectConfig> e = null,  bool canCrit = false  )
    {
        direction = (vector3 - transform.position).normalized;
        damage = Damage;
        this.canCrit = canCrit ; 
        effectDatas = e; 
        // Xoay bullet theo hướng bắn
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }
    
    protected void ReturnToPool()
    {
        if (this != null && pool != null)
        {
             pool.ReturnToPool(this);
             hasHit = false ;
        }
           
    }
}