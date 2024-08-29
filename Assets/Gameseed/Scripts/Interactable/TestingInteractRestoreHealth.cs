using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestingInteractRestoreHealth : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Testing Restore Health")] private List<RestoreHealth> listRestoreHealth => GameplayManager.instance.listRestoreHealth;
    [FoldoutGroup("Testing Restore Health")][SerializeField] private Transform transSpawn;
    [FoldoutGroup("Testing Restore Health")][SerializeField] private GameObject prefabRestoreHealth;
    public void Interact()
    {
        ResetRestoreHealth();
    }
    void ResetRestoreHealth()
    {
        int index = -1;
        for (int i = 0; i < listRestoreHealth.Count; i++)
        {
            if (!listRestoreHealth[i].onActive)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            GameObject prefab = Instantiate(prefabRestoreHealth, transSpawn.position, transSpawn.rotation);
            RestoreHealth hp = prefab.GetComponent<RestoreHealth>();
            if (!GameplayManager.instance.listRestoreHealth.Contains(hp))
                GameplayManager.instance.listRestoreHealth.Add(hp);
            index = listRestoreHealth.Count - 1;
        }
        listRestoreHealth[index].transform.position = transSpawn.position;
        listRestoreHealth[index].transform.rotation = transSpawn.rotation;
        listRestoreHealth[index].gameObject.SetActive(true);
    }
}
