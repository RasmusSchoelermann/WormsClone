using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/BulletData")]
public class BulletDataSO : ScriptableObject
{
    public GameObject bulletPrefab;

    public int damage;
    public float projectileSpeed;
    public float explosionRadius;

    public Sprite icon;
}

