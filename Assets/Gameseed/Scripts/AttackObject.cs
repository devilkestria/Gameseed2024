using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
public class AttackObject : MonoBehaviour
{
    [FoldoutGroup("Attack Object")] public bool onActive = false;
    [FoldoutGroup("Attack Object")] public string nameAtk;
    [FoldoutGroup("Attack Object")] public float damageValue;
    [FoldoutGroup("Attack Object")][SerializeField] private float timeDisapear;
    [FoldoutGroup("Attack Object")] private float deltaTimeDisapear;
    [FoldoutGroup("Attack Object")][SerializeField] private Vector3 forwardDirection;
    [FoldoutGroup("Attack Object")][SerializeField] private float forceForward;
    [FoldoutGroup("Attack Object")] public float forcePush;
    [FoldoutGroup("Attack Object")][SerializeField] private bool isPenetration;
    [FoldoutGroup("Attack Object")][SerializeField] GameObject prefabEffectOnDestroy;
    [FoldoutGroup("Attack Object")] public TypeAttack typeAttack;
    [FoldoutGroup("Attack Object/Sfx")] public List<AudioClip> listAudioOnHit;

    private void OnEnable()
    {
        onActive = true;
        deltaTimeDisapear = 0;
    }
    private void Update()
    {
        if (onActive)
        {
            if (forceForward > 0)
            {
                transform.Translate(forwardDirection * forceForward * Time.deltaTime);
            }
            if (timeDisapear <= 0) return;
            if (deltaTimeDisapear < timeDisapear)
            {
                deltaTimeDisapear += Time.deltaTime;
            }
            else
            {
                onActive = false;
                if (transform.parent != null)
                    transform.parent.gameObject.SetActive(false);
                else
                    gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Tag : " + tag + " | Other : " + other.tag);
        if (Utility.CheckHitable(tag, other.tag))
        {
            IDamageable[] dmgables = other.GetComponents<IDamageable>();
            int random = Random.Range(0, listAudioOnHit.Count);
            for (int i = 0; i < dmgables.Length; i++)
                dmgables[i].Damage(this, listAudioOnHit[random]);
            if (!isPenetration)
            {
                onActive = false;
                OnDeath();
                if (transform.parent != null)
                    transform.parent.gameObject.SetActive(false);
                else
                    gameObject.SetActive(false);
            }
        }
    }
    public virtual void OnDeath()
    {
        if (prefabEffectOnDestroy)
            Instantiate(prefabEffectOnDestroy, transform.position, Quaternion.identity);
    }
}
