using System.Collections.Generic;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.API
{
    public interface IActionPlanner
    {
        public IActionPlan GeneratePlan(ISet<GoapAction> actions, GoapGoal goal, IWorldState worldState);
    }
}