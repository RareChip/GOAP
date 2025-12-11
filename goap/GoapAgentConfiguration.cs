using System.Collections.Generic;

namespace GOAP.goap
{
    public abstract class GoapAgentConfiguration
    {
        public abstract HashSet<GoapGoal> GetGoals();
        public abstract HashSet<GoapAction> GetActions();
    }
}