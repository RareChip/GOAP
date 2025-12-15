using System.Collections.Generic;

namespace GOAP.Core.Agent
{
    public abstract class GoapAgentConfiguration
    {
        public abstract HashSet<GoapGoal> CreateGoals();
        public abstract HashSet<GoapAction> CreateActions();
        public abstract WorldState CreateWorldState();
    }
}