using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTankProximity : MonoBehaviour
{
    [SerializeField] private RailTank railTank;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("EnteredCollider");
            railTank.playerInProximity = true;
            railTank.MoveToNewPosition();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            railTank.playerInProximity = false;
        }
    }
}
