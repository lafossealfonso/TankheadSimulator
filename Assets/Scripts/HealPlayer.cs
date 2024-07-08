using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    private HealthSystem playerHealthSystem;
    [SerializeField] private int healAmount;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            playerHealthSystem = other.GetComponent<HealthSystem>();
            playerHealthSystem.Heal((int)playerHealthSystem.health);
            Destroy(gameObject);
        }
    }
}
