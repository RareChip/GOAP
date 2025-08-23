using MyBox;
using UnityEngine;

namespace RareChip.Goap
{
    [System.Serializable]
    public class GoapCondition
    {
        [SerializeField] private bool staticConditionKey = true;

        [SerializeField, ConditionalField("staticConditionKey", true)]
        private string conditionKey;

        [SerializeField, ConditionalField("staticConditionKey")]
        private StaticConditionKey staticKey;

        public ConditionType conditionType;

        [ConditionalField(nameof(conditionType), inverse: true, ConditionType.True, ConditionType.False)]
        public float value;

        public string ConditionKey => staticConditionKey ? staticKey.Key : conditionKey;

        public ConditionStateType stateType;

        public GoapCondition(string conditionKey)
        {
            this.conditionKey = conditionKey;
            this.conditionType = ConditionType.True;
        }

        public GoapCondition(string conditionKey, bool value)
        {
            this.conditionKey = conditionKey;
            this.conditionType = value ? ConditionType.True : ConditionType.False;
        }

        public GoapCondition(string conditionKey, ConditionType conditionType, float value)
        {
            this.conditionKey = conditionKey;
            this.conditionType = conditionType;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is not GoapCondition)
                return false;

            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return conditionKey.GetHashCode() * conditionType.GetHashCode() ^
                   value.GetHashCode() * stateType.GetHashCode();
        }

        public override string ToString()
        {
            return $"{conditionKey}\n{conditionType}, {value}";
        }
    }

    public enum ConditionType
    {
        True,
        False,
        Equals,
        NotEquals,
        GreaterThan,
        LessThan
    }

    public enum ConditionStateType
    {
        Local,
        World
    }
}