using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerNetworkObject : NetworkBehaviour
{
    [SerializeField]
    private Inventory inventory;

    public WeaponDataSO testWeapon;

    public bool myTurn = false;

    public ulong ownerId;

    [SerializeField]
    private Canvas playerCanvas;

    [SerializeField]
    private List<Worm> _wormsAlive = new List<Worm>();

    private Worm _currentWorm { get; set; }
    public Worm currentWorm
    {
        get 
        { 
            if (_currentWorm == null)
                throw new ArgumentException("_current Worm is null!");
            return _currentWorm;
        }
        set
        {
            if(value == null)
                throw new ArgumentException("Value to set _currentWorm is null");
            _currentWorm = value;
        }
    }

    public NetworkVariable<ushort> wormsAliveNetwork = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public WeaponManager manager;

    [SerializeField]
    private WormManager _wormManager;

    public bool eventTrigger = false;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    private void Initialize()
    {
        manager = GetComponent<WeaponManager>();
        _wormManager.SetUpWormManager(_wormsAlive);
        ownerId = gameObject.GetComponent<NetworkObject>().OwnerClientId;

        if(IsOwner)
        {
            playerCanvas.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _wormManager.SwitchWorms();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestSpawnServerRPC(ownerId);
        }
    }

    public void SetupWormsOnGameStart()
    {
        FindClientWorms();
    }

    public void SetCurrentWormWeapon(WeaponDataSO weapon)
    {
        InventoryItem selectedItem = inventory.GetInventoryItem(weapon);
        if (selectedItem == null)
            return;

        if (currentWorm == null)
            return;

        currentWorm.gameObject.GetComponent<WormController>().SetCurrentInventoryItem(selectedItem);
    }

    private void FindClientWorms()
    {
        GameObject[] wormGameObjects = GameObject.FindGameObjectsWithTag("Worm");

        for (int i = 0; i < wormGameObjects.Length; i++)
        {
            if (wormGameObjects[i].GetComponent<NetworkObject>().IsOwner)
            {
                SetClientWormsInList(wormGameObjects[i]);
            }
        }
    }

    private void SetClientWormsInList(GameObject worm)
    {
        Worm currentWorm = worm.GetComponent<Worm>();
        _wormsAlive.Add(currentWorm);
        wormsAliveNetwork.Value ++;
    }

    public List<Worm> ReturnClientWormList()
    {
        return _wormsAlive;
    }

    public void ClearWormOffClientList(int wormId)
    {
        _wormsAlive.RemoveAt(wormId);
    }

    [ServerRpc]
    private void TestSpawnServerRPC(ulong ownerId)
    {
        GameObject currentObject = Instantiate(manager.testWorm);
        currentObject.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
        NetworkObject wormObject = currentObject.GetComponent<NetworkObject>();
        ulong networkID = wormObject.NetworkObjectId;

        TestSpawnClientRPC(wormObject, networkID);

    }

    [ClientRpc]
    private void TestSpawnClientRPC(NetworkObjectReference wormReference, ulong networkID) 
    {
        if (wormReference.TryGet(out NetworkObject networkObject))
        {
            addWormToPlayer(networkObject.gameObject);
        }
    }

    private void addWormToPlayer(GameObject worm)
    {
        if (worm == null)
            return;

        if (!worm.GetComponent<NetworkObject>().IsOwner)
            return;

        worm.GetComponent<Worm>().playerRef = this;
        _wormsAlive.Add(worm.GetComponent<Worm>());

        if (_currentWorm == null && _wormsAlive.Count == 1)
            _currentWorm = worm.GetComponent<Worm>();
    }

    public Inventory ReturnPlayerInventory()
    {
        return inventory;
    }
}
