using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.Core.Planning
{
    public class PlannerState
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

            if (value is Func<T> fun) 
                return fun();

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

        public PlannerState Clone()
        {
            return new PlannerState(new Dictionary<string, object>(data));
        }
    }
}