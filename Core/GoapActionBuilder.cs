using System;
using System.Collections.Generic;
using GOAP.Core.Planning;
using GOAP.Util;

namespace GOAP.Core
{
    public interface IGoapActionBuilder
    {
        IGoapActionStrategyBuilder CreateAction(string name);
    }

    public interface IGoapActionStrategyBuilder
    {
        IGoapActionCostBuilder WithStrategy(IActionStrategy strategy);
    }

    public interface IGoapActionCostBuilder
    {
        IFinalGoapActionBuilder WithCost(Func<PlannerState,int> costFunc);
    }
    public interface IFinalGoapActionBuilder
    {
        IFinalGoapActionBuilder WithCondition(GoapCondition condition);
        IFinalGoapActionBuilder WithEffect(GoapEffect effect);
        GoapAction Build();
    }
    
    public class GoapActionBuilder : IGoapActionBuilder,
        IGoapActionStrategyBuilder, IGoapActionCostBuilder, IFinalGoapActionBuilder
    {
        private string name;
        private HashSet<GoapCondition> conditions;
        private HashSet<GoapEffect> effects;
        private IActionStrategy actionStrategy;
        private Func<PlannerState, int> costFunc;
        
        public GoapActionBuilder()
        {
            name = "";
            conditions = new HashSet<GoapCondition>();
            effects = new HashSet<GoapEffect>();
        }
        
        public IGoapActionStrategyBuilder CreateAction(string actionName)
        {
            name = actionName;
            return this;
        }

        public IGoapActionCostBuilder WithStrategy(IActionStrategy strategy)
        {
            actionStrategy = strategy;
            return this;
        }

        public IFinalGoapActionBuilder WithCost(Func<PlannerState, int> costFunction)
        {
            costFunc = costFunction;
            return this;
        }
        public IFinalGoapActionBuilder WithCondition(GoapCondition condition)
        {
            if (!GoapUtils.VerifyCondition(condition))
                throw new Exception("Incorrect format of condition!");
            
            conditions.Add(condition);
            return this;
        }

        public IFinalGoapActionBuilder WithEffect(GoapEffect effect)
        {
            if (!GoapUtils.VerifyEffect(effect))
                throw new Exception("Incorrect format of condition!");
            
            effects.Add(effect);
            return this;
        }

        public GoapAction Build()
        {
            GoapAction newAction = new GoapAction(name, actionStrategy, costFunc, conditions, effects);
            name = "";
            conditions = new HashSet<GoapCondition>();
            effects = new HashSet<GoapEffect>();
            actionStrategy = null;
            costFunc = null;
            return newAction;
        }
    }
}