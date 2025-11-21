using System;
using UnityEngine;

namespace GOAP.goap
{
    public class GoapPlanner : MonoBehaviour
    {
        private void Start()
        {
            WorldState worldState = new WorldState();
            worldState.Add("Health", 5);
            bool does = EvaluateSatisfaction("Health", worldState , (x) => ((int)x + 1), y => (int)y >= 5);
            Debug.Log(does);
        }
        
        // 1. Temporarily modify the WorldState using effect.
        // 2. Evaluate condition based on the new value of key in WorldState.
        // 3. Restore WorldState.
        // 4. Return the result of condition.
        private static bool EvaluateSatisfaction(string key, WorldState worldState, Func<object,object> effect, Predicate<object> condition)
        {
            object oldValue = worldState.Get<object>(key);
            worldState.Update(key, effect(oldValue));
            object newValue = worldState.Get<object>(key);
            bool satisfies = condition(newValue);
            worldState.Update(key, oldValue);
            return satisfies;
        }
    }
}