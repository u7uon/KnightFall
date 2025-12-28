using UnityEngine;
using UnityEngine.Rendering;

public class PlayerBulletController : MonoBehaviour
{

    [SerializeField] private float speed = 20f;

    [SerializeField] private float timeDestroy = 1f;

    [SerializeField] private int damage = 1;

    [SerializeField] private GameObject bloodEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                var blood = Instantiate(bloodEffect, transform.position, Quaternion.identity);   
                Destroy(blood,0.2f);
            }
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }
    
    void Shoot()
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime);
            }
}
