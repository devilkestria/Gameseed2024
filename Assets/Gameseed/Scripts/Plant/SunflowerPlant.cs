using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunflowerPlant : BasicPlant
{
    public override void OnDeath()
    {
        SunflowerDeath();
    }
    public void SunflowerDeath()
    {
        if (!gameplayManager.listPlantSeed.Contains(this))
            gameplayManager.listPlantSeed.Add(this);
        isActive = false;
        gameObject.SetActive(false);
    }
}
