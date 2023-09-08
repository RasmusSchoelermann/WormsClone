using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Data/WeaponDataList")]
public class WeaponDataListSO : ScriptableObject
{
    public List<WeaponDataSO> weaponDataListSO;

    public List<BulletDataSO> bulletDataListSO;
}
