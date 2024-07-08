using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    private HealthSystem playerHealthSystem;
    [SerializeField] private int healAmount;
    [SerializeField] GameObject text;

    private void Start()
    {
        text.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            text.SetActive(true);

            if (Input.GetKey(KeyCode.E))
            {
                playerHealthSystem = other.GetComponent<HealthSystem>();
                playerHealthSystem.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            text.SetActive(false);
        }
    }
}
