using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementModule : Module
{
    [SerializeField] private float speed;
    
    public float Speed
    {
        get => speed; 
        set => speed = value;
    }

    public event Action TargetReached;
    
    private bool startedMoving = false;
    private Vector3 destination;
    private float stopRange;
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    
    public bool MoveTo(Vector3 destination, float stopRange)
    {
        float distance = Vector3.Distance(transform.position, destination);
        if (distance <= stopRange)
        {
            TargetReached?.Invoke();
            return true; // Has reached destination
        }

        this.destination = destination;
        this.stopRange = stopRange;
        startedMoving = true;
        return false;   // Still has to move
    }

    void Update()
    {
        if (!startedMoving)
            return;
        
        float distance = Vector3.Distance(transform.position, destination);
        if (distance <= stopRange)
        {
            startedMoving = false;
            TargetReached?.Invoke();
            return;
        }
        
        // Start moving towards object at speed;
        Vector3 direction = speed * Time.deltaTime * (destination - transform.position).normalized;
        characterController.Move(direction);
    }
}
