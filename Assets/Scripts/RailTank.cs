using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class RailTank : MonoBehaviour
{
    public static RailTank Instance { get; private set; }

    private float targetRotation;
    private float currentRotation;

    [Header("Movement And Speeds")]
    [SerializeField] private float gunRotateSpeed;
    [SerializeField] private float movementSpeed = 2f;

    [Header("Transform Attributes")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform topTankTransform;
    [SerializeField] private Transform shootPointA;
    [SerializeField] private Transform shootPointB;
    [SerializeField] private Transform particleGroup;

    [Header("Miscellaneous")]
    [SerializeField] private WeaponScriptableObject railWeapon;
    [SerializeField] private LayerMask hittableLayerMask;

    private Transform lastPlayerTransform;

    [SerializeField] private HealthSystem currentHealthSystem; // Assigned in the inspector

    private bool isMoving;
    private bool shootPointBool;
    private Transform currentShootPoint;

    public bool playerInProximity = false;

    // New counter and limit for the shots
    private int shotCounter = 0;
    [SerializeField] private int shotsBeforeTrue = 5; // Set the desired number of shots before sending true

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one RailTank! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        targetRotation = currentRotation;
        GameObject gameObject = GameObject.FindWithTag("Player");
        InvokeRepeating("UpdatePlayerTransform", 0f, 1.5f);
        InvokeRepeating("CanSeePlayer", 0f, 10f);

        if (railWeapon != null)
        {
            InvokeRepeating("ShootRailTank", 0f, railWeapon.fireIntervalTime);
        }
    }

    private void Update()
    {
        RotateGunToTarget();

        transform.eulerAngles = new Vector3(transform.rotation.x, currentRotation, transform.rotation.z);

        if (Mathf.Abs(targetRotation - currentRotation) > 2f)
        {
            isMoving = true;
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, movementSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }

        particleGroup.gameObject.SetActive(isMoving);
    }

    public void MoveToNewPosition()
    {
        int firstPass = Random.Range(0, 360);
        if (Mathf.Abs(firstPass - currentRotation) <= 40)
        {
            MoveToNewPosition();
        }
        else
        {
            targetRotation = firstPass;
        }
    }

    public void RotateGunToTarget()
    {
        Vector3 directionToTarget = playerTransform.position - topTankTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        topTankTransform.rotation = Quaternion.RotateTowards(topTankTransform.rotation, targetRotation, gunRotateSpeed * Time.deltaTime);
    }

    private void UpdatePlayerTransform()
    {
        lastPlayerTransform = playerTransform;
    }

    public void ShootRailTank()
    {
        currentShootPoint = shootPointBool ? shootPointB : shootPointA;

        Transform bulletPrefab = Instantiate(railWeapon.bulletPrefab, currentShootPoint.position, Quaternion.identity);
        BulletPrefabScript bulletPrefabScript = bulletPrefab.GetComponent<BulletPrefabScript>();
        bulletPrefabScript.isRailTank = true;

        Vector3 shootDir = (lastPlayerTransform.position - currentShootPoint.position).normalized;

        if (Physics.Raycast(currentShootPoint.position, shootDir, out RaycastHit hit, railWeapon.range, hittableLayerMask))
        {
            Vector3 shootTargetPosition = hit.point;
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
            {
                Debug.Log("HealthSystem found on: " + hitObject.name);
                bulletPrefabScript.Setup(shootTargetPosition, shotCounter >= shotsBeforeTrue && GetComponent<HealthSystem>().health > GetComponent<HealthSystem>().maxHealth);
            }
            else
            {
                Debug.LogWarning("HealthSystem not found on: " + hitObject.name);
                bulletPrefabScript.Setup(shootTargetPosition, shotCounter >= shotsBeforeTrue && GetComponent<HealthSystem>().health > (GetComponent<HealthSystem>().maxHealth / 2));
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit anything.");
            Vector3 shootTargetPosition = currentShootPoint.position + shootDir * railWeapon.range;
            bulletPrefabScript.Setup(shootTargetPosition, false);
        }

        shootPointBool = !shootPointBool;

        // Increment the shot counter and reset if the limit is reached
        shotCounter++;
        if (shotCounter > shotsBeforeTrue)
        {
            shotCounter = 0;
        }
    }

    public void DealEnemyDamage()
    {
        Debug.Log("Executing Deal Enemy Damage");
        if (currentHealthSystem != null)
        {
            Debug.Log("Dealing damage to the player.");
            currentHealthSystem.TakeDamage(WeaponManager.Instance.CalculateDamage(railWeapon.damageMin, railWeapon.damageMax));
        }
        else
        {
            Debug.LogWarning("Current health system is null, cannot deal damage.");
        }
    }

    public void CanSeePlayer()
    {
        if (playerTransform != null)
        {
            if (currentShootPoint == null)
            {
                currentShootPoint = shootPointA;
            }

            Vector3 shootDir = (playerTransform.position - currentShootPoint.position).normalized;

            if (Physics.Raycast(currentShootPoint.position, shootDir, out RaycastHit hit, railWeapon.range, hittableLayerMask))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    return;
                }
                else
                {
                    MoveToNewPosition();
                }
            }
        }
    }

    public Transform GetPlayerTransform()
    {
        return lastPlayerTransform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            Debug.LogWarning("Bullet detected on Rail Tank");
            WeaponManager.Instance.DealDamage();
        }
    }

}
