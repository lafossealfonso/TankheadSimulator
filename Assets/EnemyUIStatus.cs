using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIStatus : MonoBehaviour
{
    [SerializeField] GameObject parentObject;
    [SerializeField] TextMeshProUGUI enemyName;
    [SerializeField] TextMeshProUGUI enemyHealthStatus;
    [SerializeField] Slider enemyHealthSlider;

    private HealthSystem healthSystem;
    private bool thereIsEnemy;

    private void Update()
    {
        if (WeaponManager.Instance.currentHealthSystem != null)
        {
            //healthSystem = WeaponManager.Instance.currentHealthSystem;
        }

        if(WeaponManager.Instance.currentHealthSystem != null)
        {
            thereIsEnemy = true;
        }

        else
        {
            thereIsEnemy = false;
        }

        ActivateObjects();
    }

    private void ActivateObjects()
    {
        parentObject.SetActive(thereIsEnemy);
    }

    public void UpdateValues(HealthSystem healthySystem)
    {
        enemyName.text = healthySystem.objectName;
        enemyHealthStatus.text = ("Enemy Armour at " + Mathf.RoundToInt(healthySystem.health / healthySystem.maxHealth * 100f)  + "% ");
        //enemyHealthStatus.text = ("Enemy Armour at " + healthySystem.health + "/" + healthySystem.maxHealth + "% ");
        enemyHealthSlider.value = Mathf.RoundToInt(healthySystem.health / healthySystem.maxHealth * 100);
        
    }
}
