using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private void Update()
    {
        Vector3 dirToCamera = Camera.main.transform.position - transform.position;
        Vector3 lookDir = -dirToCamera;
        lookDir.y =0;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
