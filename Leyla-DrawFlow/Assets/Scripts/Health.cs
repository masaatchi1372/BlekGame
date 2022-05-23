using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [HideInInspector] public int maxHealth;
    [HideInInspector] public int minHealth;
    [HideInInspector] public float health; // health parameter is always from 0-1. we'll calculate everything based on Max Health
    private bool canTakeDamage;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        canTakeDamage = true;        
    }

    // taking damage function
    public bool TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            if (health - damage > minHealth && health - damage < maxHealth)
            {
                health -= damage;
            }
            else if (health - damage <= minHealth)
            {
                health = minHealth;
            }
            else if (health - damage >= maxHealth)
            {
                health = maxHealth;
            }            

            return true;
        }

        return false;
    }
}
