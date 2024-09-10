using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MushroomPlant : BasicPlant
{
    public override void OnDeath()
    {
        MushroomDeath();
    }
    public void MushroomDeath()
    {
        if (!gameplayManager.listPlantSeed.Contains(this))
            gameplayManager.listPlantSeed.Add(this);
        isActive = false;
        gameObject.SetActive(false);
    }

}
