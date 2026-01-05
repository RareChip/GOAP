using GOAP.Runtime.Core;

namespace GOAP.Runtime.API
{
    public interface IWorldState
    {
        public int GetInt(string key);
        public float GetFloat(string key);
        public bool GetBool(string key);
        public void ApplyEffect(GoapEffect effect);
        public bool ConditionIsSatisfied(GoapCondition condition);
        public IWorldState Clone();
    }
}