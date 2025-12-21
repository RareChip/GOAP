using System.Collections.Generic;

namespace GOAP.Runtime.Internal
{
    public class ActionPlan
    {
        public GoapGoal Goal { get; }
        public Queue<GoapAction> Actions { get; }
        public int TotalCost { get; }

        public ActionPlan(GoapGoal goal, Queue<GoapAction> actions, int totalCost)
        {
            Goal = goal;
            Actions = actions;
            TotalCost = totalCost;
        }
        
        
    }
}