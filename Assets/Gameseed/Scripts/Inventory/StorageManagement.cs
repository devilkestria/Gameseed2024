using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageManagement : MonoBehaviour
{
    public List<Item> listItem;
    public List<GameObject> listShowItem;
    public GameObject prefabItem;
    public Transform transItem;
    public void AddItem(Item item)
    {
        switch (item.itemType)
        {
            case TypeItem.Equipment:
                ShowItem(item);
                break;
            case TypeItem.Consumable:
                if (CheckIdItemContains(item))
                {
                    AddQuantityItem(item);
                }
                else
                {
                    ShowItem(item);
                    ShowQuantity(item);
                }
                break;
        }
    }
    bool CheckIdItemContains(Item item)
    {
        bool result = false;
        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i].IdItem == item.IdItem)
            {
                result = true;
                break;
            }
        }
        return result;
    }
    void ShowItem(Item item)
    {
        GameObject prefab = Instantiate(prefabItem, transItem);
        Image imgprefab = prefab.GetComponent<Image>();
        imgprefab.sprite = item.itemImage;
        listItem.Add(new Item(item.IdItem, item.itemType, item.itemTypeEquip, item.itemImage, item.itemQuantity));
        listShowItem.Add(prefab);
    }
    void ShowQuantity(Item item)
    {
        int index = -1;
        // index = listItem.FindIndex((x) => x == item);
        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i].IdItem == item.IdItem)
            {
                index = i;
                break;
            }
        }
        listShowItem[index].transform.GetChild(0).gameObject.SetActive(true);
        listShowItem[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemQuantity.ToString();
    }
    void AddQuantityItem(Item item)
    {
        int index = -1;
        // index = listItem.FindIndex((x) => x == item);
        for (int i = 0; i < listItem.Count; i++)
        {
            if (listItem[i].IdItem == item.IdItem)
            {
                index = i;
                break;
            }
        }
        listItem[index].itemQuantity += item.itemQuantity;
        ShowQuantity(listItem[index]);
    }
    public void RemoveItem()
    {

    }

    [SerializeField] private GameObject panelStorage;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            panelStorage.SetActive(!panelStorage.activeSelf);
        }
    }
}
