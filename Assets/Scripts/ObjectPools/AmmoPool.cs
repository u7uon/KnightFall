using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }
    const  int INITSIZE = 5;
    const int MAXSIZE = 50 ; 

    [System.Serializable]
    public class BulletPoolData
    {
        public Bullet bulletPrefab;
        [HideInInspector] public GenericObjectPool<Bullet> pool;
    }

    [SerializeField] private BulletPoolData[] bulletPoolsData;
    [SerializeField] private Transform poolParent;

    private Dictionary<string, GenericObjectPool<Bullet>> pools = new();
    private List<Bullet> activeBullets = new();

    void Awake()
    {
        // Singleton pattern để Player và Enemy có thể truy cập chung
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        if (poolParent == null)
        {
            GameObject parent = new GameObject("BulletPoolParent");
            poolParent = parent.transform;
            poolParent.SetParent(transform);
        }

        // Chỉ khởi tạo pool cho các loại đạn được thiết lập sẵn
        foreach (var data in bulletPoolsData)
        {
            if (data.bulletPrefab != null)
            {
                CreatePoolForBullet(data.bulletPrefab, INITSIZE);
            }
        }
    }

    // Tạo pool mới nếu chưa tồn tại (lazy initialization)
    private void CreatePoolForBullet(Bullet bulletPrefab, int initialSize = 10)
    {
        string key = bulletPrefab.name;

        if (!pools.ContainsKey(key))
        {
            GenericObjectPool<Bullet> newPool = new GenericObjectPool<Bullet>(
                bulletPrefab,
                INITSIZE,
                MAXSIZE,
                poolParent
            );
            pools[key] = newPool;
        }
    }

    /// <summary>
    /// Spawn bullet từ pool. Tự động tạo pool mới nếu chưa tồn tại.
    /// </summary>
    public Bullet SpawnBullet(Bullet bulletPrefab, Vector3 position, Vector3 targetPosition, float damage , List<EffectConfig> e = null ,bool canCrit = false)
    {
        string key = bulletPrefab.name;

        // Tạo pool mới nếu chưa có (lazy initialization)
        if (!pools.ContainsKey(key))
        {
            CreatePoolForBullet(bulletPrefab, 10);
        }

        Bullet bullet = pools[key].Get();
        bullet.transform.position = position;
        bullet.SetPool(pools[key]);
        bullet.Direction(targetPosition, damage,e , canCrit);

        activeBullets.Add(bullet);
        return bullet;
    }

    /// <summary>
    /// Trả bullet về pool
    /// </summary>
    public void ReturnBullet(Bullet bullet)
    {
        if (bullet == null) return;

        activeBullets.Remove(bullet);

        string key = bullet.name.Replace("(Clone)", "").Trim();

        if (pools.ContainsKey(key))
        {
            pools[key].ReturnToPool(bullet);
        }
        else
        {
            Destroy(bullet.gameObject);
        }
    }

    /// <summary>
    /// Xóa tất cả bullets đang active (dùng khi clear màn, boss fight, etc.)
    /// </summary>
    public void ClearAllBullets()
    {
        List<Bullet> bulletsToReturn = new List<Bullet>(activeBullets);

        foreach (Bullet bullet in bulletsToReturn)
        {
            if (bullet != null)
            {
                ReturnBullet(bullet);
            }
        }

        activeBullets.Clear();
    }

    public void  DestroyUnusedPools(Bullet bulletPrefab)
    {
        string key = bulletPrefab.name;

        if (pools.ContainsKey(key))
        {
            pools[key].Clear();
            pools.Remove(key);
        }
        
    }

    /// <summary>
    /// Đếm số lượng bullets đang active
    /// </summary>
    public int GetActiveBulletCount()
    {
        activeBullets.RemoveAll(b => b == null);
        return activeBullets.Count;
    }

    /// <summary>
    /// Lấy danh sách bullets đang active (để check collision, effects, etc.)
    /// </summary>
    public List<Bullet> GetActiveBullets()
    {
        return new List<Bullet>(activeBullets);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}