using UnityEngine;

public interface ICollectable
{
    void Collect();

    void SlideTowardsPlayer(Vector3 playerPosition);
}