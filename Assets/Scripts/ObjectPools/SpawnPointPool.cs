// ============ CoinPool.cs ============
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointPool : MonoBehaviour
{
    public static SpawnPointPool Instance;

    [SerializeField] private SpawnPoint spawnPointPrefab;
    const  int INITSIZE = 5;
    const int MAXSIZE = 20 ; 
    private GenericObjectPool<SpawnPoint> coinPool;

    void Awake()
    {
        Instance = this;
        coinPool = new GenericObjectPool<SpawnPoint>(spawnPointPrefab, INITSIZE,MAXSIZE, transform);
    }


    public SpawnPoint Get()
    {
        SpawnPoint coin = coinPool.Get();
        return coin;
    }

    public void ReturnToPool(SpawnPoint c)
    {
        if (c == null) return;
        coinPool.ReturnToPool(c);
    }



    public void Spawn(Vector3 pos)
    {
        var s = Get(); 
        s.transform.position = pos ; 
        StartCoroutine(Dispose(s)  );
    }

    private IEnumerator Dispose(SpawnPoint s )
    {
        yield return new WaitForSeconds(0.6f);

        ReturnToPool(s); 
    }


}