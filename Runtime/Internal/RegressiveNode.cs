using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Util;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public class RegressiveNode
    {
        public HashSet<GoapCondition> Conditions { get; }
        public Dictionary<string, GoapCondition> ConditionMap { get; }
        public RegressiveNode ParentNode { get; private set; }
        public int Cost { get; private set; }
        public GoapAction Action { get; private set; }
        public Dictionary<string, GoapCondition> CachedState { get; private set; }

        public RegressiveNode(HashSet<GoapCondition> conditions, RegressiveNode parentNode, int cost, GoapAction action)
        {
            this.Conditions = conditions;
            this.ParentNode = parentNode;
            this.Cost = cost;
            this.Action = action;

            this.ConditionMap = new Dictionary<string, GoapCondition>();
            foreach (GoapCondition goapCondition in conditions)
            {
                this.ConditionMap.Add(goapCondition.Key, goapCondition);
            }

            this.CachedState = new Dictionary<string, GoapCondition>();
        }

        public override bool Equals(object obj)
        {
            if (obj is not RegressiveNode other)
                return false;

            return this.Conditions.SetEquals(other.Conditions);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (GoapCondition goapCondition in this.Conditions)
            {
                hash ^= goapCondition.GetHashCode();
            }

            return hash;
        }
        
        public int GetHashCode(IWorldState worldState)
        {
            int hash = 0;
            foreach (GoapCondition goapCondition in this.Conditions)
            {
                hash += goapCondition.GetHashCode(worldState);
            }

            return hash;
        }

        // The issue is that some actions have more than one effect, meaning something being the "only"
        // difference isnt good enough.
        public bool IsJustAsGood(RegressiveNode other, IWorldState worldState)
        {
            if (this.Conditions.Count > other.Conditions.Count)
                return false;
            
            foreach (GoapCondition otherCond in other.Conditions)
            {
                if (!this.ConditionMap.TryGetValue(otherCond.Key, out GoapCondition myCond))
                    return false;
            
                if (myCond.ConditionDirection != otherCond.ConditionDirection)
                    return false;
            
                if (!myCond.Value.Equals(otherCond.Value))
                {
                    if (!this.IsConditionEasier(myCond, otherCond))
                        return false;
                }
            
            }
            
            return true;
            
            return this.GetHashCode(worldState) == other.GetHashCode(worldState);
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