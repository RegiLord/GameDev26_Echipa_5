using System.Collections.Generic;
using AI.Blackboard;
using AI.States;
using UnityEngine;

public enum TeamID
{
    Player = 0,
    Enemy1 = 1,
    Enemy2 = 2,
    Other = 3
}

public class AIController : MonoBehaviour
{
    private bool startedControlling = false;
    
    public float delayToStartControlling = 5.0f;
    public float detectionRange = 10.0f;
    public TeamID teamID = 0;
    
    private AIState currentState;
    
    // Blackboard containing data
    public Blackboard blackboard = new();
    
    void Start()
    { 
        // Dont start immediately, apply a delay
        Invoke(nameof(startControlling), delayToStartControlling);
    }

    private void startControlling()
    {
        
        blackboard.Set(AIKeys.Target, Vector3.zero);
        blackboard.Set(AIKeys.TargetObject, null);
        blackboard.Set(AIKeys.MovementModule, GetComponent<MovementModule>());
        blackboard.Set(AIKeys.AttackModule, GetComponent<AttackModule>());
        
        FindClosestEnemyInDetectionRange findEnemyState = new FindClosestEnemyInDetectionRange();
        MoveToTargetState moveToTargetState = new MoveToTargetState();
        AttackTargetState attackTargetState = new AttackTargetState();
        
        DelayState delayState = new DelayState();
        
        delayState.Initialize(this, 1.0f, findEnemyState);
        
        findEnemyState.Initialize(this, moveToTargetState, delayState);
        moveToTargetState.Initialize(this, attackTargetState, findEnemyState);
        attackTargetState.Initialize(this, findEnemyState, null);
        
        currentState = findEnemyState;
        
        startedControlling = true;
        currentState.EnterState();
    }

    void Update()
    {
        if (!startedControlling)
            return;
        
        if (currentState is not null)
            currentState.UpdateState();
    }

    public void ChangeState(AIState newState)
    {
        if (currentState == null)
        {
            Debug.LogError("AI Controller: No AI state found when trying to move to next state!");
            return;
        }

        if (newState == null)
        {
            currentState = null;
            return;
        }
        
        currentState = newState;
        currentState.EnterState();
    }
    
}
