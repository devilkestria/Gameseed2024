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
    public List<BasicPlant> listPlantSeed = new List<BasicPlant>();
    public List<Seed> listSeed = new List<Seed>();
    public List<RestoreHealth> listRestoreHealth = new List<RestoreHealth>();
    public List<PowerUp> listPowerUp = new List<PowerUp>();
    public List<GameObject> listEffect = new List<GameObject>();
    public List<BasicMonster> listMonster = new List<BasicMonster>();
    public UiBoard uiBoard;
    public GridManagement gridManagement;
    public StorageManagement storageManagement;
}
