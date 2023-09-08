using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Worm_Base : MonoBehaviour, IDamageable
{
    private int _wormHealth { get; set; }
    public int wormHealth 
    { 
        get => _wormHealth;
        set 
        {
            if (value > 100)
                throw new ArgumentException("Health cap exceeded");
            _wormHealth = value; 

        } 
    }

    private float _movementSpeed;
    public float movementSpeed
    {
        get => _movementSpeed;
        set 
        {
            if (value > 5)
                throw new ArgumentException("Worm exceeded Movementspeed Cap");
            _movementSpeed = value;
        }
    }

    private float _jumpForce;
    public float jumpForce
    {
        get => _jumpForce;
        set { _jumpForce = value; }
    }

    public PlayerNetworkObject playerRef;

    public void TakeDamage(int damage)
    {
        int healthAfterDamage = _wormHealth - damage;
        if(healthAfterDamage <= 0) 
        {
            DestroyEntity();
        }
        wormHealth = _wormHealth - damage;
    }

    private void DestroyEntity()
    {
        //TODO Death Animation
    }

    public bool IsCurrentSelectedWorm()
    {
        return playerRef.currentWorm == this;
    }

    public Inventory GetPlayerInventory()
    {
        return playerRef.ReturnPlayerInventory();
    }

}
