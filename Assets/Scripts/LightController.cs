using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] Transform RLight;
    [SerializeField] Transform LLight;

    [SerializeField] GameObject RLightDisplay;
    [SerializeField] GameObject LLightDisplay;

    private bool rightLightStatus = true;
    private bool leftLightStatus = true;

    private void Update()
    {

        CheckLightStatus();

        UpdateLights();

    }

    private void CheckLightStatus()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            rightLightStatus = !rightLightStatus;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            leftLightStatus = !leftLightStatus;
        }
    }

    private void UpdateLights()
    {
        RLight.gameObject.SetActive(rightLightStatus);
        LLight.gameObject.SetActive(leftLightStatus);
        RLightDisplay.gameObject.SetActive(rightLightStatus);
        LLightDisplay.gameObject.SetActive(leftLightStatus);
    }

}
