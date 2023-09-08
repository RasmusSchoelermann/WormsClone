using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WormWeaponHandling : NetworkBehaviour
{
    private Worm wormBase;
    private WormController controller;

    private Vector3 difference;

    [SerializeField]
    private GameObject currentGunGameObject;
    [SerializeField]
    private WeaponDataSO currentGunSO;


    public SpriteRenderer currentGunRenderer;


    public void InitializeWeaponHandling(Worm worm_Base, WormController control)
    {
        wormBase = worm_Base;
        controller = control;
    }

    public void UpdateWeaponHandling()
    {
        if (!IsOwner)
            return;

        if (controller.ReturnCurrentInventoryItemWeapon().weaponData == null)
            return;

        Vector2 pos = controller.ReturnCurrentInventoryItemWeapon().weaponData.gunPosition;

        CheckWeaponState();
    }

    private void WeaponShooting()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MultiplayerGameHandler.Instance.FireCurrentWeapon(currentGunSO, currentGunGameObject);
        }
    }
    public void SetCurrentWormWeapon(WeaponDataSO weapon)
    {
        Inventory currentInventory = wormBase.GetPlayerInventory();
        InventoryItem selectedItem = currentInventory.GetInventoryItem(weapon);
        
        if (selectedItem == null)
            return;

        if (!wormBase.IsCurrentSelectedWorm())
            return;

        controller.SetCurrentInventoryItem(selectedItem);

        if (currentGunGameObject != null)
            return;

        if (!controller.isSpawning)
        {
            controller.isSpawning = true;
            MultiplayerGameHandler.Instance.SpawnCurrentWeapon(weapon, controller, wormBase.playerRef.ownerId);
        }
    }

    public void EquippedGunRoation()
    {
        difference = controller.mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();

        float rot_Z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        SetCurrentRotation(rot_Z);
    }

    private void SetCurrentRotation(float rotation)
    {
        if (currentGunGameObject == null)
            return;

        currentGunGameObject.transform.rotation = Quaternion.Euler(0, 0, rotation + 180f);
        NetworkObject gunNetworkObject = currentGunGameObject.GetComponent<NetworkObject>();
        bool flipState = CheckIfWeaponNeedsToFlip();
        CheckForWeaponFlipServerRpc(flipState, gunNetworkObject);
    }

    public GameObject returnCurrentGun()
    {
        return currentGunGameObject;
    }

    private bool WeaponActiveState()
    {
        if (controller.isJumping)
        {
            return false;
        }
        else if (controller.isMoving)
        {
            return false;
        }
        return true;
    }

    private void CheckWeaponState()
    {
        if(WeaponActiveState())
        {
            if (currentGunGameObject == null)
            {
                if (!controller.isSpawning)
                {
                    controller.isSpawning = true;
                    MultiplayerGameHandler.Instance.SpawnCurrentWeapon(currentGunSO, controller, wormBase.playerRef.ownerId);
                }
            }
            else
            {
                EquippedGunRoation();
                WeaponShooting();
            }
        }
        else
        {
            if (currentGunGameObject != null)
            {
                MultiplayerGameHandler.Instance.DespawnCurrentWeapon(currentGunGameObject, controller);
                currentGunGameObject = null;
            }
        }
    }

    public bool CheckIfWeaponNeedsToFlip()
    {
        if (currentGunGameObject.transform.localRotation.z >= 0.7f)
        {
            return true;
        } 
        else
        {
            return false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckForWeaponFlipServerRpc(bool flipState, NetworkObjectReference gunRef)
    {
        FlipWeaponClientRPC(gunRef, flipState);
    }

    [ClientRpc]
    public void FlipWeaponClientRPC(NetworkObjectReference reference, bool flipState)
    {
        if(reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.gameObject.GetComponent<SpriteRenderer>().flipY = flipState;
            controller.FlipWorm(flipState);
        }
    }

    

    public void SetCurrentGun(GameObject curGun)
    {
        currentGunGameObject = curGun;
    }

    public void SetCurrentGunSO(WeaponDataSO curGunSO)
    {
        currentGunSO = curGunSO;
    }

}
