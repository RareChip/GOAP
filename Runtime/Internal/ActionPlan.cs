using System.Collections.Generic;

namespace GOAP.Runtime.Internal
{
    public class ActionPlan
    {
        public GoapGoal Goal { get; }
        public Stack<GoapAction> Actions { get; }
        public int TotalCost { get; }

        public ActionPlan(GoapGoal goal, Stack<GoapAction> actions, int totalCost)
        {
            this.Goal = goal;
            this.Actions = actions;
            this.TotalCost = totalCost;
        }
    }
}