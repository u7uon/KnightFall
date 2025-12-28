using UnityEngine;

public class HPItem : MonoBehaviour, ICollectable
{   

    private HpPool hpPool;
    private int value = 10;

    [Header("Fly Settings")]
    [SerializeField] private float flySpeed = 8f;

    private Transform player;
    private PlayerStatsManager playerStats;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            Collect();
    }



    void Awake()
    {
        hpPool = FindAnyObjectByType<HpPool>() ; 
        // Cache player reference một lần
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<PlayerStatsManager>();
        }
    }

    public void Collect()
    {
        if(playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStatsManager>();
        }

        playerStats.HealthUp(this.value);
        AudioManager.Instance.PlaySFX("healthUp");
        Return() ;
    }

    void Return()
    {
        if(hpPool !=null )
            hpPool.Return(this) ; 
        else
            Destroy(gameObject)  ;  
    }

    public void SlideTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 direction = (playerPosition - transform.position).normalized;
        transform.position += direction * flySpeed  * Time.deltaTime;
    }


}