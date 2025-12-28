using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    public class EnemyPoolData
    {
        public Enemy enemyPrefab;
        [HideInInspector] public GenericObjectPool<Enemy> pool;
    }

    const int INITSIZE = 5 ; 
    const int MAXSIZE = 100 ; 

    private List<EnemyPoolData> enemyPoolsData;
    [SerializeField] private Transform poolParent;

    private Dictionary<string, GenericObjectPool<Enemy>> pools = new();
    private List<Enemy> activeEnemies = new();


    public void InitializePools(Enemy[] newEnemies )
    {

        if (poolParent == null)
        {
            GameObject parent = new GameObject("PoolParent");
            poolParent = parent.transform;
            poolParent.SetParent(transform);
        }

        enemyPoolsData = newEnemies.Select( x => new EnemyPoolData
        {
            enemyPrefab = x ,
            pool =  new GenericObjectPool<Enemy>(
                    x,
                    INITSIZE,
                    MAXSIZE,
                    poolParent)
        } ).ToList(); 

        foreach (var data in enemyPoolsData)
        {
            string key = data.enemyPrefab.name;
            pools[key] = data.pool;
            
        }
    }

    public void ClearOldPools()
    {

        // Clear active enemies
        ClearAllEnemies();

        // Destroy all pools
        foreach (var pool in pools.Values)
        {
            pool.Clear();
        }
        
        pools.Clear();
        
        // Giải phóng RAM
        Resources.UnloadUnusedAssets();
    }


    private void CreateNewPool(Enemy e )
    {
        var newPool = new EnemyPoolData
        {
            enemyPrefab = e ,
            pool =  new GenericObjectPool<Enemy>(
                    e,
                    INITSIZE,
                    MAXSIZE,
                    poolParent)
        } ;

        enemyPoolsData.Add(newPool);     

        pools[e.name] = newPool.pool ; 
    }

    public Enemy SpawnEnemy(Enemy enemyPrefab, Vector3 position, float healthMult, float damageMult)
    {
        string key = enemyPrefab.name;

        if (!pools.ContainsKey(key))
        {
            CreateNewPool(enemyPrefab);
        }

        Enemy enemy = pools[key].Get();
        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.identity;
        
        enemy.OnEnemyDeath += HandleEnemyDeath;
        
        activeEnemies.Add(enemy);
        return enemy;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemy.OnEnemyDeath -= HandleEnemyDeath;
        ReturnEnemy(enemy);
    }

    public void ReturnEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        activeEnemies.Remove(enemy);
        
        string key = enemy.name.Replace("(Clone)", "").Trim();
        
        if (pools.ContainsKey(key))
        {
            pools[key].ReturnToPool(enemy);
        }
        else
        {
            Destroy(enemy.gameObject);
        }
    }

    public void DespawnRandomEnemies(int count)
    {
        if (activeEnemies == null ||  activeEnemies.Count == 0) return;

        count = Mathf.Min(count, activeEnemies.Count);

        // Despawn random enemies
        for (int i = 0; i < count; i++)
        {
            if (activeEnemies.Count == 0) break;

            int randomIndex = Random.Range(0, activeEnemies.Count);
            Enemy enemy = activeEnemies[randomIndex];
            
            // Don't despawn bosses or elites
            if (enemy != null && !enemy.IsBoss && !enemy.IsElite)
            {
                enemy.Despawn();
            }
        }
    }

    public void ClearAllEnemies()
    {
        // Create a copy to avoid modification during iteration
        List<Enemy> enemiesToClear = new List<Enemy>(activeEnemies);
        
        foreach (Enemy enemy in enemiesToClear)
        {
            if (enemy != null)
            {
                enemy.Despawn();
            }
        }
        
        activeEnemies.Clear();
    }

    public int GetActiveEnemyCount()
    {
        // Clean up any null references
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count;
    }

    public List<Enemy> GetActiveEnemies()
    {
        return new List<Enemy>(activeEnemies);
    }
}