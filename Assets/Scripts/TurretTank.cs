using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine;

public class TurretTank : MonoBehaviour
{

    private float targetRotation;
    private float currentRotation;

    [Header("Movement And Speeds")]
    [Space(2)]
    [SerializeField] private float gunRotateSpeed;
    //[SerializeField] private float movementSpeed = 2f;



    [Header("Transform Attributes")]
    [Space(2)]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private Transform topTankTransform;
    [SerializeField] private Transform shootPointA;
    [SerializeField] private Transform shootPointB;
    [SerializeField] private Transform particleGroup;

    [Header("Miscellaneous")]
    [Space(2)]
    [SerializeField] private WeaponScriptableObject railWeapon;
    [SerializeField] private LayerMask hittableLayerMask;

    

    private HealthSystem currentHealthSystem;

    



    private bool shootPointBool;
    private Transform currentShootPoint;

    public bool playerInProximity = false;
    private void Awake()
    {
        
    }
    private void Start()
    {
        targetRotation = currentRotation;     

        if (railWeapon != null)
        {
            InvokeRepeating("ShootRailTank", 0f, railWeapon.fireIntervalTime);
        }


    }

    public void ShootRailTank()
    {
        if (shootPointBool == false)
        {
            currentShootPoint = shootPointA;
        }
        else if (shootPointBool == true)
        {
            currentShootPoint = shootPointB;
        }

        Transform bulletPrefab = Instantiate(railWeapon.bulletPrefab, currentShootPoint);
        BulletPrefabScript bulletPrefabScript = bulletPrefab.GetComponent<BulletPrefabScript>();
        //bulletPrefabScript.isTurretTank = true;

        Vector3 shootDir = (aimTransform.position - currentShootPoint.position).normalized;

        if (Physics.Raycast(currentShootPoint.position, shootDir, out RaycastHit hit, railWeapon.range, hittableLayerMask))
        {
            Vector3 shootTargetPosition = hit.point;
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
            {
                currentHealthSystem = healthSystem;
                bulletPrefabScript.Setup(shootTargetPosition, false);
            }

            else
            {

                bulletPrefabScript.Setup(shootTargetPosition, false);
            }
        }

        shootPointBool = !shootPointBool;

    }

    public void DealEnemyDamage()
    {
        if (currentHealthSystem != null)
        {
            currentHealthSystem.TakeDamage(WeaponManager.Instance.CalculateDamage(railWeapon.damageMin, railWeapon.damageMax));
        }
    }

}
