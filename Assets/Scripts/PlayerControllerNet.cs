using Fusion.Sockets;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class PlayerControllerNet : NetworkBehaviour
{
    public float speed = 0.3f;
    public float jumpForce = 10f;
    private bool _isGrounded;
    private bool _canDoubleJump;
    private Rigidbody _rb;

    [Networked] private TickTimer delay { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            HandleMovement(data);
            HandleJump(data);
        }
    }

    private void HandleMovement(NetworkInputData data)
    {
        Vector3 moveInput = new Vector3(data.direction.x, 0, data.direction.z);
        Vector3 localForward = transform.forward;
        Vector3 localRight = transform.right;
        Vector3 movement = (localForward * moveInput.z) + (localRight * moveInput.x);

        if (movement.magnitude > 0)
        {
            movement.Normalize();
        }

        _rb.AddForce(movement * speed, ForceMode.VelocityChange);
    }

    private void HandleJump(NetworkInputData data)
    {
        if (data.buttons.IsSet(NetworkInputData.JUMP))
        {
            if (_isGrounded)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
                _canDoubleJump = true;
            }
            else if (_canDoubleJump)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
                _canDoubleJump = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }
}
