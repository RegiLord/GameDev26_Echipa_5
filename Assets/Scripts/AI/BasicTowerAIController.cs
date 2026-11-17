
using AI.Blackboard;
using AI.States;
using Mono.Cecil.Cil;
using UnityEngine;

public class BasicTowerAIController : AIController
{
    protected override void startControlling()
    {
        blackboard.Set(AIKeys.Target, Vector3.zero);
        blackboard.Set(AIKeys.TargetObject, null);
        blackboard.Set(AIKeys.AttackModule, GetComponent<AttackModule>());

        FindClosestEnemyInDetectionRange findState = new FindClosestEnemyInDetectionRange();
        AttackTargetState attackState = new AttackTargetState();
        DelayState failedFind = new DelayState();
        DelayState delayState = new DelayState();

        var attackModule = GetComponent<AttackModule>();
        
        findState.Initialize(this, attackState, failedFind);
        failedFind.Initialize(this, 1.0f, findState);
        attackState.Initialize(this, delayState, failedFind);
        delayState.Initialize(this, Mathf.Max(1 / attackModule.AttackSpeed, 1.0f), findState);
        
        currentState = findState;
        
        startedControlling = true;
        currentState.EnterState();
    }

    public override void ChangeState(AIState newState)
    {
        if (newState == null)
        {
            Debug.LogError("WAHAT");
        }
        Debug.Log("Changing state to " + newState);
        base.ChangeState(newState);
        

    }
}
