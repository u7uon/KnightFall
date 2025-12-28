using System.Collections.Generic;
using UnityEngine;

public class DamagePopupPool : MonoBehaviour
{
    public static DamagePopupPool Instance;

    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private int poolSize = 50;
    [SerializeField] private Canvas canvas;

    private readonly Queue<DamagePopup> pool = new();
    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;

        // Tự động tìm Canvas trong scene nếu chưa assign
        if (canvas == null)
        {
            // Thử tìm Canvas cha trước
            canvas = GetComponentInParent<Canvas>();
            
            // Nếu không có, tìm Canvas đầu tiên trong scene
            if (canvas == null)
                canvas = FindFirstObjectByType<Canvas>();
            
            // Nếu vẫn không có, tạo warning
            if (canvas == null)
            {
                return;
            }
        }

        // Tự động set Render Camera cho Canvas nếu chưa có
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            if (canvas.worldCamera == null)
                canvas.worldCamera = cam;
        }

        // Tạo pool - spawn popup VÀO TRONG CANVAS thay vì vào DamagePopupPool
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(popupPrefab, canvas.transform); // Spawn vào Canvas
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public DamagePopup Get()
    {
        if (pool.Count == 0)
        {
            var extra = Instantiate(popupPrefab, transform);
            extra.gameObject.SetActive(false);
            return extra;
        }

        return pool.Dequeue();
    }

    public void ReturnToPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(false);
        pool.Enqueue(popup);
    }

    public void Spawn(Vector3 worldPos, float damage, Color color)
    {
        var popup = Get();
        
        // Setup popup TRƯỚC KHI set position
        popup.Setup(damage, color);
        
        // Chuyển world position sang screen position
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        
        // Kiểm tra nếu Canvas là Overlay hoặc Camera
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Với Screen Space Overlay, dùng screen position trực tiếp
            popupRect.position = screenPos;
        }
        else
        {
            // Với Screen Space Camera
            Camera canvasCam = canvas.worldCamera != null ? canvas.worldCamera : cam;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvasCam,
                out Vector2 localPoint
            );

            popupRect.localPosition = localPoint;
        }

        // BẬT popup SAU KHI đã set position
        popup.gameObject.SetActive(true);
    }
}