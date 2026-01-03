using System.Collections.Generic;

namespace GOAP.Runtime
{
    public interface IWorldState
    {
        public T Get<T>(string key);
        public void ApplyEffect(GoapEffect effect);
        public bool ConditionIsSatisfied(GoapCondition condition);
        public IWorldState Clone();
        public bool Equals(object other);
        public int GetHashCode();
    }
}