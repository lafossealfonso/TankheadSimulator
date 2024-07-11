using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI playerHealthStatus;
    [SerializeField] GameObject lowHealthAlert;
    [SerializeField] HealthSystem playerHealthSystem;
    [SerializeField] GameObject burstReady;
    [SerializeField] TankMovement tankMovementScript;


    private void Update()
    {
        playerHealthStatus.text = "Heikegani Armour: " + Mathf.RoundToInt(playerHealthSystem.health/playerHealthSystem.maxHealth * 100) + "%";
        healthSlider.value = Mathf.RoundToInt(playerHealthSystem.health / playerHealthSystem.maxHealth * 100);

        if(Mathf.RoundToInt(playerHealthSystem.health / playerHealthSystem.maxHealth * 100) <= 25f)
        {
            lowHealthAlert.SetActive(true);
        }

        else
        {
            lowHealthAlert.SetActive(false);    
        }

        burstReady.SetActive(!tankMovementScript.isBurstOnCooldown);
    }
}
