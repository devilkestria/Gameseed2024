using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectItem : MonoBehaviour
{
    [SerializeField] private Item item;
    Item currentItem;
    StorageManagement storageManagement;
    private void Start()
    {
        if (!item) item = GetComponent<Item>();
        if (!storageManagement) storageManagement = GameplayManager.instance.storageManagement;
        currentItem = new Item(item.IdItem, item.itemType, item.itemTypeEquip, item.itemImage, item.itemQuantity);
    }
    private void OnTriggerEnter(Collider other)
    {
        storageManagement.AddItem(currentItem);
        gameObject.SetActive(false);
    }
}
