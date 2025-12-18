using System.Collections.Generic;

namespace GOAP.Core.Agent
{
    public abstract class GoapAgentConfiguration
    {
        public abstract WorldState CreateWorldState();
        public abstract HashSet<GoapGoal> CreateGoals();
        public abstract HashSet<GoapAction> CreateActions(IGoapActionBuilder builder);
    }
}