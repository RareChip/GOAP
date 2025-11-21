using System.Collections.Generic;
using UnityEngine;

namespace GOAP.goap
{
    public class WorldState
    {
        private readonly Dictionary<string, object> data = new();
        
        public T Get<T>(string key)
        {
            if (!data.TryGetValue(key, out var value))
                return default;

            if (value is T val)
            {
                return val;
            }
            
            Debug.LogError("Value of Key [" + key + "] is not of type [" + typeof(T) + "].");
            return default;
        }

        public void Add<T>(string key, T value)
        {
            if (!data.TryAdd(key, value))
            {
                Debug.LogError("Key [" + key + "] already exists in WorldState.");
            }
        }

        public void Update<T>(string key, T value)
        {
            if (!data.Remove(key))
            {
                Debug.LogError("Key [" + key + "] does not exist in the WorldState, so it cannot be updated.");
                return;
            }

            Add(key,value);
        }
    }
}