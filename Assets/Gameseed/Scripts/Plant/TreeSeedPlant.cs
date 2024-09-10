using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSeedPlant : BasicPlant
{
    public void SpawnSeed()
    {
        int index = -1;
        for (int i = 0; i < listSeed.Count; i++)
        {
            if (!listSeed[i].isActive)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            GameObject prefab = Instantiate(prefabSeed, transform.position, transform.rotation);
            Seed seed = prefab.GetComponent<Seed>();
            if (!GameplayManager.instance.listSeed.Contains(seed))
                GameplayManager.instance.listSeed.Add(seed);
            index = listSeed.Count - 1;
        }
        listSeed[index].transform.position = transform.position;
        listSeed[index].transform.rotation = transform.rotation;
        listSeed[index].gameObject.SetActive(true);
        listSeed[index].isActive = true;
        listSeed[index].rb.isKinematic = false;
        listSeed[index].rb.useGravity = true;
        Vector3 randomPosition = listSeed[index].transform.position + Random.onUnitSphere * 1f;
        Vector3 directionToCenter = (listSeed[index].transform.position - randomPosition).normalized;
        Vector3 upwardForce = directionToCenter * 10f;
        listSeed[index].rb.AddForceAtPosition(upwardForce, randomPosition, ForceMode.Impulse);
    }
}
