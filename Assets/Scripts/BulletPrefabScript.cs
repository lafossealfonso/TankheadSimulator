using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    private bool readyToGo = false;
    private Vector3 targetPosition;
    public bool isRailTank = false;

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Transform explosionVFX;

    private bool nearTarget = false;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        readyToGo = true;
        Debug.Log("Bullet setup with target position: " + targetPosition);
    }

    private void Update()
    {
        if (readyToGo)
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
