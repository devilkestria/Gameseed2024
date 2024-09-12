using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    #region Singleton
    public static GameplayManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    #endregion

    public GameObject playerObj;
    public void SetPlayer(GameObject player)
    {
        playerObj = player;
    }

    [FoldoutGroup("Object Pulling")] public List<BasicPlant> listPlantSeed = new List<BasicPlant>();
    [FoldoutGroup("Object Pulling")] public List<Seed> listSeed = new List<Seed>();
    [FoldoutGroup("Object Pulling")] public List<RestoreHealth> listRestoreHealth = new List<RestoreHealth>();
    [FoldoutGroup("Object Pulling")] public List<PowerUp> listPowerUp = new List<PowerUp>();
    [FoldoutGroup("Object Pulling")] public List<GameObject> listEffect = new List<GameObject>();
    [FoldoutGroup("Object Pulling")] public List<BasicMonster> listMonster = new List<BasicMonster>();
    public UiBoard uiBoard;
    public GridManagement gridManagement;
    public BlackscreenManager bsManager;
    public StorageManagement storageManagement;
}
