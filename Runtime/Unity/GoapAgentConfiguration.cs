using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;
using UnityEngine;

namespace GOAP.Runtime.Unity
{
    public abstract class GoapAgentConfiguration : ScriptableObject
    {
        [SerializeField] private int maxPlanLength = 20;
        public abstract WorldState CreateWorldState();
        public abstract ISet<GoapGoal> CreateGoals(IGoapGoalBuilder builder);
        public abstract ISet<GoapAction> CreateActions(IGoapActionBuilder builder);
        public virtual IGoapPlanner CreatePlanner() => new GoapForwardPlanner(maxPlanLength);
    }
}