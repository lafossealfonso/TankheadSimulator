using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSorter : MonoBehaviour
{
    public List<Camera> cameras;
    private int currentIndex = 0;

    void Start()
    {
        // Ensure only the first camera is enabled initially
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(i == currentIndex);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        // Deactivate the current camera
        cameras[currentIndex].gameObject.SetActive(false);

        // Move to the next camera in the list
        currentIndex = (currentIndex + 1) % cameras.Count;

        // Activate the new current camera
        cameras[currentIndex].gameObject.SetActive(true);
    }
}
