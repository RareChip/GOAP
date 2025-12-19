using System.Collections.Generic;

namespace GOAP.Runtime.Agent
{
    public abstract class GoapAgentConfiguration
    {
        public abstract WorldState CreateWorldState();
        public abstract HashSet<GoapGoal> CreateGoals();
        public abstract HashSet<GoapAction> CreateActions(IGoapActionBuilder builder);
    }
}