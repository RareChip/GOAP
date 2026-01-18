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
        public abstract WorldState CreateWorldState(GoapAgent agent);
        public abstract ISet<GoapGoal> CreateGoals(GoapAgent agent, IGoapGoalBuilder builder);
        public abstract ISet<GoapAction> CreateActions(GoapAgent agent, IGoapActionBuilder builder);
        public virtual IGoapPlanner CreatePlanner() => new GoapForwardPlanner(maxPlanLength);
    }
}