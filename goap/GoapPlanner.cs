using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.goap
{
    public class GoapPlanner : MonoBehaviour
    {
        private void Start()
        {
            WorldState worldState = new WorldState();
            worldState.Add("Health", 5);
            bool does = EvaluateSatisfaction<int>("Health", worldState, (x) => x + 1, (z,y) => y >= 5);
            Debug.Log(does);
        }

        // Implement A*, run it on the main thread. Then, get this working with Jobs.
        public GoapAction[] GeneratePlan(Dictionary<GoapAction, GoapAction> adjacencyList)
        {
            HashSet<GoapAction> visited = new HashSet<GoapAction>();
            
            
            return null;
        }
        
        
        // This function must always complete amd build an adjacency list mapping Actions to other Actions that 
        // satisfy their preconditions before planning.
        private static bool EvaluateSatisfaction<T>(string key, WorldState worldState, Func<T,T> effect, Func<T,T,bool> condition)
        {
            T oldValue = worldState.Get<T>(key);
            T newValue = effect(oldValue);
            bool satisfies = condition(oldValue,newValue);
            return satisfies;
        }
    }
}