using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TurnTemporaryCamera : MonoBehaviour
{
    [FoldoutGroup("Turn Camera")][SerializeField] private Transform transCamera;
    [FoldoutGroup("Turn Camera")][SerializeField] private bool isInside;
    [FoldoutGroup("Turn Camera")] private Vector3 originRotation;
    [FoldoutGroup("Turn Camera")][SerializeField] private Vector3 targetRotation;
    [FoldoutGroup("Turn Camera")][SerializeField] private float timeTurn;
    [FoldoutGroup("Turn Camera")] private float deltaTimeTurn;
    [FoldoutGroup("Turn Camera")] private Quaternion targetQuat;
    void Start()
    {
        if (!transCamera) transCamera = GameplayManager.instance.cameraPlayerObj.transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCam") && !isInside)
        {
            originRotation = transCamera.rotation.eulerAngles;
            isInside = true;
            deltaTimeTurn = 0;
            targetQuat = Quaternion.Euler(targetRotation);
            StopAllCoroutines();
            StartCoroutine(IeTurnCamera(targetQuat));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerCam") && isInside)
        {
            isInside = false;
            deltaTimeTurn = 0;
            targetQuat = Quaternion.Euler(originRotation);
            StopAllCoroutines();
            StartCoroutine(IeTurnCamera(targetQuat));
        }
    }
    IEnumerator IeTurnCamera(Quaternion targetRotation)
    {
        while (deltaTimeTurn < timeTurn)
        {
            transCamera.rotation = Quaternion.Slerp(transCamera.rotation, targetQuat, deltaTimeTurn / timeTurn);
            deltaTimeTurn += Time.deltaTime;
            yield return null;
        }
        transCamera.rotation = targetQuat;
    }

}
