using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AOSeed : AttackObject
{
    [FoldoutGroup("Attack Object")][SerializeField] List<RestoreHealth> listRestoreHealth => GameplayManager.instance.listRestoreHealth;
    [FoldoutGroup("AO Seed")] [SerializeField] private GameObject prefabRestoreHealth;

    public override void OnDeath()
    {
        PopOutRestoreHealth();
    }

    void PopOutRestoreHealth()
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
            GameObject prefab = Instantiate(prefabRestoreHealth, transform.position, transform.rotation);
            RestoreHealth hp = prefab.GetComponent<RestoreHealth>();
            if (!GameplayManager.instance.listRestoreHealth.Contains(hp))
                GameplayManager.instance.listRestoreHealth.Add(hp);
            index = listRestoreHealth.Count - 1;
        }
        listRestoreHealth[index].transform.position = transform.position;
        listRestoreHealth[index].transform.rotation = transform.rotation;
        listRestoreHealth[index].gameObject.SetActive(true);
        Vector3 randomPosition = listRestoreHealth[index].transform.position + Random.onUnitSphere * 0.5f;
        Vector3 directionToCenter = (listRestoreHealth[index].transform.position - randomPosition).normalized;
        Vector3 upwardForce = directionToCenter * 1f;
        listRestoreHealth[index].rb.AddForceAtPosition(upwardForce, randomPosition, ForceMode.Impulse);
    }
}
