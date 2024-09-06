using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
public class GrabableInjection : IGrabable
{
    private Rigidbody rb;
    private Outlinable outlinable;
    private Transform transObject;
    private Transform transGrab;
    private BasicPlayerController playerController;

    public GrabableInjection(Rigidbody rigidbody, Outlinable outline, Transform transform, Transform grab, BasicPlayerController controller)
    {
        rb = rigidbody;
        outlinable = outline;
        transObject = transform;
        transGrab = grab;
        playerController = controller;
    }
    public void Grab()
    {
        rb.isKinematic = true;
        outlinable.enabled = false;
        transObject.parent = playerController.transGrab;
        transObject.localPosition = Vector3.zero;
        transObject.localRotation = Quaternion.identity;
        playerController.SetObjectGrab(transObject.gameObject);
    }
}
