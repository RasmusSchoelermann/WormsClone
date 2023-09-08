using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class WormController : NetworkBehaviour, IWeaponSOObjectParent
{
    [SerializeField]
    private Worm wormBase;

    private WormMovement movementController;
    private WormWeaponHandling weaponController;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private InventoryItem currentWeapon;

    public PhysicsMaterial2D noFrictionMaterial;
    public PhysicsMaterial2D fullFrictionMaterial;

    public Transform groundCheck;
    public LayerMask groundLayer;

    public Camera mainCam;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private float _xInput { get; set; }
    public float xInput
    {
        get => _xInput;
        set => _xInput = value;
    }

    public int facingDirection = 1;

    public bool isGrounded;
    public bool isOnSlope;

    public bool isMoving;
    public bool isJumping;

    public bool canWalkOnSlope;
    public bool canJump;

    public int latestGunIndex;
    public bool isSpawning;

    //private SpriteRenderer spriteRenderer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }


    void Update()
    {
        if (!IsOwner) return;

        MovementInput();
        weaponController.UpdateWeaponHandling();
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;
        movementController.MovementApplication();
    }

    private void Initialize()
    {
        movementController = GetComponent<WormMovement>();
        weaponController = GetComponent<WormWeaponHandling>();

        movementController.InitializeMovement(wormBase, this);
        weaponController.InitializeWeaponHandling(wormBase, this);

        mainCam = Camera.main;

        if(IsOwner)
            WeaponEntry.OnEquipSelectedWeapon += weaponController.SetCurrentWormWeapon;
    }

    private void MovementInput()
    {
        if (!wormBase.IsCurrentSelectedWorm())
            return;

        _xInput = Input.GetAxisRaw("Horizontal");
        FlipWormServerRpc(_xInput);

        if(Input.GetButtonDown("Jump"))
        {
            movementController.Jump();
        }
    }

    public InventoryItem ReturnCurrentInventoryItemWeapon()
    {
        return currentWeapon;
    }

    public void SetCurrentInventoryItem(InventoryItem item)
    {
        currentWeapon = item;
    }

    [ServerRpc]
    public void FlipWormServerRpc(float xInput)
    {
        facingDirection *= -1;
        if (xInput == 1 && facingDirection == 1)
        {
            FlipWormClientRPC(true);
        }
        else if (xInput == -1 && facingDirection == -1)
        {
            FlipWormClientRPC(false);
        }
    }

    [ClientRpc]
    public void FlipWormClientRPC(bool flipState)
    {
        FlipWorm(flipState);
    }

    public void FlipWorm(bool flipState)
    {
        spriteRenderer.flipX = flipState;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public Transform GetWeaponObjectFollowTransform()
    {
        return transform;
    }
}
