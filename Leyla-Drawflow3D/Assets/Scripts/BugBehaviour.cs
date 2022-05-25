using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Collider))]
public class BugBehaviour : ObjectBehaviour
{
    public float movementSpeedFactor = 1f;
    public float rotationSpeedFactor = 0.1f;
    public int maxHealth = 5;
    public int minHealth = 0;
    private Collider2D objCollider;
    private Health health;

    private void Start()
    {
        // getting collider2D component
        objCollider = GetComponent<Collider2D>();

        //initializing the health component
        health = GetComponent<Health>();
        if (health != null)
        {
            health.maxHealth = maxHealth;
            health.minHealth = minHealth;
            health.health = maxHealth;
        }
        else
        {
            Debug.LogError("The bug hasn't any health component");
        }
    }

    public override bool TakeDamage(float damage)
    {
        // taking the damage
        if (health != null)
        {
            health.TakeDamage(damage);
            if (health.health <= health.minHealth)
            {
                Die();
                return true;
            }
        }

        //downScale the character
        if (objCollider != null)
        {
            float tmpRatio = 0.8f;
            transform.localScale *= tmpRatio;
        }

        return true;
    }

    public override bool Die()
    {
        Destroy(gameObject);
        return true;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Simple movement based on delta x and delta y on screen
    /// </summary>
    public override void Move(Vector3 position)
    {
        // rotate the object towards the movement direction
        Vector3 direction = (position - transform.position).normalized;
        float angle = Vector3.SignedAngle(new Vector3(-1, 0, 0), direction, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotationSpeedFactor);

        // move the object
        transform.position = Vector3.Lerp(transform.position, position, movementSpeedFactor * Time.deltaTime);
    }
}