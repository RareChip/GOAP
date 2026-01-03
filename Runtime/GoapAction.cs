using System;
using System.Collections.Generic;

namespace GOAP.Runtime
{
    public sealed class GoapAction
    {
        public string ActionName { get; private set; }
        public Func<IWorldState, int> CalculateCost { get; private set; }
        public HashSet<GoapCondition> Conditions { get; private set; }
        public Dictionary<string, GoapCondition> ConditionMap { get; private set; }
        public HashSet<GoapEffect> Effects { get; private set; }
        public Dictionary<string, GoapEffect> EffectMap { get; private set; }
        
        private readonly IActionStrategy actionStrategy;

        public GoapAction(string actionName, IActionStrategy actionStrategy, Func<IWorldState, int> costFunc, 
            HashSet<GoapCondition> conditions, HashSet<GoapEffect> effects)
        {
            this.ActionName = actionName;
            this.actionStrategy = actionStrategy;
            this.CalculateCost = costFunc;
            this.Conditions = conditions;
            this.Effects = effects;

            this.ConditionMap = new Dictionary<string, GoapCondition>();
            foreach (GoapCondition goapCondition in conditions)
            {
                this.ConditionMap.Add(goapCondition.Key, goapCondition);
            }

            this.EffectMap = new Dictionary<string, GoapEffect>();
            foreach (GoapEffect goapEffect in effects)
            {
                this.EffectMap.Add(goapEffect.Key, goapEffect);
            }
        }
        
        public void StartAction() => this.actionStrategy.Start();
        public void ExecuteAction() => this.actionStrategy.Execute();
        public void StopAction() => this.actionStrategy.Stop();
        
    }
}
