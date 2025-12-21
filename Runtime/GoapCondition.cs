using System;
using GOAP.Runtime.Internal;
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
            if (GoapDataType != other.GoapDataType || Key != other.Key || ConditionDirection != other.ConditionDirection)
                return false;

            switch (GoapDataType)
            {
                case GoapDataType.Bool:
                case GoapDataType.Enum:
                    return Equals(Value, other.Value);
                case GoapDataType.Int:
                    
                    switch (ConditionDirection)
                    {
                        case ConditionDirection.LessThanEq:
                            return (int)Value >= (int)other.Value;
                        case ConditionDirection.GreaterThanEq:
                            return (int)Value <= (int)other.Value;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case GoapDataType.Float:
                    
                    switch (ConditionDirection)
                    {
                        case ConditionDirection.LessThanEq:
                            return (float)Value >= (float)other.Value;
                        case ConditionDirection.GreaterThanEq:
                            return (float)Value <= (float)other.Value;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)GoapDataType, Key, Value, (int)ConditionDirection);
        }
        
        public int GetHashCode(IWorldState worldState)
        {
            if (GoapResolver.ConditionIsSatisfied(this, worldState))
            {
                return HashCode.Combine((int)GoapDataType, Key, 
                    (int)ConditionDirection, true);
            }

            return GetHashCode();
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