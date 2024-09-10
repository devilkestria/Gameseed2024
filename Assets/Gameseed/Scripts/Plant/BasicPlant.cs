using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BasicPlant : MonoBehaviour, IDamageable
{
    [FoldoutGroup("Plant Seed")] public GameplayManager gameplayManager => GameplayManager.instance;
    [FoldoutGroup("Plant Seed")] public string plantName;
    [FoldoutGroup("Plant Seed")] public List<Seed> listSeed => gameplayManager.listSeed;
    [FoldoutGroup("Plant Seed")] public GameObject prefabSeed;
    [FoldoutGroup("Plant Seed")] public bool isActive = true;
    [FoldoutGroup("Plant Seed")][SerializeField] private Animator anim;
    [FoldoutGroup("Plant Seed")][SerializeField] private AudioSource audioSource;
    private void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }
    public void Damage(AttackObject attackObject, AudioClip audioClip)
    {
        switch (attackObject.typeAttack)
        {
            case TypeAttack.Impact:
                anim.SetTrigger("Impact");
                break;
            case TypeAttack.Slash:
                anim.SetTrigger("Slash");
                break;
        }
        audioSource.PlayOneShot(audioClip);
    }
    public virtual void OnDeath()
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
        if (!gameplayManager.listPlantSeed.Contains(this))
            gameplayManager.listPlantSeed.Add(this);
        isActive = false;
        gameObject.SetActive(false);
    }
    public virtual void OnGrowth(Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        isActive = true;
        anim.SetTrigger("Growth");
    }
}
