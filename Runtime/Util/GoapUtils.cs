using System;

namespace GOAP.Runtime.Util
{
    public static class GoapUtils
    {
        public static GoapDataType GetGoapDataType(object value)
        {
            GoapDataType dataType;

            switch (value)
            {
                case int:
                    dataType = GoapDataType.Int;
                    break;
                case float:
                    dataType = GoapDataType.Float;
                    break;
                case bool:
                    dataType = GoapDataType.Bool;
                    break;
                default:
                    throw new Exception("Incompatible type!");
            }

            return dataType;
        }
        public static bool VerifyCondition(GoapCondition condition)
        {
            switch (condition.GoapDataType)
            {
                case GoapDataType.Bool:
                    return condition.ConditionDirection is ConditionDirection.Equals
                        or ConditionDirection.NotEquals;
                case GoapDataType.Int:
                    return condition.ConditionDirection is ConditionDirection.GreaterThanEq
                        or ConditionDirection.LessThanEq;
                case GoapDataType.Float:
                    return condition.ConditionDirection is ConditionDirection.GreaterThanEq
                        or ConditionDirection.LessThanEq;
                case GoapDataType.Enum:
                    return condition.ConditionDirection is ConditionDirection.Equals
                        or ConditionDirection.NotEquals;
            }

            return false;
        }
        
        public static bool VerifyEffect(GoapEffect effect)
        {
            switch (effect.GoapDataType)
            {
                case GoapDataType.Bool:
                case GoapDataType.Enum:
                    return effect.EffectDirection is EffectDirection.Set;
                case GoapDataType.Int:
                case GoapDataType.Float:
                    return effect.EffectDirection is not EffectDirection.Set;
            }

            return false;
        }

        
    }
}