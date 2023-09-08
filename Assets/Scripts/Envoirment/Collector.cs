using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public bool isCollecting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.GetComponent<WormController>().IsOwner)
            return;

        ICollectable collectible = collision.GetComponent<ICollectable>();
        if (collectible == null)
            return;

        if (isCollecting)
            return;

        collectible.Collect(gameObject.GetComponent<Worm>().playerRef.ownerId);
        isCollecting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!gameObject.GetComponent<WormController>().IsOwner)
            return;

        if (collision.GetComponent<ICollectable>() == null)
            return;

        isCollecting = false;
        gameObject.GetComponent<Worm_Base>().playerRef.eventTrigger = false;
    }
}
