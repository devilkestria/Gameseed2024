using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Outlinable))]
[RequireComponent(typeof(LineRenderer))]
public class Seed : MonoBehaviour, IInteractable, IThrowable, IDamageable, IMusicable
{
    [FoldoutGroup("Seed")][SerializeField] private GameObject player => GameplayManager.instance.playerObj;
    [FoldoutGroup("Seed")] public bool isActive = false;
    [FoldoutGroup("Seed")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Seed")] public Rigidbody rb;
    [FoldoutGroup("Seed")][SerializeField] private Outlinable outlinable;
    [FoldoutGroup("Seed")][SerializeField] private GameObject atkObj;
    [FoldoutGroup("Seed")][SerializeField] private float interactRadius;
    [FoldoutGroup("Seed")][SerializeField] private LayerMask layerInteract;
    [FoldoutGroup("Seed")][SerializeField] private AudioSource sfxAudioSource;
    [FoldoutGroup("Grab")] private bool onGrabed;
    [FoldoutGroup("Grab")] private IGrabable grabable;
    [FoldoutGroup("Throw")] private bool onThrow;
    [FoldoutGroup("Throw")] private IThrowable throwable;
    [FoldoutGroup("Throw")][SerializeField] private float throwDistance;
    [FoldoutGroup("Throw")][SerializeField] private float throwHeight;
    [FoldoutGroup("Throw")][SerializeField] private int resolution = 30;
    [FoldoutGroup("Throw")][SerializeField] LineRenderer lineRenderer;
    [FoldoutGroup("Plant")] public bool onPlant;
    [FoldoutGroup("Plant")][SerializeField] private GridManagement gridManagement => GameplayManager.instance.gridManagement;
    [FoldoutGroup("Plant")][SerializeField] private Grid grid => gridManagement.grid;
    [FoldoutGroup("Plant")][SerializeField] private int indexGrid;
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
            if (!lineRenderer.enabled) lineRenderer.enabled = true;
            DrawTrijectory();
        }
        if (onGrabed || onThrow || onPlant) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRadius, layerInteract);
        if (colliders.Length > 0)
        {
            if (colliders[0].CompareTag("Player"))
            {
                if (!outlinable.enabled) outlinable.enabled = true;
                return;
            }
        }
        if (outlinable.enabled) outlinable.enabled = false;
        if (lineRenderer.enabled) lineRenderer.enabled = false;
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
            onThrow = false;
            atkObj.SetActive(false);
        }
    }
    public void Interact()
    {
        if (onPlant) return;
        Grab();
    }

    public void Grab()
    {
        outlinable.enabled = false;
        onGrabed = true;
        grabable.Grab();
    }
    public void Throw()
    {
        onGrabed = false;
        if (lineRenderer.enabled) lineRenderer.enabled = false;
        transform.parent = null;
        onThrow = true;
        atkObj.SetActive(true);
        throwable.Throw();
    }
    public void PlantSeed(int index)
    {
        onGrabed = false;
        isActive = true;
        onPlant = true;
        if (lineRenderer.enabled) lineRenderer.enabled = false;
        transform.parent = null;
        Vector3 offset = new Vector3(grid.cellSize.x / 2 + 0.35f, 0, grid.cellSize.z / 2);
        transform.position = gridManagement.listGridData[index].worldPosition + offset;
        transform.eulerAngles = new Vector3(0, 0, 90);
        gridManagement.PlantSeed(index, this);
    }
    public void UnplantSeed()
    {
        isActive = false;
        onPlant = false;
        gameObject.SetActive(false);
    }

    #region Musicable
    [FoldoutGroup("Musicable")][SerializeField] private GameObject prefabPlantSeed;
    [FoldoutGroup("Musicable")][SerializeField] private GameObject prefabSunFlowerTree;
    [FoldoutGroup("Musicable")][SerializeField] private GameObject prefabMushroom;
    public void Music(MusicData data)
    {
        if (!onPlant) return;
        string plantName;
        GameObject tree;
        switch (data.soundName)
        {
            case "Music 1":
                plantName = "Basic Plant";
                tree = prefabPlantSeed;
                break;
            case "Music 2":
                plantName = "Sunflower";
                tree = prefabSunFlowerTree;
                break;
            case "Music 3":
                plantName = "Mushroom";
                tree = prefabMushroom;
                break;
            default:
                plantName = "Basic Plant";
                tree = prefabPlantSeed;
                break;
        }
        BasicPlant plantSeed = GetAvaibleTree(plantName);
        if (plantSeed == null)
        {
            plantSeed = Instantiate(tree, transform.position, Quaternion.identity).GetComponent<BasicPlant>();
            if (!GameplayManager.instance.listPlantSeed.Contains(plantSeed))
                GameplayManager.instance.listPlantSeed.Add(plantSeed);
        }
        plantSeed.OnGrowth(transform.position);
        gridManagement.RemoveAreaData(indexGrid);
    }
    BasicPlant GetAvaibleTree(string name)
    {
        for (int i = 0; i < GameplayManager.instance.listPlantSeed.Count; i++)
        {
            BasicPlant tree = GameplayManager.instance.listPlantSeed[i];
            if (!tree.isActive && tree.plantName == name)
            {
                return tree;
            }
        }
        return null;
    }
    #endregion

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

    [FoldoutGroup("Damage")] private List<RestoreHealth> listHealth => GameplayManager.instance.listRestoreHealth;
    [FoldoutGroup("Damage")][SerializeField] GameObject prefabHealth;
    [FoldoutGroup("Damage")][SerializeField] float radiusHealthJump = 2f;
    [FoldoutGroup("Damage")][SerializeField] float healthJumpForce = 5f;
    public void Damage(AttackObject attackObject, AudioClip audioClip)
    {
        if (onPlant) return;
        int index = -1;
        for (int i = 0; i < listHealth.Count; i++)
        {
            if (!listHealth[i].onActive)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            GameObject prefab = Instantiate(prefabHealth, transform.position, transform.rotation);
            RestoreHealth hp = prefab.GetComponent<RestoreHealth>();
            if (!GameplayManager.instance.listRestoreHealth.Contains(hp))
                GameplayManager.instance.listRestoreHealth.Add(hp);
            index = listHealth.Count - 1;
        }
        sfxAudioSource.PlayOneShot(audioClip);
        listHealth[index].transform.position = transform.position;
        listHealth[index].transform.rotation = transform.rotation;
        listHealth[index].gameObject.SetActive(true);
        listHealth[index].onActive = true;
        listHealth[index].rb.isKinematic = false;
        listHealth[index].rb.useGravity = true;
        Vector3 randomPosition = listHealth[index].transform.position + Random.onUnitSphere * radiusHealthJump;
        Vector3 directionToCenter = (listHealth[index].transform.position - randomPosition).normalized;
        Vector3 upwardForce = directionToCenter * healthJumpForce;
        listHealth[index].rb.AddForceAtPosition(upwardForce, randomPosition, ForceMode.Impulse);
        if (!GameplayManager.instance.listSeed.Contains(this))
            GameplayManager.instance.listSeed.Add(this);
        isActive = false;
        gameObject.SetActive(false);
    }

}
