using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            health = 0;
            Destroy(gameObject);
        }

        if (health <= 0 && !ifEnemy) { health = 0; SceneManager.LoadScene("MenuScene"); }
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

    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
