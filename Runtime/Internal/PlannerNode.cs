using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Util;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public class PlannerNode
    {
        public HashSet<GoapCondition> Conditions { get; }
        public Dictionary<string, GoapCondition> ConditionMap { get; }
        public PlannerNode ParentNode { get; private set; }
        public int Cost { get; private set; }
        public GoapAction Action { get; private set; }
        public Dictionary<string, GoapCondition> CachedState { get; private set; }

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

            CachedState = new Dictionary<string, GoapCondition>();
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

        // The issue is that some actions have more than one effect, meaning something being the "only"
        // difference isnt good enough.
        public bool IsJustAsGood(PlannerNode other, IWorldState worldState)
        {
            if (this.Conditions.Count > other.Conditions.Count)
                return false;
            
            foreach (GoapCondition otherCond in other.Conditions)
            {
                if (!ConditionMap.TryGetValue(otherCond.Key, out GoapCondition myCond))
                    return false;
            
                if (myCond.ConditionDirection != otherCond.ConditionDirection)
                    return false;
            
                if (!myCond.Value.Equals(otherCond.Value))
                {
                    if (!IsConditionEasier(myCond, otherCond))
                        return false;
                }
            
            }
            
            return true;
            
            return GetHashCode(worldState) == other.GetHashCode(worldState);
        }
        
        private bool IsConditionEasier(GoapCondition myCond, GoapCondition otherCond)
        {
            switch (myCond.ConditionDirection)
            {
                case ConditionDirection.GreaterThanEq:
                    if (myCond.GoapDataType == GoapDataType.Int)
                        return (int)myCond.Value <= (int)otherCond.Value;
                    if (myCond.GoapDataType == GoapDataType.Float)
                        return (float)myCond.Value <= (float)otherCond.Value;
                    break;

                case ConditionDirection.LessThanEq:
                    if (myCond.GoapDataType == GoapDataType.Int)
                        return (int)myCond.Value >= (int)otherCond.Value;
                    if (myCond.GoapDataType == GoapDataType.Float)
                        return (float)myCond.Value >= (float)otherCond.Value;
                    break;

                case ConditionDirection.Equals:
                case ConditionDirection.NotEquals:
                    return myCond.Value.Equals(otherCond.Value);
            }
            return false;
        }

        public void RemoveAndCacheCondition(GoapCondition condition)
        {
            
        }
    }
}