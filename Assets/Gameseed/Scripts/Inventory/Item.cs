using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeItem { Equipment, Consumable }
public enum TypeEquipment { Head, Body, LeftArm, RightArm, LeftLeg, RightLeg, NotEquipment }
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class Item : ScriptableObject
{
    public string IdItem;
    public TypeItem itemType;
    public TypeEquipment itemTypeEquip;
    public Sprite itemImage;
    public int itemQuantity;

    public Item(string id, TypeItem item, TypeEquipment equip, Sprite spr, int quantity)
    {
        IdItem = id;
        itemType = item;
        itemTypeEquip = equip;
        itemImage = spr;
        itemQuantity = quantity;
    }
}
