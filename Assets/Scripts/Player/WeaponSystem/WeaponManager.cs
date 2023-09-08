using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private WeaponDataSO _equippedWeapon;

    [SerializeField]
    private AudioClip _pickupSound;

    public GameObject testWorm;

    public void EquipWeapon(WeaponDataSO newWeapon)
    {
        _equippedWeapon = newWeapon;
    }
}
