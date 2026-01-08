using System;
using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Core
{
    public interface IGoapGoalBuilder
    {
        IGoapGoalInsistenceBuilder CreateGoal(string name);
    }

    public interface IGoapGoalInsistenceBuilder
    {
        IGoapGoalConditionBuilder WithInsistence(Func<IWorldState, float> insistenceFunc);
    }

    public interface IGoapGoalConditionBuilder
    {
        IFinalGoapGoalBuilder WithCondition(string key, bool value);
        IFinalGoapGoalBuilder WithCondition(string key, ConditionDirection direction, object value);
        IFinalGoapGoalBuilder WithCondition(GoapCondition condition);
    }

    public interface IFinalGoapGoalBuilder
    {
        IFinalGoapGoalBuilder WithCondition(string key, bool value);
        IFinalGoapGoalBuilder WithCondition(string key, ConditionDirection direction, object value);
        IFinalGoapGoalBuilder WithCondition(GoapCondition condition);
        GoapGoal Build();
    }

    public class GoapGoalBuilder : IGoapGoalBuilder, IGoapGoalInsistenceBuilder, IGoapGoalConditionBuilder,
        IFinalGoapGoalBuilder
    {
        private string name;
        private HashSet<GoapCondition> conditions = new();
        private Func<IWorldState, float> calculateInsistence;

        public GoapGoalBuilder()
        {
            this.name = "";
            this.calculateInsistence = _ => 0.5f;
        }
        
        public IGoapGoalInsistenceBuilder CreateGoal(string name)
        {
            this.name = name;
            return this;
        }

        public IFinalGoapGoalBuilder WithCondition(string key, ConditionDirection direction, object value)
        {
            return this.WithCondition(new GoapCondition(GoapUtils.GetGoapDataType(value), key, value, direction));
        }

        public IFinalGoapGoalBuilder WithCondition(string key, bool value)
        {
            return this.WithCondition(new GoapCondition(GoapDataType.Bool, key, value, ConditionDirection.Equals));
        }

        public IFinalGoapGoalBuilder WithCondition(GoapCondition condition)
        {
            if (!GoapUtils.VerifyCondition(condition))
                throw new Exception("Incorrect format of condition!");

            this.conditions.Add(condition);
            return this;
        }

        public IGoapGoalConditionBuilder WithInsistence(Func<IWorldState, float> insistenceFunc)
        {
            this.calculateInsistence = insistenceFunc;
            return this;
        }

        public GoapGoal Build()
        {
            GoapGoal newGoal = new GoapGoal(this.name, this.conditions, this.calculateInsistence);
            name = "";
            calculateInsistence = _ => 0.5f;
            conditions = new HashSet<GoapCondition>();
            return newGoal;
        }
    }
}