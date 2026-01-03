using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Util;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public class PlannerState : IWorldState
    {
        private const float EPSILON = 0.0001f;
        private readonly Dictionary<string, object> data;

        public PlannerState(Dictionary<string, object> data)
        {
            this.data = data;
        }

        public T Get<T>(string key)
        {
            if (!this.data.TryGetValue(key, out var value))
                return default;

            if (value is T v)
                return v;

            Debug.LogError("Value of Key [" + key + "] is not of type [" + typeof(T) + "].");
            return default;
        }


        public void Update<T>(string key, T newValue)
        {
            if (!this.data.TryGetValue(key, out var oldValue))
            {
                Debug.LogError("Key [" + key + "] does not exist in PlannerState!");
                return;
            }

            if (oldValue is not T)
            {
                Debug.LogError("Value of Key [" + key + "] is not of type [" + typeof(T) + "].");
                return;
            }

            this.data[key] = newValue;
        }

        public void ApplyEffect(GoapEffect effect)
        {
            switch (effect.EffectDirection)
            {
                case EffectDirection.Set:
                    this.Update(effect.Key, effect.Value);
                    break;
                case EffectDirection.Increase:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int oldInt = this.Get<int>(effect.Key);
                            this.Update(effect.Key, oldInt + (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            float oldFloat = this.Get<float>(effect.Key);
                            this.Update(effect.Key, oldFloat + (float)effect.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EffectDirection.Decrease:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int oldInt = this.Get<int>(effect.Key);
                            this.Update(effect.Key, oldInt - (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            float oldFloat = this.Get<float>(effect.Key);
                            this.Update(effect.Key, oldFloat - (float)effect.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
            }
        }

        public bool ConditionIsSatisfied(GoapCondition condition)
        {
            switch (condition.GoapDataType)
            {
                case GoapDataType.Bool:
                    if (condition.Value is not bool b)
                    {
                        throw GoapResolver.TypeException;
                    }

                    bool boolVal = this.Get<bool>(condition.Key);
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return boolVal == b;
                        case ConditionDirection.NotEquals:
                            return boolVal != b;
                        default:
                            throw GoapResolver.ComparisonException;
                    }

                case GoapDataType.Int:
                    if (condition.Value is not int i)
                    {
                        throw GoapResolver.TypeException;
                    }

                    int intVal = this.Get<int>(condition.Key);
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
                            throw GoapResolver.ComparisonException;
                    }

                case GoapDataType.Float:
                    if (condition.Value is not float f)
                    {
                        throw GoapResolver.TypeException;
                    }

                    float floatVal = this.Get<float>(condition.Key);

                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return Math.Abs(floatVal - f) < EPSILON;
                        case ConditionDirection.GreaterThanEq:
                            return floatVal >= f;
                        case ConditionDirection.LessThanEq:
                            return floatVal <= f;
                        default:
                            throw GoapResolver.ComparisonException;
                    }

                    ;
                case GoapDataType.Enum:
                    if (condition.Value is not int e)
                    {
                        throw GoapResolver.TypeException;
                    }

                    int enumVal = this.Get<int>(condition.Key);

                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return enumVal == e;
                        case ConditionDirection.NotEquals:
                            return enumVal != e;
                        default:
                            throw GoapResolver.ComparisonException;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not PlannerState other)
                return false;

            return other.data.Count == this.data.Count &&
                   other.data.All(x => this.data.TryGetValue(x.Key, out var v) && this.data[x.Key].Equals(v));
        }

        public override int GetHashCode()
        {
            int oldHash = HashCode.Combine(this.data.Count);

            return this.data.Aggregate(oldHash, (current, pair) => HashCode.Combine(current, pair.Key, pair.Value));
        }

        public IWorldState Clone()
        {
            return new PlannerState(new Dictionary<string, object>(this.data));
        }
    }
}