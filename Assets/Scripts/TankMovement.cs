using System;
using Unity.VisualScripting;
using UnityEngine;

public class TankMovement : MonoBehaviour
{

    [Header("Speeds and Height")]
    [Space(2)]
    [SerializeField] private float lookRotationSpeed = 1f;
    [SerializeField] private float tankRotationSpeed = 5f;
    [SerializeField] private float tankMoveSpeed = 100f;
    [SerializeField] private float gunRotateSpeed = 5f;
    [SerializeField] private float raycastAndTankOffset;

    [Header("Transform Attributes")]
    [Space(2)]
    [SerializeField] private Transform lookAtParent;
    [SerializeField] private Transform tankTopParent;
    [SerializeField] private Transform raycastToTerrain;
    [SerializeField] private Transform enemyTankTransform;

    [Header("Booster Particle System")]
    [Space(2)]
    [SerializeField] private Transform Booster1;
    [SerializeField] private Transform Booster2;

    [Header("Legs Array")]
    [Space(2)]
    public Leg[] legArray;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float sprintSpeed;

    [Header("Layer Masks")]
    [Space(2)]
    public LayerMask terrainLayer;
    public LayerMask hittableLayer;

    private Transform lookAtChild;
    private Vector3 initialOffset;
    
    private Camera mainCamera;

    private void Awake()
    {
        WeaponManager.Instance.OnShootEvent += Shoot;
    }
    private void Start()
    {
        initialOffset = tankTopParent.position - transform.position;
        lookAtChild = lookAtParent.GetChild(0).transform;
        mainCamera = Camera.main;
        Cursor.visible = false;

    }

    private void Update()
    {
        MoveTank();
        AimGun();
        RotateTank();
        RotateLookAt();


    }

    private void RotateLookAt()
    {
        Vector3 tankTopPos = transform.position + initialOffset;

        

        if (Input.GetMouseButton(1))
        {
            Vector3 shootDir = (transform.position - enemyTankTransform.position).normalized;
            lookAtParent.position = tankTopPos;
            tankTopParent.position = tankTopPos;

            if (Physics.Raycast(transform.position, shootDir, out RaycastHit hit, 1000f, hittableLayer))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    

                    // Rotate the lookAtParent towards the enemyTankPosition
                    Vector3 directionToEnemy = enemyTankTransform.position - tankTopPos;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                    lookAtParent.rotation = Quaternion.RotateTowards(lookAtParent.rotation, targetRotation, lookRotationSpeed * 30f * Time.deltaTime);
                }
            }
        }

        else
        {
            lookAtParent.position = tankTopPos;
            tankTopParent.position = tankTopPos;

            float mouseX = Input.GetAxis("Mouse X");
            lookAtParent.Rotate(Vector3.up, mouseX * lookRotationSpeed);
        }
    }

    private void AimGun()
    {
        Vector3 directionToTarget = lookAtChild.position - tankTopParent.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        tankTopParent.rotation = Quaternion.RotateTowards(tankTopParent.rotation, targetRotation, gunRotateSpeed * Time.deltaTime);
        // Check if the rotation is necessary
        if (Quaternion.Angle(tankTopParent.rotation, targetRotation) > 1f) // Adjust the threshold as needed
        {
            WeaponManager.Instance.currentlyRotating = true;
        }
        else
        {
            WeaponManager.Instance.currentlyRotating = false;
        }
    }

    private void MoveTank()
    {

        bool isDashing = Input.GetKey(KeyCode.LeftShift);
        Booster1.gameObject.SetActive(isDashing);
        Booster2.gameObject.SetActive(isDashing);

        RaycastHit hit;
        if (Physics.Raycast(raycastToTerrain.position, Vector3.down, out hit, 20f, terrainLayer))
        {
            float newY = hit.point.y + raycastAndTankOffset;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }


        if (isDashing && Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            Vector3 dashDirection = transform.forward * tankMoveSpeed * 2f * Time.deltaTime;

            transform.Translate(dashDirection, Space.World);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 68f, 10f * Time.deltaTime);

            foreach (Leg leg in legArray)
            {
                leg.travelSpeed = sprintSpeed;
            }
        }
        else
        {

            float accelerationInput = Input.GetAxis("Vertical");
            transform.Translate(Vector3.forward * accelerationInput * tankMoveSpeed * Time.deltaTime);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 60f, 10f * Time.deltaTime);
            foreach (Leg leg in legArray)
            {
                leg.travelSpeed = normalSpeed;
            }
        }
    }

    private void RotateTank()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        float rotationY = rotationInput * tankRotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationY);
    }

    private void Shoot(object sender, EventArgs e)
    {
        float shootRange = WeaponManager.Instance.currentWeapon.range;
        Vector3 newPosition = lookAtParent.position + (lookAtParent.forward * shootRange);
        lookAtChild.position = newPosition;
        Debug.Log(shootRange);
    }
}