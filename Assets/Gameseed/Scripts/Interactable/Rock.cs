using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;
[RequireComponent(typeof(Outlinable))]
[RequireComponent(typeof(LineRenderer))]
public class Rock : MonoBehaviour, IInteractable, IThrowable
{
    [FoldoutGroup("Rock")][SerializeField] private GameObject player => GameplayManager.instance.playerObj;
    [FoldoutGroup("Rock")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Rock")][SerializeField] private Rigidbody rb;
    [FoldoutGroup("Rock")][SerializeField] private Outlinable outlinable;
    [FoldoutGroup("Rock")][SerializeField] private GameObject atkObj;
    [FoldoutGroup("Rock")][SerializeField] private float interactRadius;
    [FoldoutGroup("Rock")][SerializeField] private LayerMask layerInteract;
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
        transform.parent = null;
        onThrow = true;
        lineRenderer.enabled = false;
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
}
