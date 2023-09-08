using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();
    private Dictionary<WeaponDataSO, InventoryItem> _itemDictionary = new Dictionary<WeaponDataSO, InventoryItem>();

    [SerializeField]
    private PlayerNetworkObject playerRef;

    private void Update()
    {
        EventManager.OnWeaponCollected.AddListener(Add);
    }

    public void Add(WeaponDataSO weaponData, ulong ownerId)
    {
        if (ownerId != playerRef.ownerId)
            return;
        if (!playerRef.eventTrigger)
        {
            playerRef.eventTrigger = true;
            if (_itemDictionary.TryGetValue(weaponData, out InventoryItem item))
            {
                item.AddToStack();
                Debug.Log("Added to Stack: " + item.stackSize.ToString());
                EventManager.OnWeaponEntryUpdate?.Invoke(item, ownerId);
            }
            else
            {
                InventoryItem newItem = new InventoryItem(weaponData);
                inventory.Add(newItem);
                Debug.Log("New: " + newItem.weaponData.weaponName);
                _itemDictionary.Add(weaponData, newItem);
                EventManager.OnAddItemToScrollView.Invoke();
            }
        }
    }

    public void Remove(WeaponDataSO weaponData, ulong ownerId)
    {
        if (ownerId != playerRef.ownerId)
            return;

        if (_itemDictionary.TryGetValue(weaponData, out InventoryItem item))
        {
            item.RemoveFromStack();
            EventManager.OnWeaponEntryUpdate?.Invoke(item, ownerId);
            if (item.stackSize == 0)
            {
                inventory.Remove(item);
                _itemDictionary.Remove(weaponData);
            }
        }
    }

    public InventoryItem GetInventoryItem(WeaponDataSO weaponData)
    {
        if(_itemDictionary.TryGetValue(weaponData, out InventoryItem item))
        {
            return item;
        }
        return null;
    }

}
