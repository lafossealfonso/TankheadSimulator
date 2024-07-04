using UnityEngine;
using UnityEngine.UI;

public class SpriteFollow : MonoBehaviour
{
    public Transform target; // The object to follow
    public Image uiImage; // The UI Image component to display the sprite

    private Camera mainCamera;
    private RectTransform canvasRectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        canvasRectTransform = uiImage.canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null || uiImage == null || mainCamera == null || canvasRectTransform == null)
            return;

        // Convert the target's world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);

        // Check if the target is in front of the camera
        if (screenPosition.z > 0)
        {
            // Convert the screen position to canvas position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, mainCamera, out Vector2 canvasPosition);

            // Set the UI Image's position
            uiImage.rectTransform.anchoredPosition = canvasPosition;
            uiImage.enabled = true; // Make sure the image is enabled
        }
        else
        {
            // If the target is behind the camera, hide the image
            uiImage.enabled = false;
        }
    }
}
