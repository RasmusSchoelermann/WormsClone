using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMovement : MonoBehaviour
{
    private Worm wormBase;
    private WormController controller;
    private CapsuleCollider2D col;
    private Rigidbody2D rb;

    [SerializeField]
    private float _groundCheckRadius; //0.06

    [SerializeField]
    private float _slopeCheckDistance; //0.07

    [SerializeField]
    private float _maxSlopeAngle; //60

    private Vector2 _newVelocity;
    private Vector2 _newForce;
    private Vector2 _capsuleColliderSize;

    private Vector2 _slopeNormalPerp;

    private float _slopeDownAngle;
    private float _slopeSideAngle;
    private float _lastSlopeAngle;

    public void InitializeMovement(Worm worm_Base, WormController control)
    {
        wormBase = worm_Base;
        controller = control;
        col = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        _capsuleColliderSize = col.size;

        wormBase.movementSpeed = 2;
        wormBase.jumpForce = 5;

    }

    public void MovementApplication()
    {
        CheckGround();
        SlopeCheck();
        ApplyMovement();
    }

    public void ApplyMovement()
    {
        if (!wormBase.IsCurrentSelectedWorm())
            return;

        if (controller.isGrounded && !controller.isOnSlope && !controller.isJumping)
        {
            _newVelocity.Set(wormBase.movementSpeed * controller.xInput, 0.0f);
            rb.velocity = _newVelocity;
        }
        else if (controller.isGrounded && controller.isOnSlope && controller.canWalkOnSlope && !controller.isJumping)
        {
            _newVelocity.Set(wormBase.movementSpeed * _slopeNormalPerp.x * -controller.xInput, wormBase.movementSpeed * _slopeNormalPerp.y * -controller.xInput);
            rb.velocity = _newVelocity;
        }
        else if (!controller.isGrounded && controller.isJumping)
        {
            _newVelocity.Set(wormBase.movementSpeed * controller.xInput, rb.velocity.y);
            rb.velocity = _newVelocity;
        }
        else
        {
            _newVelocity.Set(rb.velocity.x, rb.velocity.y);
            rb.velocity = _newVelocity;
        }

        if(Mathf.Approximately(rb.velocity.x, 0) && Mathf.Approximately(rb.velocity.y, 0))
        {
            WormMoving(false);
        }
        else
        {
            WormMoving(true);
        }

    }

    public void Jump()
    {
        if (controller.canJump)
        {
            controller.canJump = false;

            controller.isMoving = false;
            controller.isJumping = true;

            _newVelocity.Set(0.0f, 0.0f);
            rb.velocity = _newVelocity;
            _newForce.Set(0.0f, wormBase.jumpForce);

            rb.AddForce(_newForce, ForceMode2D.Impulse);
        }
    }

    private void WormMoving(bool moveing)
    {
        controller.isMoving = moveing;
    }

    //SLOPE MOVEMENT~~~~~~~~~~~~~~~~~~~
    private void CheckGround()
    {
        controller.isGrounded = Physics2D.OverlapCircle(controller.groundCheck.position, _groundCheckRadius, controller.groundLayer);

        if (rb.velocity.y <= 0.0f && controller.isGrounded)
        {
            controller.isJumping = false;
        }

        if (controller.isGrounded && !controller.isJumping && _slopeDownAngle <= _maxSlopeAngle)
        {
            controller.canJump = true;
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, _capsuleColliderSize.y / 2));

        HorizontalSlopeCheck(checkPos);
        VerticalSlopeCheck(checkPos);
    }

    private void HorizontalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, _slopeCheckDistance, controller.groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, _slopeCheckDistance, controller.groundLayer);

        if (slopeHitFront)
        {
            controller.isOnSlope = true;

            _slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            controller.isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);

        }
        else
        {
            _slopeSideAngle = 0.0f;
            controller.isOnSlope = false;
        }
    }

    private void VerticalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, _slopeCheckDistance, controller.groundLayer);

        if (hit)
        {

            _slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (_slopeDownAngle != _lastSlopeAngle)
            {
                controller.isOnSlope = true;
            }

            _lastSlopeAngle = _slopeDownAngle;

            Debug.DrawRay(hit.point, _slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (_slopeDownAngle != 0 && _slopeDownAngle < _maxSlopeAngle)
        {
            controller.isOnSlope = true;
        }
        else
        {
            controller.isOnSlope = false;
        }

        if (_slopeDownAngle > _maxSlopeAngle || _slopeSideAngle > _maxSlopeAngle)
        {
            controller.canWalkOnSlope = false;
        }
        else
        {
            controller.canWalkOnSlope = true;
        }

        if (controller.isOnSlope && controller.canWalkOnSlope && controller.xInput == 0.0f)
        {
            rb.sharedMaterial = controller.fullFrictionMaterial;
        }
        else
        {
            rb.sharedMaterial = controller.noFrictionMaterial;
        }
    }
}
