using System.Collections.Generic;
using AI.Blackboard;
using AI.States;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private bool startedControlling = false;
    
    public float delay = 5.0f;
    
    private List<AIState> states;
    private Dictionary<AIState, AIState> stateTransitions;
    private AIState currentState;
    
    // Blackboard containing data
    public Blackboard blackboard = new();
    
    void Start()
    { 
        // Dont start immediately, apply a delay
        Invoke(nameof(startControlling), delay);
    }

    private void startControlling()
    {
        blackboard.Set(AIKeys.Target, null);
        blackboard.Set(AIKeys.MovementModule, GetComponent<MovementModule>());
        blackboard.Set(AIKeys.AttackModule, GetComponent<AttackModule>());
        
        states = new List<AIState>();
        states.Add(new FindEnemyState(this));
        states.Add(new MoveToTargetState(this));
        states.Add(new AttackTargetState(this));
        
        currentState = states[0];
        
        stateTransitions = new Dictionary<AIState, AIState>();
        
        stateTransitions[states[0]] = states[1];
        stateTransitions[states[1]] = states[2];
        stateTransitions[states[2]] = states[0];
        
        startedControlling = true;
        currentState.EnterState();
    }

    void Update()
    {
        if (!startedControlling)
            return;
        
        currentState.UpdateState();
    }

    public void NextState()
    {
        if (currentState == null)
        {
            Debug.LogError("AI Controller: No AI state found when trying to move to next state!");
            return;
        }
        
        var nextState = stateTransitions[currentState];

        if (nextState == null)
        {
            Debug.LogError("AI Controller: No AI state found when trying to move to next state!");
            return;
        }
            
        currentState = nextState;
        currentState.EnterState();
    }
    
}
