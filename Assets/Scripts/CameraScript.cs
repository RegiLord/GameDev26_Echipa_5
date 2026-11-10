using UnityEngine;
using UnityEngine.InputSystem;

public class FixedCamera : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 20f;
    public float minZoomHeight = 5f;
    public float maxZoomHeight = 40f;
    public float rotationSmoothness = 10f;

    private InputAction _moveAction;
    private InputAction _lookAction;
    
    private float _remainingAngle;
    private Vector3 _focusPoint;

    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();

        MoveCamera(); // camera movement pe planul XZ
        if (Mathf.Abs(_remainingAngle) < 1f) { CheckRotation(lookInput.x); } // verifica butoanele de rotatie (Q/E) daca e basically gata o rotatie
        RotateRemaining(); // rotatia care a mai ramas
        ZoomCamera(lookInput.y); // verifica butoanele de zomm (Z/X) ale camerei
    }

    void MoveCamera()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        
        Vector3 rotatedDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * moveDirection;
        transform.Translate(panSpeed * Time.deltaTime * rotatedDirection, Space.World);
    }

    void CheckRotation(float rotateInput)
    {
        if (_lookAction.WasPressedThisFrame())
        {
            _remainingAngle = 90f * ((rotateInput > 0.1f) ? 1f : (rotateInput < -0.1f) ? -1f : 0f);
            if (_remainingAngle != 0f)
            {
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = new Ray(transform.position, transform.forward);

                // if raycast hit
                if (groundPlane.Raycast(ray, out float distanceToGround)) { _focusPoint = ray.GetPoint(distanceToGround); }
            }
        }
    }

    void RotateRemaining()
    {
        if (Mathf.Abs(_remainingAngle) > 0.01f)
        {
            float step = _remainingAngle * rotationSmoothness * Time.deltaTime;
            transform.RotateAround(_focusPoint, Vector3.up, step);
            _remainingAngle -= step;
        }
        else { _remainingAngle = 0f; }
    }

    void ZoomCamera(float zoomInput)
    {
        if (Mathf.Abs(zoomInput) > 0.1f)
        {
            Vector3 moveDirection = zoomInput * zoomSpeed * Time.deltaTime * transform.forward;
            Vector3 targetPosition = transform.position + moveDirection;

            if (targetPosition.y >= minZoomHeight && targetPosition.y <= maxZoomHeight) { transform.position = targetPosition; }
        }
    }
}