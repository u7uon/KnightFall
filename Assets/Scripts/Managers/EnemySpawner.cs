using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class để config spawn weights cho từng tier
/// </summary>
[System.Serializable]
public class TierSpawnWeights
{
    [Tooltip("Weight cho Tier 1 (Higher = More frequent)")]
    [Range(1f, 100f)] public float tier1 = 40f;
    
    [Tooltip("Weight cho Tier 2")]
    [Range(1f, 100f)] public float tier2 = 20f;
    
    [Tooltip("Weight cho Tier 3")]
    [Range(1f, 100f)] public float tier3 = 15f;
    
    [Tooltip("Weight cho Tier 4")]
    [Range(1f, 100f)] public float tier4 = 15f;
    
    [Tooltip("Weight cho Tier 5")]
    [Range(1f, 100f)] public float tier5 = 10f;
    
    public float GetWeight(int tierIndex)
    {
        switch (tierIndex)
        {
            case 0: return tier1;
            case 1: return tier2;
            case 2: return tier3;
            case 3: return tier4;
            case 4: return tier5;
            default: return 1f;
        }
    }
    

}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    private Enemy[] enemyPrefabs;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private Transform topLeftCorner;
    [SerializeField] private Transform bottomRightCorner;
    
    [Header("Weighted Random Settings")]
    [SerializeField] private TierSpawnWeights spawnWeights = new TierSpawnWeights();
    
    [Header("Boss Settings")]
    [SerializeField] private Transform bossSpawnPosition;
    private Enemy bossPrefab;
    private bool isBossAlive = false ;

    [Header("Spawn Limits")]
    private const int MAX_ENEMY_LIMIT = 150;
    private const int SPAWN_COUNT = 4;
    
    private List<Enemy> availableEnemies = new List<Enemy>();
    private Transform PlayerPos ; 
    private EnemyPool enemyPool;
    private float healthMultiplier = 1f;
    private float damageMultiplier = 1f;
    private Coroutine spawnCoroutine;
    private int currentStage = 1;
    private int currentMapIndex = 0;
    private bool isSpawning = false;
    private StageManager stageManager;
    
    private Dictionary<int, int> spawnedTierCounts = new Dictionary<int, int>();

    void Awake()
    {
        enemyPool = GetComponent<EnemyPool>();
        if (enemyPool == null)
        {
            enemyPool = gameObject.AddComponent<EnemyPool>();
        }

        
        Boss.OnBossDie += OnBossDie ; 
        EnsureStageManagerReference();
    }

    void OnEnable()
    {
        PlayerPos = FindAnyObjectByType<PlayerController>().transform; 
        PlayerStatsManager.OnPlayerDie  +=  Stop ; 
    }




    private void EnsureStageManagerReference()
    {
        if (stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
        }
    }

    /// <summary>
    /// 🎯 BUILD TILE CACHE - Tạo danh sách tất cả vị trí có tile
    /// Gọi 1 lần duy nhất khi set map mới để tăng performance
    /// </summary>


    /// <summary>
    /// 🎲 RANDOM SPAWN POSITION - Lấy vị trí ngẫu nhiên trong Tilemap
    /// Method 2: Không dùng cache (tiết kiệm bộ nhớ, phù hợp map nhỏ hoặc map thay đổi thường xuyên)
    /// </summary>
    private Vector3 GetRandomPositionFromCorners()
    {
        float minX = Mathf.Min(topLeftCorner.position.x, bottomRightCorner.position.x);
        float maxX = Mathf.Max(topLeftCorner.position.x, bottomRightCorner.position.x);
        
        float minY = Mathf.Min(topLeftCorner.position.y, bottomRightCorner.position.y);
        float maxY = Mathf.Max(topLeftCorner.position.y, bottomRightCorner.position.y);
        
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        
        return  new Vector3(randomX, randomY , 0f);;
    }

    /// <summary>
    /// 🎯 GET RANDOM SPAWN POSITION - Method chính để lấy vị trí spawn
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        const int MAX_ATTEMPTS = 50; 
        const float MIN_DISTANCE = 0.5f; 
        
        for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
        {
            Vector3 pos = GetRandomPositionFromCorners();
            if (PlayerPos != null)
            {
                float distance = Vector3.Distance(PlayerPos.position, pos);
                
                if (distance >= MIN_DISTANCE)
                {
                    return pos; 
                }
            }
            else
            {
                return pos; 
            }
        }
        
        return GetRandomPositionFromCorners();
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
        if((currentStage-1) % 5 == 0 )
        {
            spawnInterval = Mathf.Max(0.1f, spawnInterval - 0.2f);
        }

        int newPhaseIndex = (stage - 1) / 5;
        
        if (newPhaseIndex != currentMapIndex)
        {
            currentMapIndex = newPhaseIndex;
        }

        UpdateAvailableEnemies();

        if (stage > 1)
        {
            StopSpawning();
            StartCoroutine(DelayStart(stage));
        }
        else
        {
            StartSpawning();
        }
    }

    private void UpdateAvailableEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            availableEnemies.Clear();
            return;
        }

        availableEnemies.Clear();
        spawnedTierCounts.Clear();

        int stageInPhase = ((currentStage - 1) % 5) + 1;
        int maxTierUnlocked = stageInPhase;

        for (int tier = 1; tier <= maxTierUnlocked; tier++)
        {
            int enemyIndex = tier - 1;
            
            if (enemyIndex < enemyPrefabs.Length && enemyPrefabs[enemyIndex] != null)
            {
                availableEnemies.Add(enemyPrefabs[enemyIndex]);
                spawnedTierCounts[enemyIndex] = 0;
            }
        }
    }

    private Enemy SelectWeightedEnemy()
    {
        int tierCount = availableEnemies.Count;
        
        if (tierCount == 0) return null;
        if (tierCount == 1) return availableEnemies[0];
        
        float totalWeight = 0f;
        for (int i = 0; i < tierCount; i++)
        {
            totalWeight += spawnWeights.GetWeight(i);
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        
        for (int i = 0; i < tierCount; i++)
        {
            cumulativeWeight += spawnWeights.GetWeight(i);
            
            if (randomValue <= cumulativeWeight)
            {
                spawnedTierCounts[i]++;
                return availableEnemies[i];
            }
        }
        
        spawnedTierCounts[0]++;
        return availableEnemies[0];
    }



    private IEnumerator DelayStart(int stage)
    {
        EnsureStageManagerReference();
        ClearAllEnemies();
        
        yield return new WaitForSeconds(1f);
        
        if (stageManager != null)
        {
            stageManager.OnStageTransitionComplete();
        }
        
        StartSpawning();

        if (stage % 5 == 0)
        {
            yield return new WaitForSeconds(1f);
            CallBoss();
        }
    }

    public void SetSpawnRate(float rate)
    {
        spawnInterval = Mathf.Max(0.1f, rate);
        
        if (isSpawning)
        {
            StopSpawning();
            StartSpawning();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        isSpawning = true;

        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentEnemyCount = enemyPool.GetActiveEnemyCount();
            if (currentEnemyCount >= MAX_ENEMY_LIMIT)
                enemyPool.DespawnRandomEnemies(SPAWN_COUNT);

            for (int i = 0; i < SPAWN_COUNT; i++)
            {
                var pos = GetRandomSpawnPosition();
                SpawnPointPool.Instance.Spawn(pos);

                // start a non-blocking coroutine for this single spawn
                // you can use same delay for all or add small stagger: i * 0.05f
                float delay = 0.8f; // or 0.8f + i * 0.05f to slightly stagger
                StartCoroutine(SpawnSingleDelayed(pos, delay));
            }
        }
    }

private IEnumerator SpawnSingleDelayed(Vector3 pos, float delay)
{
    yield return new WaitForSeconds(delay);

    // check flag in case spawning stopped meanwhile
    if (!isSpawning) yield break;

    SpawnRandomEnemy(pos);
}

    /// <summary>
    /// 🎯 SPAWN RANDOM ENEMY - Spawn enemy tại vị trí ngẫu nhiên trong Tilemap
    /// </summary>
    private void SpawnRandomEnemy(Vector3 pos )
    {
        if (availableEnemies == null || availableEnemies.Count == 0)
        {
            return;
        }

        // 🎯 Weighted random selection
        Enemy enemyToSpawn = SelectWeightedEnemy();
        
        if (enemyToSpawn == null)
        {
            return;
        }

        // Spawn enemy
        Enemy spawnedEnemy = enemyPool.SpawnEnemy(
            enemyToSpawn,
            pos,
            healthMultiplier,
            damageMultiplier
        );
    }

    private void CallBoss()
    {
        if (bossPrefab == null)
        {
            return;
        }

        if (bossSpawnPosition == null)
        {
            bossSpawnPosition = transform;
        }


        Instantiate(bossPrefab, bossSpawnPosition.position, Quaternion.identity);
        isBossAlive  = true;
    }

    public void StartSpawning()
    {
        if (isSpawning)
        {
            return;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    public void StopSpawning()
    {
        isSpawning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

    }

    public void ClearAllEnemies()
    {
        if (enemyPool != null)
        {
            enemyPool.ClearAllEnemies();
        }
    }


    private void Stop()
    {
        ClearAllEnemies();
        StopSpawning();
    }

    /// <summary>
    /// 🎯 SET ENEMIES - Set enemies và tilemap cho giai đoạn mới
    /// Được gọi mỗi 5 stage (stage 1, 6, 11, 16, 21, 26)
    /// </summary>
    public void SetEnemies( Enemy[] enemies, Enemy bossPrefab)
{
    if (enemies == null || enemies.Length == 0)
    {
        return;
    }



    if (bossPrefab != null)
    {
        this.bossPrefab = bossPrefab;
    }

    enemyPrefabs = enemies;

    enemyPool.ClearOldPools();
    enemyPool.InitializePools(enemyPrefabs);
    UpdateAvailableEnemies();
    
}

    private void LogSpawnStatistics()
    {
        int totalSpawned = 0;
        foreach (var count in spawnedTierCounts.Values)
        {
            totalSpawned += count;
        }
        
        if (totalSpawned == 0) return;
        
        string stats = $"[EnemySpawner] Spawn Statistics (Stage {currentStage}):\n";
        stats += $"  Total Spawned: {totalSpawned}\n";
        
        for (int i = 0; i < availableEnemies.Count; i++)
        {
            int count = spawnedTierCounts[i];
            float percentage = (count / (float)totalSpawned) * 100f;
            stats += $"  Tier {i + 1}: {count} ({percentage:F1}%)\n";
        }
        
    }

    // Public getters
    public int GetCurrentEnemyCount()
    {
        return enemyPool != null ? enemyPool.GetActiveEnemyCount() : 0;
    }

    public bool IsBossAlive() => isBossAlive ;

    public void OnBossDie() => isBossAlive = false ;

    void OnDestroy()
    {
        StopSpawning();
        PlayerStatsManager.OnPlayerDie  -=  Stop ; 
        Boss.OnBossDie -= OnBossDie ; 
    }
}