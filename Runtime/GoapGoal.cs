using System;
using System.Collections.Generic;
using GOAP.Runtime.Util;

namespace GOAP.Runtime
{
    public sealed class GoapGoal
    {
        public string GoalName { get; private set; }
        public HashSet<GoapCondition> Conditions { get; private set; }
        public Func<float> CalculateInsistence { get; private set; }

        public GoapGoal(string name, HashSet<GoapCondition> conditions, Func<float> calculateInsistence)
        {
            this.GoalName = name;
            this.Conditions = conditions;
            this.CalculateInsistence = calculateInsistence;
        }
        
        public class Builder
        {
            private string name;
            private HashSet<GoapCondition> conditions;
            private Func<float> calculateInsistence;

            public Builder(string name)
            {
                this.name = name;
                this.conditions = new HashSet<GoapCondition>();
                this.calculateInsistence = () => 1;
            }
            
            public Builder WithCondition(string key, ConditionDirection direction, object value)
            {
                return this.WithCondition(new GoapCondition
                {
                    Key = key,
                    ConditionDirection = direction,
                    Value = value,
                    GoapDataType = GoapUtils.GetGoapDataType(value)
                });
            }
            
            public Builder WithCondition(string key, bool value)
            {
                return this.WithCondition(new GoapCondition
                {
                    Key = key,
                    ConditionDirection = ConditionDirection.Equals,
                    Value = value,
                    GoapDataType = GoapDataType.Bool
                });
            }
            
            public Builder WithCondition(GoapCondition condition)
            {
                if (!GoapUtils.VerifyCondition(condition))
                    throw new Exception("Incorrect format of condition!");

                this.conditions.Add(condition);
                return this;
            }

            public Builder WithInsistence(Func<float> insistenceFunc)
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