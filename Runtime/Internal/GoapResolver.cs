using System;
using System.Collections.Generic;
using GOAP.Runtime.Util;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public static class GoapResolver
    {
        private const float EPSILON = 0.0001f;
        
        public static bool ConditionIsSatisfied(GoapCondition condition, IWorldState plannerState)
        {
            switch (condition.GoapDataType)
            {
                case GoapDataType.Bool:
                    if (condition.Value is not bool b)
                    {
                        throw TypeException();
                    }
                    
                    bool boolVal = plannerState.Get<bool>(condition.Key);
                    switch (condition.ConditionDirection)
                    { 
                        case ConditionDirection.Equals:
                            return boolVal == b;
                        case ConditionDirection.NotEquals:
                            return boolVal != b;
                        default:
                            throw ComparisonException();
                    }
                    
                case GoapDataType.Int:
                    if (condition.Value is not int i)
                    {
                        throw TypeException();
                    }

                    int intVal = plannerState.Get<int>(condition.Key);
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return intVal == i;
                        case ConditionDirection.NotEquals:
                            return intVal != i;
                        case ConditionDirection.GreaterThanEq:
                            return intVal >= i;
                        case ConditionDirection.LessThanEq:
                            return intVal <= i;
                        default:
                            throw ComparisonException();
                    }
                    
                case GoapDataType.Float:
                    if (condition.Value is not float f)
                    {
                        throw TypeException();
                    }
                    
                    float floatVal = plannerState.Get<float>(condition.Key);
                    
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return Math.Abs(floatVal - f) < EPSILON;
                        case ConditionDirection.GreaterThanEq:
                            return floatVal >= f;
                        case ConditionDirection.LessThanEq:
                            return floatVal <= f;
                        default:
                            throw ComparisonException();
                    };
                case GoapDataType.Enum:
                    if (condition.Value is not int e)
                    {
                        throw TypeException();
                    }

                    int enumVal = plannerState.Get<int>(condition.Key);
                    
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return enumVal == e;
                        case ConditionDirection.NotEquals:
                            return enumVal != e;
                        default:
                            throw ComparisonException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public static bool EffectsSatisfyConditions(HashSet<GoapEffect> effects, 
            Dictionary<string,GoapCondition> conditionMap)
        { 
            bool satisfiesSomething = false;
            foreach (GoapEffect effect in effects)
            {
                if (!conditionMap.TryGetValue(effect.Key, out GoapCondition condition))
                    continue;

                // We will always return false if any of the effects contradicts any of the conditions.
                switch (condition.ConditionDirection)
                {
                    case ConditionDirection.Equals:
                        if (effect.EffectDirection == EffectDirection.Set && effect.Value.Equals(condition.Value))
                        {
                            satisfiesSomething = true;
                            break;
                        }
                        return false;
                    case ConditionDirection.NotEquals:
                        if (effect.EffectDirection == EffectDirection.Set && !effect.Value.Equals(condition.Value))
                        {
                            satisfiesSomething = true;
                            break;
                        }

                        return false;
                    case ConditionDirection.LessThanEq:
                        switch (effect.EffectDirection)
                        {
                            case EffectDirection.Increase:
                                // Contradiction
                                break;
                            case EffectDirection.Decrease:
                                satisfiesSomething = true;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case ConditionDirection.GreaterThanEq:
                        switch (effect.EffectDirection)
                        {
                            case EffectDirection.Increase:
                                satisfiesSomething = true;
                                break;
                            case EffectDirection.Decrease:
                                // Contradiction
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                }
            }

            return satisfiesSomething;
        }
        
        public static HashSet<GoapCondition> ApplyEffects(Dictionary<string,GoapEffect> effectMap, 
            PlannerNode currentNode, IWorldState worldState)
        {
            HashSet<GoapCondition> conditions = currentNode.Conditions;
            HashSet<GoapCondition> newConditions = new HashSet<GoapCondition>();
            
            foreach (GoapCondition condition in conditions)
            {
                GoapCondition newCondition = condition;

                if (effectMap.TryGetValue(condition.Key, out GoapEffect effect))
                {
                    newCondition = ApplyEffectToCondition(effect, condition, currentNode, worldState);
                }
                
                if(!newCondition.Equals(default))
                    newConditions.Add(newCondition);
            }

            return newConditions;
        }

        private static GoapCondition ApplyEffectToCondition(GoapEffect effect, GoapCondition condition,
        PlannerNode node, IWorldState worldState)
        {
            switch (effect.EffectDirection)
            {
                case EffectDirection.Set:

                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Bool:
                        case GoapDataType.Enum:
                            return !effect.Value.Equals(condition.Value) ? condition : default;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case EffectDirection.Increase:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int newVal = (int)condition.Value - (int)effect.Value;
                            condition.Value = newVal;

                            if (ConditionIsSatisfied(condition, worldState))
                            {
                                node.RemoveAndCacheCondition(condition);
                                return default;
                            }
                            
                            return condition;
                        case GoapDataType.Float:
                            float newFloat = (float)condition.Value - (float)effect.Value;
                            condition.Value = newFloat;
                            
                            if (ConditionIsSatisfied(condition, worldState))
                            {
                                node.RemoveAndCacheCondition(condition);
                                return default;
                            }
                            
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
                            
                            if (ConditionIsSatisfied(condition, worldState))
                            {
                                node.RemoveAndCacheCondition(condition);
                                return default;
                            }
                            return condition;
                        case GoapDataType.Float:
                            float newFloat = (float)condition.Value + (float)effect.Value;
                            condition.Value = newFloat;
                            
                            if (ConditionIsSatisfied(condition, worldState))
                            {
                                node.RemoveAndCacheCondition(condition);
                                return default;
                            }
                            return condition;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static HashSet<GoapCondition> CombineConditionSets(HashSet<GoapCondition> set1,HashSet<GoapCondition> set2)
        {
            HashSet<GoapCondition> combinedSet = new HashSet<GoapCondition>();
            HashSet<string> combinedKeys = new HashSet<string>();
            Dictionary<string, GoapCondition> otherCondMap = new Dictionary<string, GoapCondition>();

            foreach (GoapCondition goapCondition in set2)
            {
                otherCondMap.Add(goapCondition.Key, goapCondition);
            }
            
            foreach (GoapCondition goapCondition in set1)
            {
                if (!otherCondMap.TryGetValue(goapCondition.Key, out GoapCondition other))
                {
                    combinedSet.Add(goapCondition);
                }
                else
                {
                    // If we find a contradiction, we return null.
                    switch (goapCondition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            if ((other.ConditionDirection == ConditionDirection.NotEquals
                                && (bool)other.Value == (bool)goapCondition.Value)
                                || (other.ConditionDirection == ConditionDirection.Equals
                                && (bool)other.Value != (bool)goapCondition.Value))
                                return null;
                            combinedSet.Add(goapCondition);
                            break;
                        case ConditionDirection.NotEquals:
                            if ((other.ConditionDirection == ConditionDirection.NotEquals
                                 && (bool)other.Value != (bool)goapCondition.Value)
                                || (other.ConditionDirection == ConditionDirection.Equals
                                    && (bool)other.Value == (bool)goapCondition.Value))
                                return null;
                            combinedSet.Add(goapCondition);
                            break;
                        case ConditionDirection.LessThanEq:
                            if (other.ConditionDirection == ConditionDirection.GreaterThanEq)
                                return null;

                            GoapCondition lessCond;
                            if (goapCondition.Value is int i)
                            {
                                lessCond = i < (int)other.Value ? goapCondition : other;
                            } else if (goapCondition.Value is float f)
                            {
                                lessCond = f < (float)other.Value ? goapCondition : other;
                            }
                            else
                            {
                                throw new Exception("Incorrect typing!");
                            }

                            combinedSet.Add(lessCond);
                            break;
                        case ConditionDirection.GreaterThanEq:
                            if (other.ConditionDirection == ConditionDirection.LessThanEq)
                                return null;

                            GoapCondition greaterCond;
                            if (goapCondition.Value is int j)
                            {
                                greaterCond = j > (int)other.Value ? goapCondition : other;
                            } else if (goapCondition.Value is float fl)
                            {
                                greaterCond = fl > (float)other.Value ? goapCondition : other;
                            }
                            else
                            {
                                throw new Exception("Incorrect typing!");
                            }

                            combinedSet.Add(greaterCond);
                            break;
                    }    
                }

                combinedKeys.Add(goapCondition.Key);

            }

            foreach (GoapCondition goapCondition in otherCondMap.Values)
            {
                if (combinedKeys.Contains(goapCondition.Key))
                    continue;

                combinedSet.Add(goapCondition);
            }
            
            return combinedSet;
        }

        private static Exception TypeException()
        {
            return new Exception("Condition compared against incorrect type in world state!");
        }

        private static Exception ComparisonException()
        {
            return new Exception("Condition has correct type, but an incompatible comparison method!");
        }

    }
}