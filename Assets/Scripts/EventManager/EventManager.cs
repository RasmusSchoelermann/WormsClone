using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    //Weapon Inventory Events
    public static UnityEvent<WeaponDataSO, ulong> OnWeaponCollected = new UnityEvent<WeaponDataSO, ulong>();
    public static UnityEvent<WeaponDataSO, ulong> OnWeaponRemoved = new UnityEvent<WeaponDataSO, ulong>();

    public static UnityEvent<InventoryItem, ulong> OnWeaponEntryUpdate = new UnityEvent<InventoryItem, ulong>();
    //public static event UpdateWeaponPickup OnWeaponEntryUpdate;
    //public delegate void UpdateWeaponPickup(InventoryItem item, ulong ownerId);

    //Worm Weapon Equipment
    public static UnityEvent<WeaponDataSO> OnEquipSelectedWeapon = new UnityEvent<WeaponDataSO>();

    //Adding Weapon to Inventory UI
    public static UnityEvent OnAddItemToScrollView = new UnityEvent();



}
