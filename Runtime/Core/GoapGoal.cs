using System;
using System.Collections.Generic;
using GOAP.Runtime.API;

namespace GOAP.Runtime.Core
{
    public sealed class GoapGoal
    {
        public string GoalName { get; }
        public HashSet<GoapCondition> Conditions { get; }
        public Func<IWorldState, float> CalculateInsistence { get; }

        public GoapGoal(string name, HashSet<GoapCondition> conditions, Func<IWorldState, float> calculateInsistence)
        {
            this.GoalName = name;
            this.Conditions = conditions;
            this.CalculateInsistence = calculateInsistence;
        }

        public override bool Equals(object obj)
        {
            if (obj is not GoapGoal other)
                return false;

            return GoalName.Equals(other.GoalName) 
                   && CalculateInsistence.Equals(other.CalculateInsistence)
                                                   && Conditions.Count == other.Conditions.Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GoalName, CalculateInsistence, Conditions.Count);
        }
    }
}