using System.Collections.Generic;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.Unity
{
    public abstract class GoapAgentConfiguration
    {
        public abstract WorldState CreateWorldState();
        public abstract HashSet<GoapGoal> CreateGoals();
        public abstract HashSet<GoapAction> CreateActions(IGoapActionBuilder builder);
    }
}