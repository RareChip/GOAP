using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.goap
{
    public class GoapPlanner
    {
        // This is interesting. We can implement an interface here for a planning strategy for goals. The default
        // implementation will use Utility AI
        public GoapGoal GenerateBestGoal(GoapGoal goals)
        {
            return null;
        }
        
        // Implement A*, run it on the main thread. Then, get this working with Jobs.
        public Queue<GoapAction> GeneratePlan(HashSet<GoapAction> actions, GoapGoal currentGoal)
        {
            // Should the actions adjacency list be updated on another thread? We can make a job for this.
            // Options: 1. have it updated every [interval] (always ready)
            //          2. update it right here, right before we need it. (takes some time)
            
            // Once we have the adjacency list, the problem is as simple as running A*. This should also be done on
            // another thread.
            
            HashSet<GoapAction> visited = new HashSet<GoapAction>();
            
            
            return null;
        }
        
        
        // This function must always complete and build an adjacency list mapping Actions to other Actions that 
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