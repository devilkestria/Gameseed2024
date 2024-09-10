using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridManagement : MonoBehaviour
{
    [Serializable]
    public class GridData
    {
        public Vector3Int gridPosition;
        public Vector3 worldPosition;
        public GameObject objAreaDig;
        public bool isOcupied;
        public Seed seed;
        public GridData(Vector3Int gridpos, Vector3 worldpos, GameObject obj, bool ocupied)
        {
            gridPosition = gridpos;
            worldPosition = worldpos;
            objAreaDig = obj;
            isOcupied = ocupied;
        }
    }
    [FoldoutGroup("Grid Management")] public Grid grid;
    [FoldoutGroup("Grid Management")] public List<GridData> listGridData = new List<GridData>();
    [FoldoutGroup("Grid Management")] public List<GameObject> listDigArea;
    [FoldoutGroup("Grid Management")][SerializeField] private GameObject prefabDigArea;

    public void AddAreaData(Vector3Int pos)
    {
        Vector3 worldPos = grid.CellToWorld(pos);
        GameObject digArea = GetAvaibleDigArea();
        if (digArea == null)
        {
            GameObject prefab = Instantiate(prefabDigArea);
            listDigArea.Add(prefab);
            digArea = prefab;
        }
        digArea.transform.SetPositionAndRotation(worldPos, Quaternion.identity);
        digArea.SetActive(true);
        listGridData.Add(new GridData(pos, worldPos, digArea, false));
    }
    public void RemoveAreaData(int index)
    {
        listGridData[index].objAreaDig.SetActive(false);
        if (listGridData[index].isOcupied)
            listGridData[index].seed.UnplantSeed();
        listGridData.RemoveAt(index);
    }
    public void PlantSeed(int index, Seed seed)
    {
        listGridData[index].seed = seed;
        listGridData[index].isOcupied = true;
    }
    public bool CheckAreaIsOcupied(Vector3Int area, out int index)
    {
        index = -1;
        bool result = false;
        for (int i = 0; i < listGridData.Count; i++)
        {
            if (listGridData[i].gridPosition == area)
            {
                result = true;
                index = i;
                break;
            }
        }
        return result;
    }
    public bool CheckPlayerCanPlantSeed(Vector3Int area, out int index)
    {
        bool result = false;
        index = -1;
        for (int i = 0; i < listGridData.Count; i++)
        {
            if (listGridData[i].gridPosition == area && !listGridData[i].isOcupied)
            {
                result = true;
                index = i;
                break;
            }
        }
        return result;
    }
    GameObject GetAvaibleDigArea()
    {
        foreach (GameObject obj in listDigArea)
        {
            if (!obj.activeSelf)
                return obj;
        }
        return null;
    }
}
