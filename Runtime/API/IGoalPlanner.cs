using System.Collections.Generic;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.API
{
    public interface IGoalPlanner
    {
        public GoapGoal GenerateBestGoal(ISet<GoapGoal> goals, IWorldState worldState);
    }
}