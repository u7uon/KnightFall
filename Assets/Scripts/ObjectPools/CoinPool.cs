// ============ CoinPool.cs ============
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab;
    const  int INITSIZE = 5;
    const int MAXSIZE = 100 ; 

    private List<Coin> activeCoins;
    private GenericObjectPool<Coin> coinPool;

    void Awake()
    {
        coinPool = new GenericObjectPool<Coin>(coinPrefab, INITSIZE,MAXSIZE, transform);
        activeCoins = new List<Coin>();
    }

    void Start()
    {
        StageManager.OnStageEnd += FlyAllCoins;
    }

    public Coin GetCoin()
    {
        Coin coin = coinPool.Get();
        if(coin == null)
           coin =  Instantiate(coinPrefab,transform);
        activeCoins.Add(coin);
        return coin;
    }

    public void ReturnToPool(Coin c)
    {
        if (c == null) return;
        activeCoins.Remove(c);
        coinPool.ReturnToPool(c);
    }

    public List<Coin> GetActiveCoins()
    {
        // Clean up null references trước khi return
        activeCoins.RemoveAll(coin => coin == null);
        return new List<Coin>(activeCoins); // Return copy để tránh modification
    }

    public void FlyAllCoins(int i)
    {
        // Tạo copy để tránh modification during iteration
        List<Coin> coinsToFly = new List<Coin>(activeCoins);

        foreach (var coin in coinsToFly)
        {
            if (coin != null && coin.gameObject.activeInHierarchy)
            {
                ReturnToPool(coin);
            }
        }
    }

    void OnDestroy()
    {
        StageManager.OnStageEnd -= FlyAllCoins;
    }
}