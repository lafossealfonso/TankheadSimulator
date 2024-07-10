using Cinemachine;
using System.Collections;
using UnityEngine;

public class RailTankSpawner : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isActivated = false;
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private RailTank railTankScript;
    [SerializeField] private float height;
    [SerializeField] private float speed; // Speed at which the object moves
    [SerializeField] private float arrivalThreshold; // Distance threshold for stopping
    [SerializeField] private GameObject textGameObject;

    [Header("Camera Settings")]
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform railTank;
    [SerializeField] private Transform spiderTank;
    [SerializeField] private Transform railTankCameraPosition;
    [SerializeField] private TankMovement tankMovementScript;
    [SerializeField] private WeaponManager weaponManagerScript;
    private Transform lastCameraPosition;

    private void Start()
    {
        originalPosition = railTank.transform.position;
        objectToActivate.SetActive(false); // Ensure the object to activate starts deactivated
        railTankScript.enabled = false;
        textGameObject.SetActive(false);

        tankMovementScript.enabled = false;
        virtualCamera.m_Lens.NearClipPlane = 155f;
        weaponManagerScript.enabled = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            textGameObject.SetActive(true);

            if (Input.GetKey(KeyCode.E))
            {
                // Activate the object above its original position
                objectToActivate.transform.position = originalPosition + Vector3.up * height;
                objectToActivate.SetActive(true);
                isActivated = true;
                virtualCamera.m_Follow = railTankCameraPosition;
                virtualCamera.m_LookAt = railTank;
                tankMovementScript.enabled = false;
                lastCameraPosition = mainCameraTransform;
                mainCameraTransform.position = railTankCameraPosition.position;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        textGameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCameraNearClipPlane();

        if (isActivated)
        {
            // Move the object towards originalPosition at a constant speed
            objectToActivate.transform.position = Vector3.MoveTowards(objectToActivate.transform.position, originalPosition, speed * Time.deltaTime);

            // Check if the object is close enough to the target position
            if (Vector3.Distance(objectToActivate.transform.position, originalPosition) < arrivalThreshold)
            {
                // Ensure exact positioning at the end
                objectToActivate.transform.position = originalPosition;
                isActivated = false;

                StartCoroutine(WaitFor(2f));
            }
        }
    }

    private void UpdateCameraNearClipPlane()
    {
        if (virtualCamera.m_Lens.NearClipPlane > 10f)
        {
            virtualCamera.m_Lens.NearClipPlane = Mathf.Lerp(virtualCamera.m_Lens.NearClipPlane, 1f, 0.5f * Time.deltaTime);
        }
        else
        {
            EnableScripts();
        }
    }

    private void EnableScripts()
    {
        tankMovementScript.enabled = true;
        weaponManagerScript.enabled = true;
        virtualCamera.m_Lens.NearClipPlane = 0.1f;
    }

    private IEnumerator WaitFor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        railTankScript.enabled = true;
        tankMovementScript.enabled = true;
        virtualCamera.m_Follow = spiderTank;
        virtualCamera.m_LookAt = spiderTank;
        mainCameraTransform.position = lastCameraPosition.position;
        Destroy(gameObject);
    }
}
