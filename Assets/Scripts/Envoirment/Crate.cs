using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Crate : NetworkBehaviour, ICollectable
{

    [SerializeField]
    private WeaponDataSO weaponData;

    [SerializeField]
    private Sprite crateIcon;
    [SerializeField]
    private Transform iconTransform;
    [SerializeField]
    private SpriteRenderer iconHolder;


    private void Start()
    {
        crateIcon = weaponData.icon;
        iconHolder.sprite = crateIcon;
    }

    public void Collect(ulong ownerId)
    {
        OnDestroyCrateServerRpc();
        EventManager.OnWeaponCollected?.Invoke(weaponData, ownerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDestroyCrateServerRpc()
    {
        NetworkObject.Despawn();
        OnDestroyCrateClientRpc();
    }

    [ClientRpc]
    private void OnDestroyCrateClientRpc()
    {
        Destroy(this.gameObject);
    }
}
