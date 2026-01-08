using System.Collections.Generic;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.API
{
    public interface IGoapPlanner
    {
        public GoapGoal GenerateBestGoal(ISet<GoapGoal> goals, IWorldState worldState);
        public IActionPlan GeneratePlan(ISet<GoapAction> actions, GoapGoal goal, IWorldState worldState);
    }
}