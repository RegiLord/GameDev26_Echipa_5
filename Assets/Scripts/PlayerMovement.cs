using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float cameraSensitivity = 100f;
    
    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction lookAction;
    private InputAction upAction;
    private InputAction downAction;

    private bool sprinting, rising, falling;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 deltaMouse = Vector2.zero;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get Input Actions
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        lookAction = InputSystem.actions.FindAction("Look");
        upAction = InputSystem.actions.FindAction("Up");
        downAction = InputSystem.actions.FindAction("Down");
        
        // Lock cursor to middle screen
        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    void ReadInput()
    {
        // MOVE
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        moveDirection.x = moveInput.x;
        moveDirection.z = moveInput.y;
        moveDirection.Normalize();
        
        // MOVE UP
        rising = upAction.IsPressed();
        // MOVE DOWN
        falling = downAction.IsPressed();
    
        // SPRINT
        sprinting = sprintAction.IsPressed();
        
        // LOOK
        deltaMouse = lookAction.ReadValue<Vector2>();
    }
    
    void MovePlayer()
    {
        Vector3 forceVector = 
            moveDirection.x * transform.right +         // Moving right or left
            moveDirection.z * transform.forward +       // Moving forward or back
            (rising ? Vector3.up : Vector3.zero) +    // Moving up
            (falling ? Vector3.down : Vector3.zero);   // Moving down
        
        forceVector = speed * (sprinting ? 2 : 1) * forceVector.normalized;

        rb.AddForce(forceVector);
        
        // Stopping unnecessary movement
        if (rb.linearVelocity.magnitude < 0.05f)
            rb.linearVelocity = Vector3.zero;
    }

    void RotateCamera()
    {
        Vector3 rotateVector = Vector3.zero;
        rotateVector.x = -deltaMouse.y * cameraSensitivity * Time.deltaTime;
        rotateVector.y = deltaMouse.x * cameraSensitivity * Time.deltaTime;
        rotateVector.z = 0;
        
        Vector3 eulerAngles = transform.rotation.eulerAngles + rotateVector;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

}
