using System;
using GOAP.Runtime.Util;

namespace GOAP.Runtime
{
    public struct GoapCondition : IEquatable<GoapCondition>
    {
        public GoapDataType GoapDataType;
        public string Key;
        public object Value;
        public ConditionDirection ConditionDirection;

        public GoapCondition(GoapDataType goapDataType, string key, object value, ConditionDirection conditionDirection)
        {
            GoapDataType = goapDataType;
            Key = key;
            Value = value;
            ConditionDirection = conditionDirection;
        }

        public bool Equals(GoapCondition other)
        {
            return GoapDataType == other.GoapDataType && Key == other.Key && Equals(Value, other.Value) && ConditionDirection == other.ConditionDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)GoapDataType, Key, Value, (int)ConditionDirection);
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