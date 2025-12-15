using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.Core.Agent
{
    public class WorldStateBuilder
    {
        private readonly Dictionary<string, Func<object>> data = new();

        public WorldStateBuilder AddData(string key, Func<object> val)
        {
            if(!data.TryAdd(key,val))
            {
                Debug.LogError("Key [" + key + "] is already in WorldState data.");
            }

            return this;
        }

        public WorldState Build()
        {
            return new WorldState(data);
        }
    }
}