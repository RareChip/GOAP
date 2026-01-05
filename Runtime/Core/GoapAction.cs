using System;
using System.Collections.Generic;
using GOAP.Runtime.API;

namespace GOAP.Runtime.Core
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
            this.ActionName = actionName;
            this.actionStrategy = actionStrategy;
            this.CalculateCost = costFunc;
            this.Conditions = conditions;
            this.Effects = effects;
        }
        
        public void StartAction() => this.actionStrategy.Start();
        public void ExecuteAction() => this.actionStrategy.Execute();
        public void StopAction() => this.actionStrategy.Stop();
        
    }
}
