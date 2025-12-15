using System;
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
                    switch (condition.ConditionState)
                    { 
                        case ConditionState.Equals:
                            return boolVal == b;
                        case ConditionState.NotEquals:
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
                        case ConditionState.Equals:
                            return intVal == i;
                        case ConditionState.NotEquals:
                            return intVal != i;
                        case ConditionState.GreaterThan:
                            return intVal > i;
                        case ConditionState.LessThan:
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
                    
                    switch (condition.ConditionState)
                    {
                        case ConditionState.Equals:
                            return Math.Abs(floatVal - f) < EPSILON;
                        case ConditionState.GreaterThan:
                            return floatVal > f;
                        case ConditionState.LessThan:
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
                    
                    switch (condition.ConditionState)
                    {
                        case ConditionState.Equals:
                            return enumVal == e;
                        case ConditionState.NotEquals:
                            return enumVal != e;
                        default:
                            LogComparisonError();
                            return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // This method assumes effect and condition are well formated. 
        // This method needs to determine if this effect will get the condition CLOSER to the world state.
        // The effect needs to think - what is the condition asking of me? Can I get closer to what it wants?
        // If so, return the new condition.
        public static bool EffectSatisfiesCondition(GoapEffect effect, GoapCondition condition, PlannerState state)
        {
            // Invalid vvvvvvv - Next job.
            object oldValue = state.Get<object>(condition.Key);

            switch (effect.EffectDirection)
            {
                case EffectState.Set:
                    object newValue = effect.Value;
                    
                    
                    
                    break;
                case EffectState.Increase:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            state.Update(effect.Key, (int)oldValue + (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            state.Update(effect.Key, (float)oldValue + (float)effect.Value);
                            break;
                        default:
                            Debug.LogError("Malformatted GoapEffect!");
                            break;
                    }
                    
                    break;
                case EffectState.Decrease:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            state.Update(effect.Key, (int)oldValue - (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            state.Update(effect.Key, (float)oldValue - (float)effect.Value);
                            break;
                        default:
                            Debug.LogError("Malformatted GoapEffect!");
                            break;
                    }
                    break;
            }

            if (ConditionIsSatisfied(condition, state))
            {
                
            }

            return false;
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