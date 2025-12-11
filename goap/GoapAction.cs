using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.goap
{
    public sealed class GoapAction
    {
        public string ActionName { get; private set; }
        public HashSet<GoapCondition> Conditions { get; private set; }
        public HashSet<GoapEffect> Effects { get; private set; }
        
        private IActionStrategy actionStrategy;
        private Func<WorldState, float> calculateCost;
        
        public float CalculateCost(WorldState worldState) => calculateCost(worldState);
        
        public void StartAction() => actionStrategy.Start();
        public void ExecuteAction() => actionStrategy.Execute();
        public void StopAction() => actionStrategy.Stop();
    }
}
