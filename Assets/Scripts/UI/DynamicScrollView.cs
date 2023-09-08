using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicScrollView : MonoBehaviour
{

    [SerializeField]
    private Inventory playerInventory;

    [SerializeField]
    private Transform scrollViewContent;

    [SerializeField]
    private GameObject currentPrefab;

    [SerializeField]
    private List<WeaponDataSO> weaponEntries = new List<WeaponDataSO>();

    private void Update()
    {
        EventManager.OnAddItemToScrollView.AddListener(UpdateScrollView);
    }

    public void UpdateScrollView()
    {
        foreach (var inventoryItem in playerInventory.inventory)
        {
            if(!weaponEntries.Contains(inventoryItem.weaponData))
            {
                weaponEntries.Add(inventoryItem.weaponData);
                GameObject newWeaponEntry = Instantiate(currentPrefab, scrollViewContent);
                if(newWeaponEntry.TryGetComponent<WeaponEntry>(out WeaponEntry entry))
                {
                    entry.SetWeaponData(inventoryItem);
                }
            }
        }
    }
}
