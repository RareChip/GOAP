using System;
using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Core
{
    public sealed class GoapGoal
    {
        public string GoalName { get; private set; }
        public HashSet<GoapCondition> Conditions { get; private set; }
        public Func<IWorldState, float> CalculateInsistence { get; private set; }

        public GoapGoal(string name, HashSet<GoapCondition> conditions, Func<IWorldState, float> calculateInsistence)
        {
            this.GoalName = name;
            this.Conditions = conditions;
            this.CalculateInsistence = calculateInsistence;
        }
        
        public class Builder
        {
            private readonly string name;
            private readonly HashSet<GoapCondition> conditions = new();
            private Func<IWorldState, float> calculateInsistence;

            public Builder(string name)
            {
                this.name = name;
                this.calculateInsistence = _ => 0.5f;
            }
            
            public Builder WithCondition(string key, ConditionDirection direction, object value)
            {
                return this.WithCondition(new GoapCondition(GoapUtils.GetGoapDataType(value), key, value, direction));
            }
            
            public Builder WithCondition(string key, bool value)
            {
                return this.WithCondition(new GoapCondition(GoapDataType.Bool, key, value, ConditionDirection.Equals));
            }
            
            public Builder WithCondition(GoapCondition condition)
            {
                if (!GoapUtils.VerifyCondition(condition))
                    throw new Exception("Incorrect format of condition!");

                this.conditions.Add(condition);
                return this;
            }

            public Builder WithInsistence(Func<IWorldState,float> insistenceFunc)
            {
                this.calculateInsistence = insistenceFunc;
                return this;
            }

            public GoapGoal Build()
            {
                return new GoapGoal(this.name, this.conditions, this.calculateInsistence);
            }
        }
    }
}