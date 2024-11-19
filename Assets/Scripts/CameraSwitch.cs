using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineFreeLook freeLookCamera;

    private bool isUsingVirtualCamera = true;

    void Start()
    {
        // Initialize both cameras with priority 5
        virtualCamera.Priority = 5;
        freeLookCamera.Priority = 5;
        SwitchToVirtualCamera(); // Start with the virtual camera active
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // Use any key or button for switching
        {
            if (isUsingVirtualCamera)
            {
                SwitchToFreeLookCamera();
            }
            else
            {
                SwitchToVirtualCamera();
            }
        }
    }

    void SwitchToVirtualCamera()
    {
        virtualCamera.Priority = 6;  // Set higher than freeLookCamera (5) but less than dolly camera (10)
        freeLookCamera.Priority = 5; // Lower priority for freeLookCamera
        isUsingVirtualCamera = true;
    }

    void SwitchToFreeLookCamera()
    {
        virtualCamera.Priority = 5;  // Lower priority for virtualCamera
        freeLookCamera.Priority = 6; // Set higher than virtualCamera (5) but less than dolly camera (10)
        isUsingVirtualCamera = false;
    }
}
