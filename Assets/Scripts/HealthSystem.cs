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
    // Update is called once per frame
    void Update()
    {
       if(health <= 0)
        {
            Destroy(gameObject);
        } 
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        

        if(ifEnemy == true)
        {
            RailTank.Instance.MoveToNewPosition();
        }
    }
}
