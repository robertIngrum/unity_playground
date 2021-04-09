using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharacter : MonoBehaviour
{
    public Transform cameraHolder;
    public float mouseSensitivity = 150f;
    public float maxRotation = 50f;
    public float minRotation = -50f;

    public float Speed = 5f;
    public float JumpHeight = 5f;
    public float GroundDistance = 10f;
    public float DashDistance = 5f;
    public LayerMask Ground;

    private Rigidbody rigidBody;
    private bool isGrounded = true;
    private bool canDoubleJump = true;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        isGrounded = Physics.CheckSphere(rigidBody.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        rotate();
        jump();
        dash();
        move();
    }

    private void rotate() {
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float verticalRotation   = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * -1;

        Vector3 currentRotation = cameraHolder.localEulerAngles;
        float projectedRotation = currentRotation.x + verticalRotation;

        if (projectedRotation > 180) {
            projectedRotation -= 360;
        }

        projectedRotation = Mathf.Clamp(projectedRotation, minRotation, maxRotation);

        // Stop spinning of rigidbody
        rigidBody.angularVelocity = Vector3.zero;
        transform.Rotate(0, horizontalRotation, 0);
        cameraHolder.Rotate(projectedRotation - currentRotation.x, 0, 0);
    }

    private void move() {
        float verticalMove   = Input.GetAxis("Vertical");
        float horizontalMove = Input.GetAxis("Horizontal");

        Vector3 moveVector  = transform.forward * verticalMove + transform.right * horizontalMove;
        Vector3 forceVector = moveVector * Speed * Time.fixedDeltaTime;

        if (Vector3.Distance(forceVector, Vector3.zero) == 0f) {
            // rigidBody.velocity = Vector3.Scale(rigidBody.velocity, new Vector3(0.1f, 1, 0.1f));
        } else {
            // rigidBody.MovePosition(rigidBody.position + moveVector * Speed * Time.fixedDeltaTime);
            rigidBody.AddForce(forceVector, ForceMode.Impulse);
            // rigidBody.velocity = forceVector + transform.up * rigidBody.velocity.y;
        }
    }

    private void jump() {
        if (isGrounded) {
            canDoubleJump = true;
        }

        if (Input.GetButtonDown("Jump") && canJump()) {
            if (!isGrounded) { canDoubleJump = false; }

            rigidBody.AddForce(
                Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y),
                ForceMode.Impulse
            );
        }
    }

    private void dash() {
        if (!Input.GetButtonDown("Dash")) { return; }

        // float rawDirection    = Mathf.Log(1f / (Time.deltaTime * rigidBody.drag + 1)) / -Time.deltaTime;
        // Vector3 dashDirection = new Vector3(rawDirection, 0, rawDirection);
        Vector3 dashVelocity  = transform.forward * DashDistance; // * dashDirection);
        
        rigidBody.AddForce(dashVelocity, ForceMode.Impulse);
    }

    private bool canJump() {
        return isGrounded || canDoubleJump;
    }
}