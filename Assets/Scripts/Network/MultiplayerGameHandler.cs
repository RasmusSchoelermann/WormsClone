using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerGameHandler : NetworkBehaviour
{
    [SerializeField]
    private WeaponDataListSO weaponDataListSO;

    [SerializeField]
    private BulletDataSO bulletDataListSO;

    public static MultiplayerGameHandler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnCurrentWeapon(WeaponDataSO weaponDataSO, IWeaponSOObjectParent weaponDataSoObjectParent, ulong ownerId)
    {
        NetworkSpawnCurrentWeaponServerRpc(GetWeaponDataSoIndex(weaponDataSO), weaponDataSoObjectParent.GetNetworkObject(), ownerId);
    }

    public void DespawnCurrentWeapon(GameObject currentGun, IWeaponSOObjectParent weaponDataSoObjectParent)
    {
        NetworkDespawnCurrentWeaponServerRpc(currentGun.GetComponent<NetworkObject>(), weaponDataSoObjectParent.GetNetworkObject());
    }

    public void FireCurrentWeapon(WeaponDataSO weaponDataSO, GameObject currentGun)
    {
        NetworkShootCurrentWeaponServerRpc(GetWeaponDataSoIndex(weaponDataSO), currentGun.GetComponent<NetworkObject>());
    }


    [ServerRpc(RequireOwnership = false)]
    private void NetworkSpawnCurrentWeaponServerRpc(int weaponObjectSOIndex, NetworkObjectReference weaponObjectParentReference, ulong ownerId)
    {
        WeaponDataSO weaponDataSO = GetWeaponDataSoFromIndex(weaponObjectSOIndex);
        GameObject curGun = Instantiate(weaponDataSO.gunPrefab);
        NetworkObject curGunNetworkObj = curGun.GetComponent<NetworkObject>();

        curGunNetworkObj.SpawnWithOwnership(ownerId);

        IWeaponSOObjectParent weaponDataSOObjectParent = GetWeaponSoObjectParentFromNetworkObjectReference(weaponObjectParentReference);

        curGunNetworkObj.TrySetParent(weaponDataSOObjectParent.GetWeaponObjectFollowTransform(), false);
        NetworkSpawnCurrentWeaponClientRpc(curGun, weaponObjectParentReference, weaponObjectSOIndex);
    }

    [ClientRpc]
    private void NetworkSpawnCurrentWeaponClientRpc(NetworkObjectReference curGun, NetworkObjectReference wormReference, int weaponObjectSOIndex)
    {
        if (curGun.TryGet(out NetworkObject networkObject))
        {
            WeaponDataSO weaponDataSO = GetWeaponDataSoFromIndex(weaponObjectSOIndex);
            IWeaponSOObjectParent weaponDataSOObjectParent = GetWeaponSoObjectParentFromNetworkObjectReference(wormReference);
            Worm worm = weaponDataSOObjectParent.GetWeaponObjectFollowTransform().GetComponent<Worm>();

            GameObject currentGun = networkObject.gameObject;
            worm.GetComponent<WormController>().isSpawning = false;
            worm.GetComponent<WormWeaponHandling>().SetCurrentGun(currentGun);
            worm.GetComponent<WormWeaponHandling>().SetCurrentGunSO(weaponDataSO);
            worm.GetComponent<WormWeaponHandling>().currentGunRenderer = currentGun.GetComponent<SpriteRenderer>();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NetworkDespawnCurrentWeaponServerRpc(NetworkObjectReference gunReference, NetworkObjectReference wormReference)
    {
        if (gunReference.TryGet(out NetworkObject networkObject))
        {
            IWeaponSOObjectParent weaponDataSOObjectParent = GetWeaponSoObjectParentFromNetworkObjectReference(wormReference);
            WormController wormController = weaponDataSOObjectParent.GetWeaponObjectFollowTransform().GetComponent<WormController>();
            wormController.latestGunIndex = GetWeaponDataSoIndex(wormController.ReturnCurrentInventoryItemWeapon().weaponData);

            NetworkDespawnCurrentWeaponClientRpc(gunReference);
            networkObject.Despawn(true);
        }
    }

    [ClientRpc]
    private void NetworkDespawnCurrentWeaponClientRpc(NetworkObjectReference gunReference)
    {
        if (gunReference.TryGet(out NetworkObject networkObject))
        {
             Destroy(networkObject.gameObject);
        }
    }

    private IWeaponSOObjectParent GetWeaponSoObjectParentFromNetworkObjectReference(NetworkObjectReference reference)
    {
        reference.TryGet(out NetworkObject weaponObjectParentNetworkObject);
        return weaponObjectParentNetworkObject.GetComponent<IWeaponSOObjectParent>();
    }

    private int GetWeaponDataSoIndex(WeaponDataSO weaponDataSo)
    {
        return weaponDataListSO.weaponDataListSO.IndexOf(weaponDataSo);
    }

    private WeaponDataSO GetWeaponDataSoFromIndex(int weaponDataSOIndex)
    {
        return weaponDataListSO.weaponDataListSO[weaponDataSOIndex];
    }

    [ServerRpc]
    private void NetworkShootCurrentWeaponServerRpc(int weaponObjectSOIndex, NetworkObjectReference currentWeapon)
    {
        WeaponDataSO weaponDataSO = GetWeaponDataSoFromIndex(weaponObjectSOIndex);
        BulletDataSO bulletDataSO = weaponDataSO.bulletSO;
        if (currentWeapon.TryGet(out NetworkObject networkObject))
        {
            GameObject bullet = Instantiate(bulletDataSO.bulletPrefab, networkObject.transform.position - networkObject.transform.right, networkObject.transform.rotation);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.GetComponent<Bullet>().bulletData = bulletDataSO;

            Rigidbody2D bulletR2D = bullet.GetComponent<Rigidbody2D>();
            bulletR2D.AddForce(-networkObject.transform.right * bulletDataSO.projectileSpeed, ForceMode2D.Impulse);


            
            /*SpriteRenderer renderer = networkObject.GetComponent<SpriteRenderer>();
            if (renderer == null)
                return;

            if (renderer.flipY)
            {
            }*/
        }

    }


    //Ground

    public void UpdateGroundSprite()
    {
        NetworkUpdateGroundSpriteServerRpc();
    }

    [ServerRpc]
    private void NetworkUpdateGroundSpriteServerRpc()
    {

    }

    [ClientRpc]
    private void NetworkUpdateGroundSpriteClientRpc()
    {

    }
}
