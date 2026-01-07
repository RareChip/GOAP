using System.Collections.Generic;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;

namespace GOAP.Runtime.API
{
    public interface IGoapPlanner
    {
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals, IWorldState worldState);
        public IActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState);
    }
}