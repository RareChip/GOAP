using System.Collections.Generic;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.API
{
    public interface IGoapPlanner
    {
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals, IWorldState worldState);
        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState);
    }
}