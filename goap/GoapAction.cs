using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.goap
{
    public sealed class GoapAction
    {
        private HashSet<GoapCondition> conditions;
        private HashSet<GoapEffect> effects;
        private IActionStrategy actionStrategy;
        private Func<WorldState, float> calculateCost;

        public float CalculateCost(WorldState worldState) => calculateCost(worldState);
        
        public void StartAction() => actionStrategy.Start();
        public void ExecuteAction() => actionStrategy.Execute();
        public void StopAction() => actionStrategy.Stop();
    }
}
