using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/WeaponData")]
public class WeaponDataSO : ScriptableObject, INetworkSerializable
{
    public string weaponName;

    public BulletDataSO bulletSO;

    public GameObject gunPrefab;
    public Vector2 gunPosition;

    public int damage;
    public float projectileSpeed;

    public float explosionRadius;
    
    public Sprite icon;
    public GameObject crateIconGameObject;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref weaponName);
    }
}
