using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float power = 1f;
    public bool DealDamage(Collider2D other)
    {
        ObjectBehaviour behaviour;
        if (other.TryGetComponent<ObjectBehaviour>(out behaviour))
        {
            return behaviour.TakeDamage(power);
        }

        return false;
    }
}
