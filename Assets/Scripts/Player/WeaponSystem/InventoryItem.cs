using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{
    public WeaponDataSO weaponData;
    public int stackSize;


    public InventoryItem(WeaponDataSO weapon)
    { 
        weaponData = weapon;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
