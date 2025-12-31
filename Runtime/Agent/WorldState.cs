using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Internal;
using UnityEngine;

namespace GOAP.Runtime.Agent
{
    public class WorldState : IWorldState
    {
        private readonly Dictionary<string, Func<object>> data;
        
        public WorldState(Dictionary<string, Func<object>> data)
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

        public void Update<T>(string key, T value)
        {
            // noop
        }

        public IWorldState Clone()
        {
            return CreatePlannerState();
        }

        public IWorldState CreatePlannerState()
        {
            Dictionary<string, object> calculatedSnapshot = new Dictionary<string, object>();

            foreach (var pair in data)
            {
                calculatedSnapshot.Add(pair.Key, pair.Value());
            }
            
            return new PlannerState(calculatedSnapshot);
        }
    }
}