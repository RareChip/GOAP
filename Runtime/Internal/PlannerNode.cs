using System;
using System.Collections.Generic;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Internal
{
    public class PlannerNode
    {
        public HashSet<GoapCondition> Conditions { get; }
        public Dictionary<string, GoapCondition> ConditionMap { get; }
        public PlannerNode ParentNode { get; private set; }
        public int Cost { get; private set; }
        public GoapAction Action { get; private set; }

        public PlannerNode(HashSet<GoapCondition> conditions, PlannerNode parentNode, int cost, GoapAction action)
        {
            Conditions = conditions;
            ParentNode = parentNode;
            Cost = cost;
            Action = action;

            ConditionMap = new Dictionary<string, GoapCondition>();
            foreach (GoapCondition goapCondition in conditions)
            {
                ConditionMap.Add(goapCondition.Key, goapCondition);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not PlannerNode other)
                return false;

            return Conditions.SetEquals(other.Conditions);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (GoapCondition goapCondition in Conditions)
            {
                hash ^= goapCondition.GetHashCode();
            }

            return hash;
        }
        
        public int GetHashCode(IWorldState worldState)
        {
            int hash = 0;
            foreach (GoapCondition goapCondition in Conditions)
            {
                hash += goapCondition.GetHashCode(worldState);
            }

            return hash;
        }

        public bool IsJustAsGood(PlannerNode other, IWorldState worldState)
        {
            // if (Conditions.Count != other.Conditions.Count)
            //     return false;
            //
            // List<(GoapCondition, GoapCondition)> differingConditions = new List<(GoapCondition, GoapCondition)>();
            //
            // foreach (GoapCondition otherCond in other.Conditions)
            // {
            //     if (!ConditionMap.TryGetValue(otherCond.Key, out GoapCondition myCond))
            //         return false;
            //
            //     if (myCond.ConditionDirection != otherCond.ConditionDirection)
            //         return false;
            //
            //     if (myCond.Value != otherCond.Value)
            //     {
            //         differingConditions.Add((myCond,otherCond));
            //     }
            //
            // }
            //
            // return differingConditions.TrueForAll(x => 
            //     GoapResolver.ConditionIsSatisfied(x.Item1, worldState)
            //     && GoapResolver.ConditionIsSatisfied(x.Item2, worldState));
            return GetHashCode(worldState) == other.GetHashCode(worldState);
        }
        
    }
}