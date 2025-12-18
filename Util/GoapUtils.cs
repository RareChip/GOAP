using System;
using GOAP.Core;
using UnityEngine;

namespace GOAP.Util
{
    public static class GoapUtils
    {
        public static GoapCondition CreateEnumCondition(string key, ConditionDirection direction, int enumValue)
        {
            return new GoapCondition
            {
                Key = key,
                Value = enumValue,
                ConditionDirection = direction,
                GoapDataType = GoapDataType.Enum
            };
        }

        public static bool VerifyCondition(GoapCondition condition)
        {
            switch (condition.GoapDataType)
            {
                case GoapDataType.Bool:
                    return condition.ConditionDirection is ConditionDirection.Equals
                        or ConditionDirection.NotEquals;
                case GoapDataType.Int:
                    return true;
                case GoapDataType.Float:
                    return condition.ConditionDirection is ConditionDirection.GreaterThan
                        or ConditionDirection.LessThan;
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
                    return effect.EffectDirection is EffectDirection.Set;
                case GoapDataType.Int:
                    return true;
                case GoapDataType.Float:
                    return effect.EffectDirection is not EffectDirection.Set;
                case GoapDataType.Enum:
                    return effect.EffectDirection is EffectDirection.Set;
            }

            return false;
        }
    }
}