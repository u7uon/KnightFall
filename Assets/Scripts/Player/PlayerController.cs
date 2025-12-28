using System;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Image = UnityEngine.UI.Image;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public bool flipx = false; 
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SuckNearItems();
        HandleMovement();
        UpdateAnimation();   
    }

    private void HandleMovement()
{
    Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    
    // Nếu không nhấn phím, dừng ngay lập tức
    if (input.magnitude < 0.1f)
    {
        rb.linearVelocity = Vector2.zero;
    }
    else
    {
        rb.linearVelocity = input.normalized * PlayerStatsManager.Instance.GetPlayerSpeed();
    }

    // Lật sprite
    if (input.x > 0)
        flipx = spriteRenderer.flipX = false;
    else if (input.x < 0)
        flipx = spriteRenderer.flipX = true;
}

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("IsRunning", isRunning);
    }

    void SuckNearItems()
    {
        var range  = PlayerStatsManager.Instance.GetPickupRange();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (var collider in colliders)
        {
            ICollectable collectable = collider.GetComponent<ICollectable>();
            if (collectable != null)
            {
                collectable.SlideTowardsPlayer(transform.position);
            }
        }
    }
}
