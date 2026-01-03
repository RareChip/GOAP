using System.Collections.Generic;
using GOAP.Runtime.Internal;

namespace GOAP.Runtime
{
    public interface IGoapPlanner
    {
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals, IWorldState worldState);
        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState);
    }
}