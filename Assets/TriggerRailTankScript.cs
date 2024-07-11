using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRailTankScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Debug.LogWarning("Bullet detected on Rail Tank");
            WeaponManager.Instance.DealDamage();
        }
    }
}
