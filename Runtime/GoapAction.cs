using System;
using System.Collections.Generic;

namespace GOAP.Runtime
{
    public sealed class GoapAction
    {
        public string ActionName { get; private set; }
        public Func<IWorldState, int> CalculateCost { get; private set; }
        public HashSet<GoapCondition> Conditions { get; private set; }
        public HashSet<GoapEffect> Effects { get; private set; }
        
        private readonly IActionStrategy actionStrategy;

        public GoapAction(string actionName, IActionStrategy actionStrategy, Func<IWorldState, int> costFunc, 
            HashSet<GoapCondition> conditions, HashSet<GoapEffect> effects)
        {
            ActionName = actionName;
            this.actionStrategy = actionStrategy;
            CalculateCost = costFunc;
            Conditions = conditions;
            Effects = effects;
        }
        
        public void StartAction() => actionStrategy.Start();
        public void ExecuteAction() => actionStrategy.Execute();
        public void StopAction() => actionStrategy.Stop();
        
    }
}
