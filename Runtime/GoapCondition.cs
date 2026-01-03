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
            this.GoapDataType = goapDataType;
            this.Key = key;
            this.Value = value;
            this.ConditionDirection = conditionDirection;
        }

        public bool Equals(GoapCondition other)
        {
            if (this.GoapDataType != other.GoapDataType || this.Key != other.Key || this.ConditionDirection != other.ConditionDirection)
                return false;

            switch (this.GoapDataType)
            {
                case GoapDataType.Bool:
                case GoapDataType.Enum:
                    return Equals(this.Value, other.Value);
                case GoapDataType.Int:
                    
                    switch (this.ConditionDirection)
                    {
                        case ConditionDirection.LessThanEq:
                            return (int)this.Value >= (int)other.Value;
                        case ConditionDirection.GreaterThanEq:
                            return (int)this.Value <= (int)other.Value;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case GoapDataType.Float:
                    
                    switch (this.ConditionDirection)
                    {
                        case ConditionDirection.LessThanEq:
                            return (float)this.Value >= (float)other.Value;
                        case ConditionDirection.GreaterThanEq:
                            return (float)this.Value <= (float)other.Value;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)this.GoapDataType, this.Key, this.Value, (int)this.ConditionDirection);
        }
        
        public int GetHashCode(IWorldState worldState)
        {
            if (worldState.ConditionIsSatisfied(this))
            {
                return HashCode.Combine((int)this.GoapDataType, this.Key, 
                    (int)this.ConditionDirection, true);
            }

            return this.GetHashCode();
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