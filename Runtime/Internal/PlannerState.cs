using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public class PlannerState : IWorldState
    {
        private readonly Dictionary<string, object> data;

        public PlannerState(Dictionary<string, object> data)
        {
            this.data = data;
        }
        
        public T Get<T>(string key)
        {
            if (!data.TryGetValue(key, out var value))
                return default;

            if (value is T v) 
                return v;
            
            Debug.LogError("Value of Key [" + key + "] is not of type [" + typeof(T) + "].");
            return default;
        }

        public void Update<T>(string key, T newValue)
        {
            if (!data.TryGetValue(key, out var oldValue))
            {
                Debug.LogError("Key [" + key + "] does not exist in PlannerState!");
                return;
            }

            if (oldValue is not T)
            {
                Debug.LogError("Value of Key [" + key + "] is not of type [" + typeof(T) + "].");
                return;
            }

            data[key] = newValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is not PlannerState other)
                return false;

            return other.data.Count == data.Count &&
                   other.data.All(x => 
                       data.TryGetValue(x.Key, out var v) && data[x.Key].Equals(v));
        }

        public override int GetHashCode()
        {
            int oldHash = HashCode.Combine(data.Count);

            return data.Aggregate(oldHash, (current, pair) => HashCode.Combine(current, pair.Key, pair.Value));
        }

        public IWorldState Clone()
        {
            return new PlannerState(new Dictionary<string, object>(data));
        }
    }
}