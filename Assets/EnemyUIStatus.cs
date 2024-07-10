using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIStatus : MonoBehaviour
{
    [SerializeField] GameObject parentObject;
    [SerializeField] TextMeshProUGUI enemyName;
    [SerializeField] TextMeshProUGUI enemyHealthStatus;
    [SerializeField] Slider enemyHealthSlider;

    public HealthSystem healthSystem;
    private bool thereIsEnemy;

    private void Update()
    {

        ActivateObjects();
        enemyHealthStatus.text = ("Enemy Armour at " + Mathf.RoundToInt(healthSystem.health / healthSystem.maxHealth * 100f) + "% ");

    }

    private void ActivateObjects()
    {
        parentObject.SetActive(true);
    }

    public void UpdateValues(HealthSystem healthySystem)
    {
        healthSystem = healthySystem;
        enemyName.text = healthySystem.objectName;
        enemyHealthStatus.text = ("Enemy Armour at " + Mathf.RoundToInt(healthySystem.health / healthySystem.maxHealth * 100f)  + "% ");
        enemyHealthSlider.value = Mathf.RoundToInt(healthySystem.health / healthySystem.maxHealth * 100);
        
    }
}
