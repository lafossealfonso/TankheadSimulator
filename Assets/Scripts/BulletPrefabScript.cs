using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    private bool readyToGo = false;
    private Vector3 targetPosition;
    public bool isRailTank = false;
    //public bool isPlayerTank = false;
    public bool isSpecial = false;
    [SerializeField] private Vector3 specialOffset;
    [SerializeField] private Transform specialBulletPrefab;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Transform explosionVFX;
    private bool nearTarget = false;

    public void Setup(Vector3 targetPosition, bool isSpecial)
    {
        this.isSpecial = isSpecial;
        if (isSpecial)
        {
            this.targetPosition = RailTank.Instance.GetPlayerTransform().position; // Get playerTransform from RailTank
            moveSpeed = 30f;
        }
        else
        {
            this.targetPosition = targetPosition;
        }
        readyToGo = true;
    }

    private void Update()
    {
        if (readyToGo)
        {
            if (isSpecial)
            {
                MoveToSpecialPosition();
            }
            else
            {
                MoveToTargetPosition();
            }
        }
    }

    private void MoveToTargetPosition()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        transform.LookAt(targetPosition);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.5f) // Adjust this threshold as needed
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
            if (isRailTank && nearTarget)
            {
                RailTank.Instance.DealEnemyDamage();
            }

            else
            {
                WeaponManager.Instance.DealDamage();
            }
            Destroy(gameObject);
        }
    }

    private void MoveToSpecialPosition()
    {
        Vector3 specialTargetPosition = targetPosition + specialOffset;
        Vector3 moveDir = (specialTargetPosition - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        transform.LookAt(targetPosition);

        float distanceToSpecialTarget = Vector3.Distance(transform.position, specialTargetPosition);
        if (distanceToSpecialTarget < 0.5f) // Adjust this threshold as needed
        {
            
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

            // Instantiate 5 bullets aimed at an area around the original target
            int bulletCount = 5;
            float spreadRadius = 5f; // Adjust the spread radius as needed
            for (int i = 0; i < bulletCount; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-spreadRadius, spreadRadius), 0, Random.Range(-spreadRadius, spreadRadius));
                Vector3 aoeTargetPosition = targetPosition + randomOffset;

                Transform bulletPrefab = Instantiate(specialBulletPrefab, transform.position, Quaternion.identity);
                BulletPrefabScript bulletPrefabScript = bulletPrefab.GetComponent<BulletPrefabScript>();
                bulletPrefabScript.Setup(aoeTargetPosition, false);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet collided with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player")) // Ensure the player's tag is set correctly
        {
            nearTarget = true;
            Debug.Log("Detected collision with player.");
            if (isRailTank)
            {
                Debug.Log("Calling DealEnemyDamage from BulletPrefabScript");
                RailTank.Instance.DealEnemyDamage();
                Destroy(gameObject); // Destroy bullet on hit
            }
            
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Entered trigger of enemy");
            nearTarget = true;
            WeaponManager.Instance.currentHealthSystem = other.gameObject.GetComponent<HealthSystem>();
            WeaponManager.Instance.DealDamage(); //deals damage to the enemy
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Ensure the player's tag is set correctly
        {
            nearTarget = false; 
        }
    }
}
