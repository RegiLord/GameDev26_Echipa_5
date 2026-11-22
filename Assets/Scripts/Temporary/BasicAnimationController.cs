using System;
using UnityEngine;

public class BasicAnimationController : MonoBehaviour
{
    private AttackModule attackModule;
    private MovementModule movementModule;
    
    private Vector3 destination;
    
    [SerializeField] private Animator animator;

    void Awake()
    {
        attackModule = GetComponent<AttackModule>();
        movementModule = GetComponent<MovementModule>();
    }

    private void OnEnable()
    {
        attackModule.OnAttack += useAttackAnimation;
    }

    private void OnDisable()
    {
        attackModule.OnAttack -= useAttackAnimation;
    }

    void useAttackAnimation(float _)
    {
        animator.SetTrigger("Attack");
    }
    
    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isRunning", movementModule.IsMoving);

        if (movementModule.IsMoving)
        {
            destination = movementModule.Destination;
            
            destination.y = transform.position.y;
            transform.LookAt(destination);
        }
    }
}
