using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretUIElement : MonoBehaviour
{
    GameObject childGameObject;

    private void Start()
    {
        childGameObject = gameObject.transform.GetChild(0).gameObject;
    }
    private void Update()
    {
        childGameObject.SetActive(WeaponManager.Instance.currentlyRotating);
    }
}
