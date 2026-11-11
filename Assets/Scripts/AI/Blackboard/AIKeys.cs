using UnityEngine;

namespace AI.Blackboard
{
    public class AIKeys
    {
        public static readonly BlackboardKey<Vector3> Target = new();
        public static readonly BlackboardKey<GameObject> TargetObject = new();
        public static readonly BlackboardKey<MovementModule> MovementModule = new();
        public static readonly BlackboardKey<AttackModule> AttackModule = new();
    }
}