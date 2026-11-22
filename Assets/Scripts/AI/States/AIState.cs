using UnityEngine;

namespace AI.States
{
    public class AIState
    {
        protected AIController controller;
        protected AIState trueNextState = null, falseNextState = null;

        public AIState() {}

        public virtual void Initialze(AIController controller)
        {
            this.controller = controller;
        }
        public virtual void Initialize(AIController controller, AIState trueNextState, AIState falseNextState)
        {
            this.controller = controller;
            this.trueNextState = trueNextState;
            this.falseNextState = falseNextState;
        }

        public virtual void EnterState()
        {
        }

        public virtual void ExitState(bool exitCode)
        {
            if (exitCode)
                controller.ChangeState(trueNextState);
            else 
                controller.ChangeState(falseNextState);
        }

        public virtual void UpdateState()
        {
        }
    }
}