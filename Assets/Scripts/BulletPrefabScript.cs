using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    private bool readyToGo = false;
    private Vector3 targetPosition;
    public bool isRailTank = false;
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
        Debug.Log("Bullet setup with target position: " + targetPosition);
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

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.5f) // Adjust this threshold as needed
        {
            Debug.Log("Bullet reached target position.");
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
            if (isRailTank && nearTarget)
            {
                Debug.Log("Bullet hit the target and is dealing damage.");
                RailTank.Instance.DealEnemyDamage();
            }
            Destroy(gameObject);
        }
    }

    private void MoveToSpecialPosition()
    {
        Vector3 specialTargetPosition = targetPosition + specialOffset;
        Vector3 moveDir = (specialTargetPosition - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceToSpecialTarget = Vector3.Distance(transform.position, specialTargetPosition);
        if (distanceToSpecialTarget < 0.5f) // Adjust this threshold as needed
        {
            Debug.Log("Bullet reached special target position.");
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

            // Instantiate 5 bullets aimed at an area around the original target
            int bulletCount = 20;
            float spreadRadius = 15f; // Adjust the spread radius as needed
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
            }
            Destroy(gameObject); // Destroy bullet on hit
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
