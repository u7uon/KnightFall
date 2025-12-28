using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : Component
{
    private readonly Queue<T> pool = new();
    private readonly T prefab;
    private readonly Transform parent;
    private readonly int maxSize;
    private int currentSize = 0; 

    public GenericObjectPool(T prefab, int initialSize, int maxSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.maxSize = maxSize;

        // Pre-warm nhẹ
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            currentSize++;
        }
    }

    public T Get()
    {
        // Clean destroyed objects
        while (pool.Count > 0 && pool.Peek() == null)
        {
            pool.Dequeue();
            currentSize--;
        }

        // Có sẵn trong pool → dùng luôn
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            if (obj != null)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        // 🎯 LAZY CREATE: Chưa đạt max → tạo mới
        if (currentSize < maxSize)
        {
            T newObj = Object.Instantiate(prefab, parent);
            newObj.gameObject.SetActive(true);
            currentSize++;
            return newObj;
        }

        return null;
    }

    public void ReturnToPool(T obj)
    {
        if (obj == null) return;

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// 🧹 Clear toàn bộ pool - Destroy tất cả objects
    /// Gọi khi đổi map để giải phóng RAM
    /// </summary>
    public void Clear()
    {
        // Destroy tất cả objects trong pool (inactive)
        while (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }
        
        // Reset counter
        currentSize = 0;
        
    }

    // 📊 Debug helpers
    public int GetActiveCount() => currentSize - pool.Count;
    public int GetTotalCount() => currentSize;
    public int GetPooledCount() => pool.Count;
}