using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [FoldoutGroup("Change Camera")][SerializeField] private CinemachineVirtualCamera cameraTarget;
    [FoldoutGroup("Change Camera")][SerializeField] private CinemachineVirtualCamera playerCamera;
    [FoldoutGroup("Change Camera")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Change Camera")][SerializeField] private bool isChange;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerCam") && !isChange)
        {
            isChange = true;
            playerController.ChangeCamera(cameraTarget.transform);
            cameraTarget.Priority = 2;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("PlayerCam") && isChange)
        {
            isChange = false;
            playerController.ChangeCamera(playerCamera.transform);
            cameraTarget.Priority = 0;
        }
    }
}
