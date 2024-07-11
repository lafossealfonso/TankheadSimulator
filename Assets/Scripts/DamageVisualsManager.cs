using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageVisualsManager : MonoBehaviour
{
    
    [SerializeField] private Transform gunShotSmokePrefab;
    [SerializeField] private Transform shootSpawnPosition;

    private void Awake()
    {
        WeaponManager.Instance.OnShootEvent += GunShotVisual;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GunShotVisual(object sender, EventArgs e)
    {
        Instantiate(gunShotSmokePrefab, shootSpawnPosition.position, Quaternion.identity);   
    }

    

}
