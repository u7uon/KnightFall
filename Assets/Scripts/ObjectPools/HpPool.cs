using UnityEngine;

public class HpPool : MonoBehaviour
{
    public static HpPool instance ; 


    private GenericObjectPool<HPItem> pool ; 

    [SerializeField] private HPItem hpPrefab ;
    void Awake()
    {   
        instance = this ; 

        pool = new GenericObjectPool<HPItem>(hpPrefab,5,30, this.transform );
        
    }
    
    private HPItem Get() => pool.Get();

    public void Spawn(Vector3 pos)
    {
        var hp = Get() ; 

        hp.gameObject.SetActive(true); 

        hp.transform.position = pos ; 

    }

    public void Return(HPItem hp)
    {
        if(hp != null) pool.ReturnToPool(hp); 
    }


    public void Clear()
    {
        pool.Clear() ; 
    }


}