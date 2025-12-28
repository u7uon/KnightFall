using System.Collections;
using UnityEngine;

public class FireShuriken : EnemyBullet
{
    private Vector3 spawnPosition;
    private bool isReturning = false;
    private float returnTime = 1.5f;
    private Coroutine returnCoroutine;

    private void OnEnable()
    {
        spawnPosition = transform.position;
        isReturning = false;
        
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(ReturnAfterDelay());
    }

    private void OnDisable()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(returnTime);
        isReturning = true;
    }

    private void Update()
    {
        if (isReturning)
        {
            // Tìm Boss
            Boss boss = FindAnyObjectByType<Boss>();
            Vector3 targetPosition = boss != null ? boss.transform.position : spawnPosition;

            // Bay về target
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            transform.position += directionToTarget * Time.deltaTime * Speed;

            // Xoay theo hướng bay về
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Kiểm tra đã về đến target chưa
            if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                ReturnToPool();
            }
        }
        else
        {
            // Bay thẳng như bình thường
            if (direction == Vector3.zero) return;
            transform.position += direction * Time.deltaTime * Speed;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Gây damage cả khi bay đi và bay về
        if (collider2D.CompareTag("Player"))
        {
            PlayerStatsManager.Instance.TakeDame(GetDamge());
            ReturnToPool();
        }
    }
}