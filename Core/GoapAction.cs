using System;
using System.Collections.Generic;
using GOAP.Core.Agent;

namespace GOAP.Core
{
    public sealed class GoapAction
    {
        public string ActionName { get; private set; }
        public HashSet<GoapCondition> Conditions { get; private set; }
        public HashSet<GoapEffect> Effects { get; private set; }
        
        private IActionStrategy actionStrategy;
        private Func<WorldState, int> calculateCost;
        
        public int CalculateCost(WorldState worldState) => calculateCost(worldState);
        
        public void StartAction() => actionStrategy.Start();
        public void ExecuteAction() => actionStrategy.Execute();
        public void StopAction() => actionStrategy.Stop();
        
    }
}
