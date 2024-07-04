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

    private void Start()
    {
        originalPosition = transform.position;
        objectToActivate.SetActive(false); // Ensure the object to activate starts deactivated
        railTankScript.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            // Activate the object above its original position
            objectToActivate.transform.position = originalPosition + Vector3.up * height;
            objectToActivate.SetActive(true);
            isActivated = true;
        }
    }

    private void Update()
    {
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
                railTankScript.enabled = true;
            }
        }
    }
}
