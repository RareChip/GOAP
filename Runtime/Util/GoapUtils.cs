using System;
using System.Collections.Generic;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;

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
        public static HashSet<GoapAction> AddAllActions(params GoapAction[] actions)
        {
            HashSet<GoapAction> actionsSet = new HashSet<GoapAction>();
            foreach (GoapAction goapAction in actions)
            {
                actionsSet.Add(goapAction);
            }

            return actionsSet;
        }
        
        public static HashSet<GoapGoal> AddAllGoals(params GoapGoal[] goals)
        {
            HashSet<GoapGoal> goalSet = new HashSet<GoapGoal>();
            foreach (GoapGoal goal in goals)
            {
                goalSet.Add(goal);
            }

            return goalSet;
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

        public static void ApplyEffectsToPlannerState(HashSet<GoapEffect> effects, PlannerState plannerState)
        {
            // foreach (GoapEffect effect in effects)
            // {
            //     switch (effect.EffectDirection)
            //     {
            //         case EffectDirection.Set:
            //             plannerState.Update(effect.Key, effect.Value);
            //             break;
            //         case EffectDirection.Increase:
            //             switch (effect.GoapDataType)
            //             {
            //                 case GoapDataType.Int:
            //                     plannerState.Update(effect.Key, plannerState.Get<int>(effect.Key) + (int)effect.Value);
            //                     break;
            //                 case GoapDataType.Float:
            //                     plannerState.Update(effect.Key, plannerState.Get<float>(effect.Key) + (float)effect.Value);
            //                     break;
            //                 default:
            //                     throw new ArgumentOutOfRangeException();
            //             }
            //             
            //             break;
            //         case EffectDirection.Decrease:
            //             switch (effect.GoapDataType)
            //             {
            //                 case GoapDataType.Int:
            //                     plannerState.Update(effect.Key, plannerState.Get<int>(effect.Key) - (int)effect.Value);
            //                     break;
            //                 case GoapDataType.Float:
            //                     plannerState.Update(effect.Key, plannerState.Get<float>(effect.Key) - (float)effect.Value);
            //                     break;
            //                 default:
            //                     throw new ArgumentOutOfRangeException();
            //             }
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException();
            //     }
            // }
        }
    }
}