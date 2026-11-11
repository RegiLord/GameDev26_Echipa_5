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
    
    
    private AIState startState;
    private AIState currentState;
    private List<AIState> states = new List<AIState>();
    
    public AIState StartState => startState;
    
    // Blackboard containing data
    public Blackboard blackboard = new();
    
    void Start()
    { 
        // Dont start immediately, apply a delay
        Debug.Log("AI Controller: Starting AI Controller!");
        Invoke(nameof(startControlling), delayToStartControlling);
    }

    private void startControlling()
    {
        blackboard.Set(AIKeys.Target, null);
        blackboard.Set(AIKeys.MovementModule, GetComponent<MovementModule>());
        blackboard.Set(AIKeys.AttackModule, GetComponent<AttackModule>());
        
        AIState findEnemyState = new FindEnemyState(this, null);
        AIState moveToTargetState = new MoveToTargetState(this, null);
        AIState attackTargetState = new AttackTargetState(this, null);
        
        findEnemyState.NextState = moveToTargetState;
        moveToTargetState.NextState = attackTargetState;
        attackTargetState.NextState = findEnemyState;

        startState = findEnemyState;
        currentState = findEnemyState;
        
        startedControlling = true;
        currentState.EnterState();
    }

    void Update()
    {
        if (!startedControlling)
            return;
        
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
            Debug.LogError("AI Controller: No AI state found when trying to move to next state!");
            return;
        }
        
        if (gameObject.name == "Box")
            Debug.Log("AI Controller: Changing state to " + newState.GetType().Name);
        
        currentState = newState;
        currentState.EnterState();
    }
    
}
