using UnityEngine;

namespace AI.Blackboard
{
    public class AIKeys
    {
        public static readonly BlackboardKey<Transform> Target = new();
        public static readonly BlackboardKey<MovementModule> MovementModule = new();
        public static readonly BlackboardKey<AttackModule> AttackModule = new();
    }
}