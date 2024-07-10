using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public EventHandler<EventArgs> OnShootEvent;

    public WeaponScriptableObject currentWeapon;

    public List<WeaponScriptableObject> weaponsList;

    public int currentWeaponIndex = 0;

    public bool ableToAct = true;
    public bool enoughAmmo;
    public bool currentlyRotating = false;

    public HealthSystem currentHealthSystem;

    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private Transform shootTarget;
    [SerializeField] private LayerMask hittableLayerMask;
    [SerializeField] private EnemyUIStatus enemyUIStatusScript;

    public Animator tankAnimator;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one WeaponManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentWeapon = weaponsList[currentWeaponIndex];
    }

    private void Start()
    {
        ableToAct = true;

        for (int i = 0; i < weaponsList.Count; i++)
        {
            weaponsList[i].ammunition = weaponsList[i].maxAmmunition;
            weaponsList[i].reloadAlert = false;
        }
    }

    private void Update()
    {
        RotateWeapon();

        if (currentWeapon.ammunition > 0)
        {
            enoughAmmo = true;
            currentWeapon.reloadAlert = false;
        }
        else if (currentWeapon.ammunition <= 0)
        {
            enoughAmmo = false;
            currentWeapon.reloadAlert = true;
        }

        if (Input.GetMouseButtonDown(0) && ableToAct && enoughAmmo & !currentlyRotating)
        {
            ShootCurrentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.R) && ableToAct)
        {
            ReloadCurrentGun();
        }
    }

    private void ReloadCurrentGun()
    {
        ableToAct = false;
        StartCoroutine(ReloadGunTimer(currentWeapon.reloadTime));
    }

    private void ShootCurrentWeapon()
    {
        ableToAct = false;
        currentWeapon.ammunition--;
        OnShootEvent?.Invoke(this, EventArgs.Empty);

        tankAnimator.SetTrigger("Shoot");

        Transform bulletPrefab = Instantiate(currentWeapon.bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        BulletPrefabScript bulletPrefabScript = bulletPrefab.GetComponent<BulletPrefabScript>();

        Vector3 shootDir = (shootTarget.position - bulletSpawner.position).normalized;

        if (Physics.Raycast(bulletSpawner.position, shootDir, out RaycastHit hit, currentWeapon.range, hittableLayerMask))
        {
            Vector3 shootTargetPosition = hit.point;
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
            {
                currentHealthSystem = healthSystem;
            }
            else
            {
                Debug.Log("resetted current healthSystem");
                currentHealthSystem = null;
            }

            bulletPrefabScript.Setup(shootTargetPosition, false);
        }
        else
        {
            Vector3 shootTargetVector = shootTarget.position;
            bulletPrefabScript.Setup(shootTargetVector, false);
            Debug.Log("Not shooting raycast");
            currentHealthSystem = null;
        }

        StartCoroutine(ResetCanShoot(currentWeapon.fireIntervalTime));
    }

    public void UpdateUiEnemyStatusValues()
    {
        if (currentHealthSystem != null)
        {
            enemyUIStatusScript.UpdateValues(currentHealthSystem);
        }
    }
    public void DealDamage()
    {
        if (currentHealthSystem != null)
        {
            currentHealthSystem.TakeDamage(CalculateDamage(currentWeapon.damageMin, currentWeapon.damageMax));
            Debug.Log("DEALING TONS OF DAMAGE");

            enemyUIStatusScript.UpdateValues(currentHealthSystem);
        }

        else { Debug.Log("Current Health system null"); }
    }

    private IEnumerator ResetCanShoot(float intervalTime)
    {
        float timer = 0f;
        float startingIntervalTime = intervalTime;
        while (timer < intervalTime)
        {
            timer += Time.deltaTime;
            currentWeapon.fireIntervalTime = timer / intervalTime;

            yield return null;
        }

        currentWeapon.fireIntervalTime = startingIntervalTime;
        ableToAct = true;
    }

    private IEnumerator ReloadGunTimer(float reloadTime)
    {
        float timer = 0f;
        float startingIntervalTime = reloadTime;
        while (timer < reloadTime)
        {
            timer += Time.deltaTime;
            currentWeapon.reloadTime = timer / reloadTime;

            yield return null;
        }

        currentWeapon.reloadTime = startingIntervalTime;
        currentWeapon.ammunition = currentWeapon.maxAmmunition;
        ableToAct = true;
    }

    private void RotateWeapon()
    {
        currentWeapon = weaponsList[currentWeaponIndex];

        if (Input.GetKeyDown(KeyCode.Mouse1) && ableToAct)
        {
            currentWeaponIndex++;

            if (currentWeaponIndex >= weaponsList.Count)
            {
                currentWeaponIndex = 0;
            }

            currentWeapon = weaponsList[currentWeaponIndex];
        }
    }

    public void SetupWeapon(WeaponScriptableObject passedWeapon)
    {
        currentWeapon = passedWeapon;
    }

    public WeaponScriptableObject GetWeaponOnIndex(int index)
    {
        WeaponScriptableObject weaponOnIndex = weaponsList[index];
        return weaponOnIndex;
    }

    public int CalculateDamage(int minimumDmg, int maxDmg)
    {
        int totalDamage = Random.Range(minimumDmg, maxDmg);
        return totalDamage;
    }
}