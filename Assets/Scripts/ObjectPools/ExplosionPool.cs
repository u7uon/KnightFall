using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    public static ExplosionPool Instance { get; private set; }
    const int INITSIZE = 10;
    const int MAXSIZE = 30;

    [System.Serializable]
    public class ExplosionPoolData
    {
        public Explosion explosionPrefab;
        [HideInInspector] public GenericObjectPool<Explosion> pool;
    }

    [SerializeField] private ExplosionPoolData[] explosionPoolsData;
    [SerializeField] private Transform poolParent;

    private Dictionary<string, GenericObjectPool<Explosion>> pools = new();
    private List<Explosion> activeExplosions = new();

    void Awake()
    {
        // Singleton pattern
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
            GameObject parent = new GameObject("ExplosionPoolParent");
            poolParent = parent.transform;
            poolParent.SetParent(transform);
        }

        // Chỉ khởi tạo pool cho các loại explosion được thiết lập sẵn
        foreach (var data in explosionPoolsData)
        {
            if (data.explosionPrefab != null)
            {
                CreatePoolForExplosion(data.explosionPrefab, INITSIZE);
            }
        }
    }

    // Tạo pool mới nếu chưa tồn tại (lazy initialization)
    private void CreatePoolForExplosion(Explosion explosionPrefab, int initialSize = 10)
    {
        string key = explosionPrefab.name;

        if (!pools.ContainsKey(key))
        {
            GenericObjectPool<Explosion> newPool = new GenericObjectPool<Explosion>(
                explosionPrefab,
                INITSIZE,
                MAXSIZE,
                poolParent
            );
            pools[key] = newPool;
        }
    }

    /// <summary>
    /// Spawn explosion từ pool. Tự động tạo pool mới nếu chưa tồn tại.
    /// </summary>
    public Explosion Spawn(Explosion explosionPrefab, Vector3 pos, float dmg)
    {
        string key = explosionPrefab.name;

        // Tạo pool mới nếu chưa có (lazy initialization)
        if (!pools.ContainsKey(key))
        {
            CreatePoolForExplosion(explosionPrefab, INITSIZE);
        }

        Explosion explosion = pools[key].Get();
        explosion.transform.position = pos;
        explosion.SetUp(pos, dmg);
        explosion.gameObject.SetActive(true);

        activeExplosions.Add(explosion);
        return explosion;
    }

    /// <summary>
    /// Trả explosion về pool
    /// </summary>
    public void Return(Explosion explosion)
    {
        if (explosion == null) return;

        activeExplosions.Remove(explosion);

        string key = explosion.name.Replace("(Clone)", "").Trim();

        if (pools.ContainsKey(key))
        {
            pools[key].ReturnToPool(explosion);
        }
        else
        {
            Destroy(explosion.gameObject);
        }
    }

    /// <summary>
    /// Xóa tất cả explosions đang active
    /// </summary>
    public void Clear()
    {
        List<Explosion> explosionsToReturn = new List<Explosion>(activeExplosions);

        foreach (Explosion explosion in explosionsToReturn)
        {
            if (explosion != null)
            {
                Return(explosion);
            }
        }

        activeExplosions.Clear();
    }

    public void ClearUnUsedPool(Explosion explosionPrefab)
    {
        string key = explosionPrefab.name;

        if (pools.ContainsKey(key))
        {
            pools[key].Clear();
        }
    }

    /// <summary>
    /// Đếm số lượng explosions đang active
    /// </summary>
    public int GetActiveExplosionCount()
    {
        activeExplosions.RemoveAll(e => e == null);
        return activeExplosions.Count;
    }

    /// <summary>
    /// Lấy danh sách explosions đang active
    /// </summary>
    public List<Explosion> GetActiveExplosions()
    {
        return new List<Explosion>(activeExplosions);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


}