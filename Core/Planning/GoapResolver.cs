using System;
using System.Collections.Generic;
using GOAP.Util;
using UnityEngine;

namespace GOAP.Core.Planning
{
    public static class GoapResolver
    {
        private const float EPSILON = 0.0001f;
        
        public static bool ConditionIsSatisfied(GoapCondition condition, PlannerState plannerState)
        {
            switch (condition.GoapDataType)
            {
                case GoapDataType.Bool:
                    if (condition.Value is not bool b)
                    {
                        LogTypeError();
                        return false;
                    }
                    
                    bool boolVal = plannerState.Get<bool>(condition.Key);
                    switch (condition.ConditionDirection)
                    { 
                        case ConditionDirection.Equals:
                            return boolVal == b;
                        case ConditionDirection.NotEquals:
                            return boolVal != b;
                        default:
                            LogComparisonError();
                            return false;
                    }
                    
                case GoapDataType.Int:
                    if (condition.Value is not int i)
                    {
                        LogTypeError();
                        return false;
                    }

                    int intVal = plannerState.Get<int>(condition.Key);
                    switch (condition.Value)
                    {
                        case ConditionDirection.Equals:
                            return intVal == i;
                        case ConditionDirection.NotEquals:
                            return intVal != i;
                        case ConditionDirection.GreaterThan:
                            return intVal > i;
                        case ConditionDirection.LessThan:
                            return intVal < i;
                        default:
                            LogComparisonError();
                            return false;
                    }
                    
                case GoapDataType.Float:
                    if (condition.Value is not float f)
                    {
                        LogTypeError();
                        return false;
                    }
                    
                    float floatVal = plannerState.Get<float>(condition.Key);
                    
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return Math.Abs(floatVal - f) < EPSILON;
                        case ConditionDirection.GreaterThan:
                            return floatVal > f;
                        case ConditionDirection.LessThan:
                            return floatVal < f;
                        default:
                            LogComparisonError();
                            return false;
                    };
                case GoapDataType.Enum:
                    if (condition.Value is not int e)
                    {
                        LogTypeError();
                        return false;
                    }

                    int enumVal = plannerState.Get<int>(condition.Key);
                    
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return enumVal == e;
                        case ConditionDirection.NotEquals:
                            return enumVal != e;
                        default:
                            LogComparisonError();
                            return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool EffectsResolveConditions(HashSet<GoapCondition> conditions, HashSet<GoapCondition> effectResults)
        {
            Dictionary<string, GoapCondition> effectMap = new Dictionary<string, GoapCondition>();

            foreach (GoapCondition cond in effectResults)
            {
                effectMap.Add(cond.Key, cond);
            }

            foreach (GoapCondition condition in conditions)
            {
                if (!effectMap.TryGetValue(condition.Key, out GoapCondition effectCondition))
                    continue;

                switch (condition.ConditionDirection)
                {
                    case ConditionDirection.Equals:
                        break;
                    case ConditionDirection.NotEquals:
                        break;
                    case ConditionDirection.LessThan:
                        break;
                    case ConditionDirection.GreaterThan:
                        break;
                }
            }

            return false;
        }
        public static HashSet<GoapCondition> ApplyEffects(GoapAction action, HashSet<GoapCondition> conditions)
        {
            HashSet<GoapCondition> newConditions = new HashSet<GoapCondition>();

            foreach (GoapCondition condition in conditions)
            {
                GoapCondition newCondition = condition;
                
                foreach (GoapEffect effect in action.Effects)
                {
                    if (condition.Key != effect.Key)
                        continue;

                    newCondition = ApplyEffectToCondition(effect, condition);
                    
                    break;
                }
                
                if(!newCondition.Equals(default))
                    newConditions.Add(newCondition);
            }

            return newConditions;
        }

        private static GoapCondition ApplyEffectToCondition(GoapEffect effect, GoapCondition condition)
        {
            switch (effect.EffectDirection)
            {
                case EffectDirection.Set:
                    return !effect.Value.Equals(condition.Value) ? condition : default;
                case EffectDirection.Increase:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int newVal = (int)condition.Value - (int)effect.Value;
                            condition.Value = newVal;
                            return condition;
                        case GoapDataType.Float:
                            float newFloat = (float)condition.Value - (float)effect.Value;
                            condition.Value = newFloat;
                            return condition;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case EffectDirection.Decrease:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int newVal = (int)condition.Value + (int)effect.Value;
                            condition.Value = newVal;
                            return condition;
                        case GoapDataType.Float:
                            float newFloat = (float)condition.Value + (float)effect.Value;
                            condition.Value = newFloat;
                            return condition;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static HashSet<GoapCondition> CombineConditionSets(HashSet<GoapCondition> set1, HashSet<GoapCondition> set2)
        {
            HashSet<GoapCondition> combinedSet = new HashSet<GoapCondition>();
            
            
            
            return combinedSet;
        }

        private static void LogTypeError()
        {
            Debug.LogError("Condition compared against incorrect type in world state!");
        }

        private static void LogComparisonError()
        {
            Debug.LogError("Condition has correct type, but an incompatible comparison method!");
        }

    }
}