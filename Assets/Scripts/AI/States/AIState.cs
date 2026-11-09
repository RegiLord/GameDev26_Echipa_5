using UnityEngine;

namespace AI.States
{
    public class AIState
    {
        public AIController controller;

        public AIState(AIController controller)
        {
            this.controller = controller;
        }

        public virtual void EnterState()
        {
        }

        public virtual void ExitState()
        {
            controller.NextState();
        }

        public virtual void UpdateState()
        {
        }
    }
}