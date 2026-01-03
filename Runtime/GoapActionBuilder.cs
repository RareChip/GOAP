using System;
using System.Collections.Generic;
using GOAP.Runtime.Internal;
using GOAP.Runtime.Util;

namespace GOAP.Runtime
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
        IFinalGoapActionBuilder WithCost(Func<IWorldState,int> costFunc);
    }
    public interface IFinalGoapActionBuilder
    {
        IFinalGoapActionBuilder WithCondition(string key, bool value);
        IFinalGoapActionBuilder WithCondition(string key, ConditionDirection direction, object value);
        IFinalGoapActionBuilder WithCondition(GoapCondition condition);
        IFinalGoapActionBuilder WithEffect(string key, bool value);
        IFinalGoapActionBuilder WithEffect(string key, EffectDirection direction, object value);
        IFinalGoapActionBuilder WithEffect(GoapEffect effect);
        GoapAction Build();
    }
    
    public class GoapActionBuilder : IGoapActionBuilder,
        IGoapActionStrategyBuilder, IGoapActionCostBuilder, IFinalGoapActionBuilder
    {
        private string name = "";
        private HashSet<GoapCondition> conditions = new();
        private HashSet<GoapEffect> effects = new();
        private IActionStrategy actionStrategy;
        private Func<IWorldState, int> costFunc;

        public IGoapActionStrategyBuilder CreateAction(string actionName)
        {
            this.name = actionName;
            return this;
        }

        public IGoapActionCostBuilder WithStrategy(IActionStrategy strategy)
        {
            this.actionStrategy = strategy;
            return this;
        }

        public IFinalGoapActionBuilder WithCost(Func<IWorldState, int> costFunction)
        {
            this.costFunc = costFunction;
            return this;
        }


        public IFinalGoapActionBuilder WithCondition(string key, bool value)
        {
            return this.WithCondition(new GoapCondition
            {
                Key = key,
                ConditionDirection = ConditionDirection.Equals,
                Value = value,
                GoapDataType = GoapDataType.Bool
            });
        }

        public IFinalGoapActionBuilder WithCondition(string key, ConditionDirection direction, object value)
        {
            return this.WithCondition(new GoapCondition
            {
                Key = key,
                ConditionDirection = direction,
                Value = value,
                GoapDataType = GoapUtils.GetGoapDataType(value)
            });
        }
        

        public IFinalGoapActionBuilder WithCondition(GoapCondition condition)
        {
            if (!GoapUtils.VerifyCondition(condition))
                throw new Exception("Incorrect format of condition!");

            this.conditions.Add(condition);
            return this;
        }

        public IFinalGoapActionBuilder WithEffect(string key, bool value)
        {
            this.effects.Add(new GoapEffect
            {
                Key = key,
                Value = value,
                GoapDataType = GoapDataType.Bool,
                EffectDirection = EffectDirection.Set
            });
            return this;
        }

        public IFinalGoapActionBuilder WithEffect(string key, EffectDirection direction, object value)
        {
            return this.WithEffect(new GoapEffect
            {
                Key = key,
                Value = value,
                GoapDataType = GoapUtils.GetGoapDataType(value),
                EffectDirection = direction
            });
        }

        public IFinalGoapActionBuilder WithEffect(GoapEffect effect)
        {
            if (!GoapUtils.VerifyEffect(effect))
                throw new Exception("Incorrect format of condition!");

            this.effects.Add(effect);
            return this;
        }

        public GoapAction Build()
        {
            GoapAction newAction = new GoapAction(this.name, this.actionStrategy, this.costFunc, this.conditions, this.effects);
            this.name = "";
            this.conditions = new HashSet<GoapCondition>();
            this.effects = new HashSet<GoapEffect>();
            this.actionStrategy = null;
            this.costFunc = null;
            return newAction;
        }
    }
}