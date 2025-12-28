using UnityEngine;

public class SkillIndicatorPool : MonoBehaviour
{
    public static SkillIndicatorPool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private SkillIndicator indicatorPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 50;
    [SerializeField] private Transform poolParent;

    private GenericObjectPool<SkillIndicator> pool;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Tạo prefab nếu chưa có
        if (indicatorPrefab == null)
        {
            indicatorPrefab = CreateDefaultPrefab();
        }

        // Tạo pool parent nếu chưa có
        if (poolParent == null)
        {
            poolParent = new GameObject("SkillIndicators").transform;
            poolParent.SetParent(transform);
        }

        // Khởi tạo pool
        pool = new GenericObjectPool<SkillIndicator>(
            indicatorPrefab,
            initialPoolSize,
            maxPoolSize,
            poolParent
        );

    }

    /// <summary>
    /// Spawn indicator với anchor position và aim direction
    /// </summary>
    public SkillIndicator SpawnIndicator(
        Vector3 anchorPosition,
        Vector3 aimDirection,
        SkillIndicator.IndicatorShape shape,
        Vector2 size,
        Color color,
        float duration)
    {
        SkillIndicator indicator = pool.Get();

        if (indicator == null)
        {
            return null;
        }

        indicator.Setup(anchorPosition, aimDirection, shape, size, color, duration);
        return indicator;
    }

    /// <summary>
    /// Spawn circle indicator - không cần aim direction (đối xứng)
    /// </summary>
    public SkillIndicator SpawnCircle(Vector3 position, float radius, Color color, float duration = 2f)
    {
        return SpawnIndicator(position, Vector3.zero, SkillIndicator.IndicatorShape.Circle, 
            new Vector2(radius, radius), color, duration);
    }

    /// <summary>
    /// Spawn rectangle indicator - anchor ở giữa cạnh, mở rộng theo aimDirection
    /// </summary>
    public SkillIndicator SpawnRectangle(Vector3 anchorPosition, Vector3 aimDirection, float width, float height, Color color, float duration = 2f)
    {
        return SpawnIndicator(anchorPosition, aimDirection, SkillIndicator.IndicatorShape.Rectangle, 
            new Vector2(width, height), color, duration);
    }

    /// <summary>
    /// Spawn cone indicator - đỉnh ở anchor, mở rộng theo aimDirection
    /// </summary>
    public SkillIndicator SpawnCone(Vector3 anchorPosition, Vector3 aimDirection, float radius, float angle, Color color, float duration = 2f)
    {
        return SpawnIndicator(anchorPosition, aimDirection, SkillIndicator.IndicatorShape.Cone, 
            new Vector2(radius, angle), color, duration);
    }

    /// <summary>
    /// Spawn ring indicator - không cần aim direction (đối xứng)
    /// </summary>
    public SkillIndicator SpawnRing(Vector3 position, float innerRadius, float outerRadius, Color color, float duration = 2f)
    {
        return SpawnIndicator(position, Vector3.zero, SkillIndicator.IndicatorShape.Ring, 
            new Vector2(innerRadius, outerRadius), color, duration);
    }

    /// <summary>
    /// Trả indicator về pool
    /// </summary>
    public void ReturnIndicator(SkillIndicator indicator)
    {
        if (indicator != null)
        {
            pool.ReturnToPool(indicator);
        }
    }

    /// <summary>
    /// Clear toàn bộ pool (gọi khi đổi map/scene)
    /// </summary>
    public void ClearPool()
    {
        pool.Clear();
    }

    /// <summary>
    /// Tạo prefab mặc định nếu chưa assign
    /// </summary>
    private SkillIndicator CreateDefaultPrefab()
    {
        GameObject obj = new GameObject("SkillIndicatorPrefab");
        SkillIndicator indicator = obj.AddComponent<SkillIndicator>();
        
        // Ẩn prefab
        obj.SetActive(false);
        obj.hideFlags = HideFlags.HideInHierarchy;
        
        return indicator;
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


}