using System.Collections;
using System.Collections.Generic;
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
    public List<PlantSeed> listPlantSeed = new List<PlantSeed>();
    public List<Seed> listSeed = new List<Seed>();
    public List<RestoreHealth> listRestoreHealth = new List<RestoreHealth>();
    public List<PowerUp> listPowerUp = new List<PowerUp>();
    public List<GameObject> listEffect = new List<GameObject>();
    public UiBoard uiBoard;
    public StorageManagement storageManagement;
}
