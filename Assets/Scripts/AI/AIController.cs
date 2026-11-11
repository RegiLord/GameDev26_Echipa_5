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
        blackboard.Set(AIKeys.Target, null);
        blackboard.Set(AIKeys.MovementModule, GetComponent<MovementModule>());
        blackboard.Set(AIKeys.AttackModule, GetComponent<AttackModule>());
        
        AIState findEnemyState = new();
        AIState moveToTargetState = new();
        AIState attackTargetState = new();
        
        findEnemyState.Initialize(this, moveToTargetState, null);
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
        if (gameObject.name == "Capsule")
        {
            Debug.Log("AI Controller: Changing state to " + newState.GetType().Name);
        }
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
        
        if (gameObject.name == "Box")
            Debug.Log("AI Controller: Changing state to " + newState.GetType().Name);
        
        currentState = newState;
        currentState.EnterState();
    }
    
}
