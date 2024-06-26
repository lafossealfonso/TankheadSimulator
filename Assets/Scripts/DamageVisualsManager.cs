using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageVisualsManager : MonoBehaviour
{
    private HealthSystem healthSystem;

    [Header("<b>Player Attributes<b>")]
    [Header("Smoke Health Visual Prefabs")]
    [Space(2)]

    [SerializeField] private Transform smokePrefab1;
    [SerializeField] private Transform smokePrefab2;
    [SerializeField] private Transform smokePrefab3;

    [Header("Tank Parts Effect Prefabs")] 
    [Space(2)]
    [SerializeField] private Transform gunShotSmokePrefab;
    [SerializeField] private Transform shootSpawnPosition;

    private void Awake()
    {
        WeaponManager.Instance.OnShootEvent += GunShotVisual;
    }

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthSmokeEffect();
    }

    void GunShotVisual(object sender, EventArgs e)
    {
        Instantiate(gunShotSmokePrefab, shootSpawnPosition.position, Quaternion.identity);   
    }

    void UpdateHealthSmokeEffect()
    {
        // Check for health thresholds and activate/deactivate smoke prefabs accordingly
        if (healthSystem.health <= (healthSystem.maxHealth / 4) * 3 && healthSystem.health >= (healthSystem.maxHealth / 2)) // 75%
        {
            smokePrefab1.gameObject.SetActive(true);
            smokePrefab2.gameObject.SetActive(false);
            smokePrefab3.gameObject.SetActive(false);
        }
        else if (healthSystem.health <= (healthSystem.maxHealth / 2) && healthSystem.health >= (healthSystem.maxHealth / 4)) // 50%
        {
            smokePrefab1.gameObject.SetActive(false);
            smokePrefab2.gameObject.SetActive(true);
            smokePrefab3.gameObject.SetActive(false);
        }
        else if (healthSystem.health <= (healthSystem.maxHealth / 4)) // 25%
        {
            smokePrefab1.gameObject.SetActive(false);
            smokePrefab2.gameObject.SetActive(false);
            smokePrefab3.gameObject.SetActive(true);
        }
        else // Above 75%
        {
            smokePrefab1.gameObject.SetActive(false);
            smokePrefab2.gameObject.SetActive(false);
            smokePrefab3.gameObject.SetActive(false);
        }
    }

}
