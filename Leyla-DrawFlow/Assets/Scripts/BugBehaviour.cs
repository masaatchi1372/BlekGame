using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BugBehaviour : ObjectBehaviour
{
    public float movementSpeedFactor = 1f;
    public float rotationSpeedFactor = 2f;
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
    }

    public override bool TakeDamage(float damage)
    {
        // taking the damage
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"healt:{health.health} , {damage}");
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
            Debug.Log($"downScale:{tmpRatio}");
            transform.localScale *= tmpRatio;
        }

        return true;
    }

    public override bool Die()
    {
        Debug.Log("died");
        Destroy(gameObject);
        return true;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Move(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    /// <summary>
    /// Simple movement based on delta x and delta y on screen
    /// </summary>
    public override void Move(float dX, float dY)
    {        
        return;
        
        // move the object
        transform.position = Vector3.Lerp(transform.position, new Vector3(dX, dY, 0), movementSpeedFactor * Time.deltaTime);

        // rotate the object
        Quaternion toRotate = Quaternion.LookRotation(Vector3.forward, new Vector2(dX,dY).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeedFactor * Time.deltaTime);
    }
}
