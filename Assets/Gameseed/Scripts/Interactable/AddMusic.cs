using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;

public class AddMusic : MonoBehaviour, IInteractable
{
    [FoldoutGroup("Add Music")][SerializeField] private MusicData musicData;
    [FoldoutGroup("Add Music")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Add Music")][SerializeField] private float interactRadius;
    [FoldoutGroup("Add Music")][SerializeField] private LayerMask layerInteract;
    [FoldoutGroup("Add Music")][SerializeField] private GameObject objInteract;
    [FoldoutGroup("Add Music")][SerializeField] private Outlinable outlinable;
    
    private void Start()
    {
        if (!outlinable) outlinable = GetComponent<Outlinable>();
        if (!playerController) playerController = GameplayManager.instance.playerObj.GetComponent<BasicPlayerController>();
    }

    public void Interact()
    {
        playerController.AddNewMusic(musicData);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, layerInteract);
        if (colliders.Length > 0)
        {
            if (colliders[0].CompareTag("Player"))
            {
                outlinable.enabled = true;
                objInteract.SetActive(true);
                return;
            }
        }
        outlinable.enabled = false;
        objInteract.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
