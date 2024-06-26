using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Object", menuName = "Category/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public float range;
    public int ammunition;
    public int maxAmmunition;
    public float reloadTime;
    public float fireIntervalTime;
    public int damageMin;
    public int damageMax;
    public bool reloadAlert;
    public Transform bulletPrefab;
}
