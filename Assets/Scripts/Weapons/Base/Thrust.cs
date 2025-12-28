using System.Collections;
using UnityEngine;

public class ThrustWeapon : WeaponBase
{
    [Header("Thrust Settings")]
    public float thrustOvershoot = 1f;
    public float thrustSpeed = 50f;
    public float returnSpeed = 20f;

    private Transform currentTarget;

    protected override void TryAttack()
    {
        if (isAttacking) return;
        Transform target = FindNearestEnemy();
        if (target == null) return;

        currentTarget = target;
        StartCoroutine(ThrustAttack());
    }

    private IEnumerator ThrustAttack()
    {
        
        isThrustLocked = true;

        Vector3 startPos = transform.localPosition;
        Vector2 dir = (currentTarget.position - transform.position).normalized;
        float distToEnemy = Vector2.Distance(player.position, currentTarget.position);
        float actualDistance = distToEnemy + thrustOvershoot;
        Vector3 endPos = startPos + (Vector3)dir * actualDistance;

        float elapsed = 0f;
        float thrustDuration = actualDistance / thrustSpeed;
        isAttacking = true;
        while (elapsed < thrustDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / thrustDuration);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        elapsed = 0f;
        float returnDuration = actualDistance / returnSpeed;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(endPos, startPos, elapsed / returnDuration);
            yield return null;
        }

        transform.localPosition = startPos;

        isThrustLocked = false;
    }


}
