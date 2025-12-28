using UnityEngine;

public class Explosion : MonoBehaviour
{

    private float damage = 0 ; 

    public void SetDamage( float dmg)
    {
        damage = dmg; 
    }

    public void SetUp(Vector3 pos , float dmg)
    {
        transform.position = pos;
        damage = dmg;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerStatsManager.Instance.TakeDame(damage);
        }
    }
    public void Disapear()
    {
        ExplosionPool.Instance.Return(this);
    }
}
