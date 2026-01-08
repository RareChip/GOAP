using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.Internal
{
    public class ForwardActionPlan : IActionPlan
    {
        public GoapGoal Goal { get; }
        public GoapAction[] Actions { get; }
        public int TotalCost { get; private set; }
        public bool IsComplete => TotalCost <= 0;
        private readonly Stack<GoapAction> actionStack;
        private IWorldState worldState;
        
        public ForwardActionPlan(GoapGoal goal, Stack<GoapAction> actionStack, int totalCost, IWorldState worldState)
        {
            this.Goal = goal;
            this.TotalCost = totalCost;
            this.Actions = actionStack.ToArray();
            this.actionStack = actionStack;
        }

        public GoapAction PopAction()
        {
            if (!actionStack.TryPop(out GoapAction action)) 
                return null;
            
            TotalCost -= action.CalculateCost(worldState);
            return action;

        }
    }
}