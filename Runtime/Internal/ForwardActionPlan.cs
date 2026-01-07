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
        private readonly Stack<ForwardNode> nodes;
        
        public ForwardActionPlan(GoapGoal goal, Stack<ForwardNode> nodes, int totalCost)
        {
            this.Goal = goal;
            this.TotalCost = totalCost;
            this.Actions = new GoapAction[nodes.Count];
            this.nodes = nodes;
            ForwardNode[] nodeArray = nodes.ToArray();
            for (int i = 0; i < nodes.Count; i++)
            {
                Actions[i] = nodeArray[i].Action;
            }
        }

        public GoapAction PopAction()
        {
            if (!nodes.TryPop(out ForwardNode node)) 
                return null;
            
            TotalCost -= node.Cost;
            return node.Action;

        }
    }
}