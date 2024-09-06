using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlantSeed : MonoBehaviour, IDamageable
{
    [FoldoutGroup("Plant Seed")][SerializeField] private GameplayManager gameplayManager => GameplayManager.instance;
    [FoldoutGroup("Plant Seed")][SerializeField] private List<Seed> listSeed => gameplayManager.listSeed;
    [FoldoutGroup("Plant Seed")][SerializeField] private GameObject prefabSeed;
    [FoldoutGroup("Plant Seed")] public bool isActive = true;
    [FoldoutGroup("Plant Seed")][SerializeField] private Animator anim;
    [FoldoutGroup("Plant Seed")][SerializeField] private AudioSource audioSource;
    [FoldoutGroup("Plant Seed")][SerializeField] private List<AudioClip> listAudioHitImpact;
    [FoldoutGroup("Plant Seed")][SerializeField] private List<AudioClip> listAudioHitSlash;
    private void Awake()
    {
        if(!anim) anim = GetComponent<Animator>();
        if(!audioSource) audioSource = GetComponent<AudioSource>();
    }
    public void Damage(AttackObject attackObject)
    {
        switch (attackObject.typeAttack)
        {
            case TypeAttack.Impact:
                DamageImpact();
                break;
            case TypeAttack.Slash:
                DamageSlash();
                break;
        }
    }
    void DamageImpact()
    {
        anim.SetTrigger("Impact");
        int randomIndex = Random.Range(0, listAudioHitImpact.Count);
        audioSource.PlayOneShot(listAudioHitImpact[randomIndex]);
    }
    void DamageSlash()
    {
        anim.SetTrigger("Slash");
        int randomIndex = Random.Range(0, listAudioHitSlash.Count);
        audioSource.PlayOneShot(listAudioHitSlash[randomIndex]);
    }
    public void OnDeath()
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
    public void OnGrowth()
    {
        anim.SetTrigger("Growth");
        isActive = true;
    }
}
