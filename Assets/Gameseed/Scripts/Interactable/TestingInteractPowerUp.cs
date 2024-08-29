using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestingInteractPowerUp : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Testing Interact Power Up")] private List<PowerUp> listPowerUp => GameplayManager.instance.listPowerUp;
    [FoldoutGroup("Testing Interact Power Up")][SerializeField] private Transform transSpawn;
    [FoldoutGroup("Testing Interact Power Up")][SerializeField] private GameObject prefabPowerUp;

    public void Interact()
    {
        ResetPowerUp();
    }
    void ResetPowerUp()
    {
        int index = -1;
        for (int i = 0; i < listPowerUp.Count; i++)
        {
            if (!listPowerUp[i].onActive)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            GameObject prefab = Instantiate(prefabPowerUp, transSpawn.position, transSpawn.rotation);
            PowerUp powerUp = prefab.GetComponent<PowerUp>();
            if (!GameplayManager.instance.listPowerUp.Contains(powerUp))
                GameplayManager.instance.listPowerUp.Add(powerUp);
            index = listPowerUp.Count - 1;
        }
        listPowerUp[index].transform.position = transSpawn.position;
        listPowerUp[index].transform.rotation = transSpawn.rotation;
        listPowerUp[index].gameObject.SetActive(true);
    }
}
