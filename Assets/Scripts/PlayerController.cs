using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 6;
    private float gravity = 9.8f;
    private float jumpAcceleration = 10f;
    private float verticalSpeed = 0;

    public Transform cameraHolder;
    public float mouseSensitivity = 2f;
    public float upLimit = -50f;
    public float downLimit = 50f;

    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        KeyControl spaceKey = Keyboard.current.spaceKey;

        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove   = Input.GetAxis("Vertical");

        if (characterController.isGrounded) {
            verticalSpeed = 0;

            if (spaceKey.wasPressedThisFrame || spaceKey.isPressed) {
                verticalSpeed += jumpAcceleration;
            }
        } else {
            verticalSpeed -= gravity * (Time.deltaTime);
        }

        Vector3 gravityMove = new Vector3(0, verticalSpeed, 0);
        Vector3 move = transform.forward * verticalMove + transform.right * horizontalMove;
        characterController.Move(speed * Time.deltaTime * move + gravityMove * Time.deltaTime);
    }

    private void Rotate()
    {
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float verticalRotation   = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        Vector3 currentRotation = cameraHolder.localEulerAngles;
        
        if (currentRotation.x > 180) {
            currentRotation.x -= 360;
        }

        currentRotation.y = Mathf.Clamp(currentRotation.y, upLimit, downLimit);

        transform.Rotate(0, horizontalRotation, 0);
        cameraHolder.Rotate(-verticalRotation, 0 , 0);
    }
}
