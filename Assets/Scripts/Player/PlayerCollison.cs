using UnityEngine;

public class PlayerCollison : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Usb"))
        {
            Destroy(collision.gameObject);
        }
    }
}
