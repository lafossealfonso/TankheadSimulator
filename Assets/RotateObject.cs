using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float rotationSpeed = 45f;

    // Axis of rotation
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rotate the object around the specified axis at the specified speed
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
