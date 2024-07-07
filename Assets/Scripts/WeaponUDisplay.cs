using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUDisplay : MonoBehaviour
{
    private WeaponScriptableObject thisWeapon;
    [SerializeField]private int slotNumber;

    //UI Elements
    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] TextMeshProUGUI weaponRange;
    [SerializeField] TextMeshProUGUI weaponDamage;
    [SerializeField] TextMeshProUGUI weaponAmmo;
    [SerializeField] TextMeshProUGUI reloadAlert;
    [SerializeField] Slider reloadTimeSlider;
    [SerializeField] Slider fireTimeSlider;
    [SerializeField] Image bgImage1;
    [SerializeField] Image bgImage2;

    private float maxFireTime;
    private float maxReloadTime;

    private bool currentlySelected;

    private void Awake()
    {
        //thisWeapon = WeaponManager.Instance.GetWeaponOnIndex(slotNumber);
        
        WeaponManager.Instance.OnShootEvent += OnShoot;
    }

    private void Start()
    {
        if(WeaponManager.Instance.GetWeaponOnIndex(slotNumber) != null)
        {
            thisWeapon = WeaponManager.Instance.GetWeaponOnIndex(slotNumber);
        }
        fireTimeSlider.maxValue = thisWeapon.fireIntervalTime;
        SetupUI();
    }


    private void Update()
    {
        if(thisWeapon!= null)
        {
            // Update UI elements
            reloadAlert.gameObject.SetActive(thisWeapon.reloadAlert);
            reloadTimeSlider.value = thisWeapon.reloadTime;
            fireTimeSlider.value = thisWeapon.fireIntervalTime;
            weaponAmmo.text = thisWeapon.ammunition + " | " + thisWeapon.maxAmmunition + " Rounds";

            // Check if this weapon is currently selected
            if (WeaponManager.Instance.currentWeaponIndex == slotNumber)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            }
            else
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }
    }



    private void SetupUI()
    {
        weaponName.text = thisWeapon.weaponName;
        weaponRange.text = "Range - " + thisWeapon.range + "m";
        weaponDamage.text = "DAMAGE [" + thisWeapon.damageMin + " | " + thisWeapon.damageMax + "]";
        weaponAmmo.text = thisWeapon.ammunition + " | " + thisWeapon.maxAmmunition + "Rounds";
        reloadAlert.gameObject.SetActive(false);
        maxFireTime = thisWeapon.fireIntervalTime;
        maxReloadTime = thisWeapon.reloadTime;
    }

    public void OnShoot(object sender, EventArgs e)
    {
        weaponAmmo.text = thisWeapon.ammunition + " | " + thisWeapon.maxAmmunition + "Rounds";
    }

}
