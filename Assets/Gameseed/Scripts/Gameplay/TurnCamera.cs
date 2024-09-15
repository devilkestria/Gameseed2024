using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TurnCamera : MonoBehaviour
{
    [FoldoutGroup("Turn Camera")][SerializeField] private Transform transCamera;
    [FoldoutGroup("Turn Camera")][SerializeField] private Vector3 targetRotation;
    [FoldoutGroup("Turn Camera")][SerializeField] private float timeTurn;
    [FoldoutGroup("Turn Camera")] private float deltaTimeTurn;
    [FoldoutGroup("Turn Camera")] private Quaternion targetQuat;
    [FoldoutGroup("Turn Camera")] private Coroutine corouTurning;
    void Start()
    {
        if (!transCamera) transCamera = GameplayManager.instance.cameraPlayerObj.transform;
        targetQuat = Quaternion.Euler(targetRotation);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCam"))
        {
            if (transCamera.rotation == targetQuat && corouTurning != null) return;
            deltaTimeTurn = 0;
            corouTurning = StartCoroutine(IeTurnCamera(targetQuat));
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
        corouTurning = null;
    }
}
