using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IWeaponSOObjectParent
{
    public Transform GetWeaponObjectFollowTransform();
    public NetworkObject GetNetworkObject();
}
