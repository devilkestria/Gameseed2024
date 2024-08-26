using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public abstract class Damaging : MonoBehaviour
{
    private List<IObserverDamaging> _observer = new List<IObserverDamaging>();
    public void AddObserver(IObserverDamaging objserverDamaging)
    {
        _observer.Add(objserverDamaging);
    }
    public void RemoveObserver(IObserverDamaging observerDamaging)
    {
        _observer.Remove(observerDamaging);
    }
    protected void OnDamaging()
    {
        throw new System.NotImplementedException();
    }
}
