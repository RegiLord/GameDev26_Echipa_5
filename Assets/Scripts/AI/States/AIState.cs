using UnityEngine;

namespace AI.States
{
    public class AIState
    {
        protected AIController controller;
        public AIState NextState;

        public AIState(AIController controller, AIState nextState)
        {
            this.controller = controller;
            this.NextState = nextState;
        }

        public virtual void EnterState()
        {
        }

        public virtual void ExitState()
        {
            controller.ChangeState(NextState);
        }

        public virtual void UpdateState()
        {
        }
    }
}