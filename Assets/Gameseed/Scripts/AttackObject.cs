using Sirenix.OdinInspector;
using UnityEngine;
public class AttackObject : MonoBehaviour
{
    [FoldoutGroup("Attack Object")] public bool onActive = false;
    [FoldoutGroup("Attack Object")] public int IdAtkObject;
    [FoldoutGroup("Attack Object")] public float damageValue;
    [FoldoutGroup("Attack Object")][SerializeField] private float timeDisapear;
    [FoldoutGroup("Attack Object")] private float deltaTimeDisapear;
    [FoldoutGroup("Attack Object")][SerializeField] private Vector3 forwardDirection;
    [FoldoutGroup("Attack Object")][SerializeField] private float forceForward;
    [FoldoutGroup("Attack Object")] public float forcePush;
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
            if (deltaTimeDisapear < timeDisapear)
            {
                deltaTimeDisapear += Time.deltaTime;
            }
            else
            {
                onActive = false;
                gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Utility.CheckHitable(tag, other.tag))
        {
            other.GetComponent<Status>().OnDamage(this);
            onActive = false;
            gameObject.SetActive(false);
        }
    }
}
