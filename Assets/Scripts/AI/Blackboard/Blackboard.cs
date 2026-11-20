using System.Collections.Generic;

namespace AI.Blackboard
{
    public class Blackboard
    {
        private readonly Dictionary<BlackboardKey, object> memory = new();

        public void Set<T>(BlackboardKey<T> key, T value)
        {
            memory[key] = value;
        }

        public bool TryGet<T>(BlackboardKey<T> key, out T value)
        {
            if (memory.TryGetValue(key, out object data))
            {
                if (data is null)
                {
                    value = default;
                    return true;
                }
                
                if (data is T t)
                {
                    value = t;
                    return true;
                }
            }
            
            value = default;
            return false;
        }

        public void Remove<T>(BlackboardKey<T> key)
        {
            memory.Remove(key);
        }
    }
}