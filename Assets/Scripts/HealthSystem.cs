using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public string objectName;
    public float health;
    public float maxHealth = 100;

    public bool ifEnemy = false;

    private void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            Debug.Log(objectName + " has been destroyed.");
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(objectName + " took " + damage + " damage. Remaining health: " + health);

        if (ifEnemy)
        {
            RailTank.Instance.MoveToNewPosition();
        }
    }
}
