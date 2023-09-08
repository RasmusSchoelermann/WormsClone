using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponEntry : MonoBehaviour
{
    public Button entryButton;
    public Image entryIcon;
    public TextMeshProUGUI entryWeaponName;
    public TextMeshProUGUI entryWeaponStackSize;

    public InventoryItem savedWeapon;

    public static event SelectedWeaponEquip OnEquipSelectedWeapon;
    public delegate void SelectedWeaponEquip(WeaponDataSO weaponDataSo);

    private void Update()
    {
        EventManager.OnWeaponEntryUpdate.AddListener(UpdateStackSize);
    }

    private void UpdateStackSize(InventoryItem item, ulong ownerId)
    {
        if (item.weaponData.weaponName != savedWeapon.weaponData.weaponName)
            return;

        if(ownerId != gameObject.GetComponentInParent<PlayerNetworkObject>().ownerId)
            return;

        savedWeapon = item;
        ChangeStackSizeText();
        
    }

    public void SetWeaponData(InventoryItem inventoryItem)
    {
        savedWeapon = inventoryItem;

        ChangeUiText(savedWeapon.weaponData.weaponName);
        ChangeIcon(savedWeapon.weaponData.icon);
    }

    public void EqiupWeapon()
    {
        OnEquipSelectedWeapon?.Invoke(savedWeapon.weaponData);
    }

    private void ChangeIcon(Sprite image)
    {
        entryIcon.sprite = image;
    }

    private void ChangeUiText(string weaponName)
    {
        entryWeaponName.text = weaponName;
    }

    private void ChangeStackSizeText()
    {
        entryWeaponStackSize.text = savedWeapon.stackSize.ToString();
    }
}
