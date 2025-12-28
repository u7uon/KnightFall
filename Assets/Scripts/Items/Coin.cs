using Unity.Mathematics;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    private CoinPool coinPool;
    private int value = 1;

    [Header("Fly Settings")]
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private float flyAcceleration = 15f;
    [SerializeField] private float rotationSpeed = 720f;

    private Transform player;
    private bool isFlying = false;
    private float currentSpeed;

    private PlayerStatsManager playerStats;
    private PlayerInventory playerInventory ; 

    void Awake()
    {

        coinPool = FindAnyObjectByType<CoinPool>() ; 
        // Cache player reference một lần
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerStats = playerObj.GetComponent<PlayerStatsManager>();
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        // Chỉ collect khi KHÔNG đang bay (tránh trigger 2 lần)
        if (!isFlying && collision.CompareTag("Player"))
        {
                Collect();
                AudioManager.Instance.PlaySFX("coin_collect");
        }
    }

    public void SetValue(int value = 1 ){ this.value= value;}

    public void Collect()
    {
        if(playerInventory == null)
            playerInventory = FindAnyObjectByType<PlayerInventory>();

        playerInventory.PickUpCoins(this.value);
        ReturnCoin();
    }

    private void ReturnCoin()
    {
        if (coinPool != null)
        {
            coinPool.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FlyToPlayer()
    {
        // Tìm lại player nếu bị null
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        isFlying = true;
        currentSpeed = flySpeed;
    }

    void Update()
    {
        if (isFlying && player != null)
        {
            // Tính hướng đến player
            Vector3 direction = (player.position - transform.position).normalized;

            // Tăng tốc dần
            currentSpeed += flyAcceleration * Time.deltaTime;

            // Di chuyển về phía player
            transform.position += direction * currentSpeed * Time.deltaTime;

            // Xoay coin khi bay
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            // Kiểm tra khoảng cách - tự động collect khi đủ gần
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < 0.5f)
            {
                ReturnCoin();
            }
        }
        
        // silde to player if distance < 2 units
        else if (player != null && playerStats != null     )
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < playerStats.GetPickupRange()) 
            {
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * flySpeed * 0.5f * Time.deltaTime;
            }
        }
    }

    void OnDisable()
    {
        // Reset state khi coin được return về pool
        isFlying = false;
        currentSpeed = 0f;
        transform.rotation = Quaternion.identity;
    }

    public void SlideTowardsPlayer(Vector3 playerPosition)
    {
        // triển khai logic slide ở đây
        Vector3 direction = (playerPosition - transform.position).normalized;
        transform.position += direction * flySpeed  * Time.deltaTime;
    }


}