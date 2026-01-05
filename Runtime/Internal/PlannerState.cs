using System;
using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Internal
{
    public class PlannerState : IWorldState
    {
        private const float EPSILON = 0.0001f;
        private readonly Dictionary<string, int> numMap;
        private readonly int[] ints;
        private readonly float[] floats;
        private readonly bool[] bools;
        
        public PlannerState(Dictionary<string, object> data)
        {
            if (data == null)
                return;
            
            this.numMap = new Dictionary<string, int>();
            List<int> intList = new List<int>();
            List<float> floatList = new List<float>();
            List<bool> boolList = new List<bool>();
            
            foreach (KeyValuePair<string, object> pair in data)
            {
                string key = pair.Key;
                switch (pair.Value)
                {
                    case int i:
                        this.numMap.Add(key, intList.Count);
                        intList.Add(i);
                        break;
                    case float f:
                        this.numMap.Add(key, floatList.Count);
                        floatList.Add(f);
                        break;
                    case bool b:
                        this.numMap.Add(key, boolList.Count);
                        boolList.Add(b);
                        break;
                    default:
                        throw new InvalidOperationException("Incompatible data type.");
                }
            }

            this.ints = intList.ToArray();
            this.floats = floatList.ToArray();
            this.bools = boolList.ToArray();
        }

        private PlannerState(PlannerState initializer)
        {
            this.numMap = initializer.numMap;
            
            this.ints = new int[initializer.ints.Length];
            this.floats = new float[initializer.floats.Length];
            this.bools = new bool[initializer.bools.Length];
            
            Array.Copy(initializer.ints, this.ints, initializer.ints.Length);
            Array.Copy(initializer.floats, this.floats, initializer.floats.Length);
            Array.Copy(initializer.bools, this.bools, initializer.bools.Length);
        }
        public int GetInt(string key)
        {
            if (!this.numMap.TryGetValue(key, out int index))
            {
                throw new Exception($"Key [{key}] not found in WorldState!");
            }

            return this.GetInt(index);
        }

        public int GetInt(int index)
        {
            return this.ints[index];
        }
        
        public float GetFloat(string key)
        {
            if (!this.numMap.TryGetValue(key, out int index))
            {
                throw new Exception($"Key [{key}] not found in WorldState!");
            }

            return this.GetFloat(index);
        }

        public float GetFloat(int index)
        {
            return this.floats[index];
        }

        public bool GetBool(string key)
        {
            if (!this.numMap.TryGetValue(key, out int index))
            {
                throw new Exception($"Key [{key}] not found in WorldState!");
            }
            
            return this.GetBool(index);
        }

        public bool GetBool(int index)
        {
            return this.bools[index];
        }
        
        private void UpdateInt(string key, int newValue)
        {
            if (!this.numMap.TryGetValue(key, out int index))
            {
                throw new Exception($"Key {key} not found!");
            }
            this.ints[index] = newValue;
        }

        private void UpdateFloat(string key, float newValue)
        {
            if (!this.numMap.TryGetValue(key, out int index))
            {
                throw new Exception($"Key {key} not found!");
            }
            this.floats[index] = newValue;
        }

        private void UpdateBool(string key, bool newValue)
        {
            if (!this.numMap.TryGetValue(key, out int index))
            {
                throw new Exception($"Key {key} not found!");
            }
            this.bools[index] = newValue;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is not PlannerState other)
                return false;

            if (this.ints.Length != other.ints.Length)
                return false;
            
            if (this.floats.Length != other.floats.Length)
                return false;
            
            if (this.bools.Length != other.bools.Length)
                return false;
            
            for (int i = 0; i < this.ints.Length; i++)
            {
                if (this.ints[i] != other.ints[i])
                    return false;
            }
            
            for (int i = 0; i < this.floats.Length; i++)
            {
                if (Math.Abs(this.floats[i] - other.floats[i]) > EPSILON)
                    return false;
            }
            
            for (int i = 0; i < this.bools.Length; i++)
            {
                if (this.bools[i] != other.bools[i])
                    return false;
            }
            
            return true;
        }

        public override int GetHashCode()
        {
            HashCode myHash = new HashCode();
            myHash.Add(this.ints.Length);
            myHash.Add(this.floats.Length);
            myHash.Add(this.bools.Length);
            
            foreach (int i in this.ints)
            {
                myHash.Add(i);
            }
            
            foreach (float f in this.floats)
            {
                myHash.Add(f);
            }
            
            foreach (bool b in this.bools)
            {
                myHash.Add(b);
            }
            
            return myHash.ToHashCode();
        }
        
        public IWorldState Clone()
        {
            return new PlannerState(this);
        }

        public void ApplyEffect(GoapEffect effect)
        {
            switch (effect.EffectDirection)
            {
                case EffectDirection.Set:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Bool:
                            this.UpdateBool(effect.Key, (bool)effect.Value);
                            break;
                        case GoapDataType.Int:
                        case GoapDataType.Enum:
                            this.UpdateInt(effect.Key, (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            this.UpdateFloat(effect.Key, (float)effect.Value);
                            break;
                    }
                    break;
                case EffectDirection.Increase:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int oldInt = this.GetInt(effect.Key);
                            this.UpdateInt(effect.Key, oldInt + (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            float oldFloat = this.GetFloat(effect.Key);
                            this.UpdateFloat(effect.Key, oldFloat + (float)effect.Value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EffectDirection.Decrease:
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            int oldInt = this.GetInt(effect.Key);
                            this.UpdateInt(effect.Key, oldInt - (int)effect.Value);
                            break;
                        case GoapDataType.Float:
                            float oldFloat = this.GetFloat(effect.Key);
                            this.UpdateFloat(effect.Key, oldFloat - (float)effect.Value);
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
                        throw new InvalidTypeException(condition.Value, typeof(bool));
                    }

                    bool boolVal = this.GetBool(condition.Key);
                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return boolVal == b;
                        case ConditionDirection.NotEquals:
                            return boolVal != b;
                        default:
                            throw new ComparisonException();
                    }

                case GoapDataType.Int:
                    if (condition.Value is not int i)
                    {
                        throw new InvalidTypeException(condition.Value, typeof(int));
                    }

                    int intVal = this.GetInt(condition.Key);
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
                            throw new ComparisonException();
                    }

                case GoapDataType.Float:
                    if (condition.Value is not float f)
                    {
                        throw new InvalidTypeException(condition.Value, typeof(float));
                    }

                    float floatVal = this.GetFloat(condition.Key);

                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return Math.Abs(floatVal - f) < EPSILON;
                        case ConditionDirection.GreaterThanEq:
                            return floatVal >= f;
                        case ConditionDirection.LessThanEq:
                            return floatVal <= f;
                        default:
                            throw new ComparisonException();
                    }
                    
                case GoapDataType.Enum:
                    if (condition.Value is not int e)
                    {
                        throw new InvalidTypeException(condition.Value, typeof(int));
                    }

                    int enumVal = this.GetInt(condition.Key);

                    switch (condition.ConditionDirection)
                    {
                        case ConditionDirection.Equals:
                            return enumVal == e;
                        case ConditionDirection.NotEquals:
                            return enumVal != e;
                        default:
                            throw new ComparisonException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}