using System;
using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Internal;
using UnityEngine;

namespace GOAP.Runtime.Unity
{
    public class WorldState
    {
        private readonly Dictionary<string, Func<object>> data;

        public WorldState()
        {
            data = new Dictionary<string, Func<object>>();
        }
        public WorldState(Dictionary<string, Func<object>> data)
        {
            this.data = data;
        }
        
        public WorldState(Dictionary<string, object> data)
        {
            this.data = new Dictionary<string, Func<object>>();
            foreach (KeyValuePair<string, object> pair in data)
            {
                this.data.Add(pair.Key, () => pair.Value);
            }
        }

        public void AddConditionData(string key, Func<object> getData)
        {
            if (getData() is not int or float or bool)
            {
                Debug.LogError("Only ints, float, bool, and enum types are supported in WorldState!");
                return;
            }
            if (!data.TryAdd(key, getData))
            {
                Debug.LogError($"Key {key} already exists in WorldState!");
            }
        }
        
        public IWorldState CreateSnapshot()
        {
            Dictionary<string, object> calculatedSnapshot = new Dictionary<string, object>();

            foreach (var pair in this.data)
            {
                calculatedSnapshot.Add(pair.Key, pair.Value());
            }
            
            return new PlannerState(calculatedSnapshot);
        }
    }
}