using UnityEngine;

namespace AI.States
{
    public class DelayState : AIState
    {
        private float timer = 0.0f;
        private float delay = 0.0f;
        
        public void Initialize(AIController controller, float delay, AIState nextState)
        {
            this.controller = controller;
            this.falseNextState = nextState;
            this.delay = delay;    
        }

        public override void UpdateState()
        {
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                timer = 0.0f;
                ExitState(true);
            }
        }
    }
}