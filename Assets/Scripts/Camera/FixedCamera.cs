using UnityEngine;
using UnityEngine.InputSystem;

public class FixedCamera : MonoBehaviour
{
    [Header("Movement")]
    public float panSpeed = 30f;

    [Header("Rotation")]
    public float horizontalRotationSpeed = 175f;
    public float verticalRotationSpeed = 100f;
    public float minPitchAngle = 30f;
    public float maxPitchAngle = 85f;

    [Header("Zoom")]
    public float zoomSpeed = 1.35f;
    public float minZoomHeight = 5f;
    public float maxZoomHeight = 100f;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _interactAction;

    private Vector3 _focusPoint;
    private float _currentPitch;

    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _interactAction = InputSystem.actions.FindAction("CameraInteract");

        _currentPitch = transform.eulerAngles.x;
        
        // get focus point for raycast
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = new Ray(transform.position, transform.forward);
        if (groundPlane.Raycast(ray, out var distanceToGround)) 
            _focusPoint = ray.GetPoint(distanceToGround);
    }

    void Update()
    {
        MoveCamera(); // camera movement pe planul XZ
        RotateCamera(); // roteste camera pe planul Y al punctului (orizontal) si pe planul X al sau (vertical)
        ZoomCamera(); // zoom forward (X) / backward (Z)
    }

    void MoveCamera()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            
            float scaledSpeed = Vector3.Distance(transform.position, _focusPoint) * panSpeed;
            Vector3 rotatedDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * moveDirection;
            Vector3 scaledDirection = scaledSpeed * Time.deltaTime * rotatedDirection;    
            
            transform.Translate(scaledDirection, Space.World);
            _focusPoint += scaledDirection;
        }
    }

    void RotateCamera()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();

        // rotatie stanga / dreapta (y)
        if (lookInput.x != 0.0f)
            transform.RotateAround(_focusPoint, Vector3.up, -lookInput.x * horizontalRotationSpeed * Time.deltaTime);

        // rotatie sus / jos (x)
        if (lookInput.y != 0.0f)
        {
            float clampedPitch = Mathf.Clamp(_currentPitch + lookInput.y * verticalRotationSpeed * Time.deltaTime, minPitchAngle, maxPitchAngle);
            transform.RotateAround(_focusPoint, transform.right, clampedPitch - _currentPitch);
            
            _currentPitch = clampedPitch;
        }
    }

    void ZoomCamera()
    {
        float zoomInput = _interactAction.ReadValue<float>();

        if (zoomInput != 0.0f)
        {
            float scaledSpeed = zoomSpeed * Vector3.Distance(transform.position, _focusPoint);
            Vector3 moveDirection = zoomInput * scaledSpeed * Time.deltaTime * transform.forward;
            Vector3 targetPosition = transform.position + moveDirection;
            
            if (targetPosition.y >= minZoomHeight && targetPosition.y <= maxZoomHeight)
                transform.position = targetPosition;
        }
    }
}