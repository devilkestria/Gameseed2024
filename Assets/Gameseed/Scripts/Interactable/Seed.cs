using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Outlinable))]
[RequireComponent(typeof(LineRenderer))]
public class Seed : MonoBehaviour, IInteractable, IThrowable, IDamageable
{
    [FoldoutGroup("Seed")][SerializeField] private GameObject player => GameplayManager.instance.playerObj;
    [FoldoutGroup("Seed")] public bool isActive = false;
    [FoldoutGroup("Seed")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Seed")] public Rigidbody rb;
    [FoldoutGroup("Seed")][SerializeField] private Outlinable outlinable;
    [FoldoutGroup("Seed")][SerializeField] private GameObject atkObj;
    [FoldoutGroup("Seed")][SerializeField] private float interactRadius;
    [FoldoutGroup("Seed")][SerializeField] private LayerMask layerInteract;
    [FoldoutGroup("Grab")] private bool onGrabed;
    [FoldoutGroup("Grab")] private IGrabable grabable;
    [FoldoutGroup("Throw")] private bool onThrow;
    [FoldoutGroup("Throw")] private IThrowable throwable;
    [FoldoutGroup("Throw")][SerializeField] private float throwDistance;
    [FoldoutGroup("Throw")][SerializeField] private float throwHeight;
    [FoldoutGroup("Throw")][SerializeField] private int resolution = 30;
    [FoldoutGroup("Throw")][SerializeField] LineRenderer lineRenderer;
    private void Start()
    {
        if (!outlinable) outlinable = GetComponent<Outlinable>();
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        if (!playerController) playerController = player.GetComponent<BasicPlayerController>();
        lineRenderer.positionCount = resolution + 1;
        grabable = new GrabableInjection(rb, outlinable, transform, playerController.transGrab, playerController);
        throwable = new ThrowableInjection(rb, transform, throwDistance, throwHeight);
    }
    private void Update()
    {
        if (onGrabed)
        {
            lineRenderer.enabled = true;
            DrawTrijectory();
        }
        if (onGrabed || onThrow) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, layerInteract);
        if (colliders.Length > 0)
        {
            if (colliders[0].CompareTag("Player"))
            {
                outlinable.enabled = true;
                return;
            }
        }
        outlinable.enabled = false;
        lineRenderer.enabled = false;
    }
    private void FixedUpdate()
    {
        if (onThrow && throwable is ThrowableInjection strategy)
        {
            strategy.ApplyGravity();
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (!onThrow) return;
        if (other.gameObject.CompareTag("Ground"))
        {
            rb.isKinematic = true;
            rb.useGravity = true;
            onThrow = false;
            atkObj.SetActive(false);
        }
    }
    public void Grab()
    {
        onGrabed = true;
        grabable.Grab();
    }

    public void Interact()
    {
        Grab();
    }
    public void Throw()
    {
        onGrabed = false;
        lineRenderer.enabled = false;
        transform.parent = null;
        onThrow = true;
        atkObj.SetActive(true);
        throwable.Throw();
    }
    void DrawTrijectory()
    {
        Vector3[] points = new Vector3[resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;
            points[i] = CalculateParabolaPoint(t);
        }
        lineRenderer.SetPositions(points);
    }
    Vector3 CalculateParabolaPoint(float t)
    {
        float x = t * throwDistance;
        float y = 4 * throwHeight * t * (1 - t);
        Vector3 point = transform.position + transform.forward.normalized * x + Vector3.up * y;
        return point;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    [FoldoutGroup("Damage")] private List<RestoreHealth> listRestoreHealth => GameplayManager.instance.listRestoreHealth;
    [FoldoutGroup("Damage")][SerializeField] GameObject prefabRestoreHealth;
    public void Damage(AttackObject attackObject)
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
        listRestoreHealth[index].onActive = true;
        listRestoreHealth[index].rb.isKinematic = false;
        listRestoreHealth[index].rb.useGravity = true;
        Vector3 randomPosition = listRestoreHealth[index].transform.position + Random.onUnitSphere * 0.5f;
        Vector3 directionToCenter = (listRestoreHealth[index].transform.position - randomPosition).normalized;
        Vector3 upwardForce = directionToCenter * 1f;
        listRestoreHealth[index].rb.AddForceAtPosition(upwardForce, randomPosition, ForceMode.Impulse);
        if (!GameplayManager.instance.listSeed.Contains(this))
            GameplayManager.instance.listSeed.Add(this);
        isActive = false;
        gameObject.SetActive(false);
    }
}
