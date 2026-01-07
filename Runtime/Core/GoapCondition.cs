using System;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Core
{
    public class GoapCondition
    {
        public GoapDataType GoapDataType { get; }
        public string Key { get; }
        public object Value { get; }
        public ConditionDirection ConditionDirection { get; }

        public GoapCondition(GoapDataType goapDataType, string key, object value, ConditionDirection conditionDirection)
        {
            this.GoapDataType = goapDataType;
            this.Key = key;
            this.Value = value;
            this.ConditionDirection = conditionDirection;
        }

        public override bool Equals(object obj)
        {
            if (obj is not GoapCondition other)
                return false;

            return GoapDataType == other.GoapDataType
                   && Key == other.Key && Value.Equals(other.Value) && ConditionDirection == other.ConditionDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GoapDataType, Key, Value, ConditionDirection);
        }
    }
    
    public enum ConditionDirection
    {
        Equals,
        NotEquals,
        LessThanEq,
        GreaterThanEq,
    }
}