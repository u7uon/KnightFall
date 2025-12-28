using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private List<MapData> maps;
    [SerializeField] private Transform mapContainer;
    
    private MapData currentMap;
    private GameObject currentMapInstance; // Instance của map hiện tại
    // Track the last two maps to avoid repeating them on the next change
    private List<MapData> lastTwoMaps = new List<MapData>(2);

    void Start()
    {
        StageManager.OnStageStart += UpdateRandomMapOnStageChange;
        
        // no-op: selection will exclude the last two maps regardless of total count
        
        // Validate mapContainer
        if (mapContainer == null)
        {
            return;
        }
        
        ActiveRandomMap();
    }
    void OnDestroy()
    {
        StageManager.OnStageStart -= UpdateRandomMapOnStageChange;
    }

    void UpdateRandomMapOnStageChange(int currentStage)
    {
        // Đổi map mỗi 5 stage (stage 6, 11, 16, 21, 26)
        if ((currentStage - 1) % 5 == 0 && currentStage > 1)
        {
            ActiveRandomMap();
        }
    }

    void ActiveRandomMap()
    {
        // Build eligible list by excluding the previous two maps
        List<MapData> eligible = new List<MapData>(maps);
        eligible.RemoveAll(m => m == null);

        foreach (var m in lastTwoMaps)
        {
            eligible.Remove(m);
        }

        // If no eligible maps remain (e.g. total maps < 3), relax constraint to exclude only the current map
        if (eligible.Count == 0)
        {
            eligible = new List<MapData>(maps);
            eligible.RemoveAll(m => m == null);
            if (currentMap != null)
                eligible.Remove(currentMap);
        }

        // If still empty (e.g. maps count == 0 or 1), fall back to all maps
        if (eligible.Count == 0)
        {
            eligible = new List<MapData>(maps);
            eligible.RemoveAll(m => m == null);
        }

        // Correct Random.Range usage: max is exclusive
        MapData newMap = eligible[Random.Range(0, eligible.Count)];
        
        // Destroy map cũ
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
        }

        // Instantiate map mới vào container
        if (newMap.Map != null)
        {
            currentMapInstance = Instantiate(newMap.Map, mapContainer);
            currentMapInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currentMapInstance.SetActive(true) ; 
            currentMapInstance.name = newMap.MapName; // Đặt tên cho dễ debug

        }


        // Cập nhật current map
        // Update lastTwoMaps: push previous currentMap into history
        if (currentMap != null)
        {
            lastTwoMaps.Insert(0, currentMap);
            if (lastTwoMaps.Count > 2) lastTwoMaps.RemoveAt(lastTwoMaps.Count - 1);
        }

        currentMap = newMap;
        
        // Set enemies cho EnemySpawner
        EnemySpawner enemySpawner = FindAnyObjectByType<EnemySpawner>();
        if (enemySpawner != null && currentMap.Enemies != null)
        {
            enemySpawner.SetEnemies(currentMap.Enemies , currentMap.BossPrefab);
        }


        // No temporary list used anymore — selection is driven by `lastTwoMaps` constraints
    }

    // Helper method để get current map
    public MapData GetCurrentMap()
    {
        return currentMap;
    }

    // Helper method để force change map (for testing)
    public void ForceChangeMap()
    {
        ActiveRandomMap();
    }
}