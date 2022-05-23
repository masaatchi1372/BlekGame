using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBehaviour : MonoBehaviour
{
    public abstract bool TakeDamage(float damage);
    public abstract bool Die();
    public abstract void Move(Vector3 position);
}
