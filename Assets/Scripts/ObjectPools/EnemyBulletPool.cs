using UnityEngine;

public class EnmyBulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    const  int INITSIZE = 5;
    const int MAXSIZE = 20 ; 


    private GenericObjectPool<Bullet> bulletPool;

    void Awake()
    {
        bulletPool = new GenericObjectPool<Bullet>(bulletPrefab, INITSIZE,MAXSIZE, transform);
    }

    // public Bullet SpawnBullet(Vector2 position, Vector2 direction)
    // {
    //     Bullet b = bulletPool.Get();
    //     b.transform.position = position;
    //     b.Direction(direction);

    //     // When bullet expires, it should call ReturnToPool
    //     b.SetPool(bulletPool);
    //     return b;
    // }
}
