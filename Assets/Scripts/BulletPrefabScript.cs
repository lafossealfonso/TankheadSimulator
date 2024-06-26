using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    private bool readyToGo = false;
    [SerializeField]private Vector3 targetPosition;

    public bool isEnemy = false;

    [SerializeField] float moveSpeed;
    [SerializeField] Transform explosionVFX;

    private bool nearTarget = false;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        readyToGo = true;
    }

    private void Update()
    {

        if (readyToGo && isEnemy == false)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

            transform.position += moveDir * moveSpeed * Time.deltaTime;

            float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

            if(distanceBeforeMoving < distanceAfterMoving)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
                
                
                WeaponManager.Instance.DealDamage();
                WeaponManager.Instance.UpdateUiEnemyStatusValues();
                
                
                Destroy(gameObject);
            }
        }

        else if(readyToGo && isEnemy == true) 
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

            transform.position += moveDir * moveSpeed * Time.deltaTime;

            float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

            if (distanceBeforeMoving < distanceAfterMoving)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
                if (nearTarget)
                {
                    RailTank.Instance.DealEnemyDamage();
                }
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            nearTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            nearTarget = false;
        }
    }
}
