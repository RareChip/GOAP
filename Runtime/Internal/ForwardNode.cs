using GOAP.Runtime.API;
using GOAP.Runtime.Core;

namespace GOAP.Runtime.Internal
{
    public class ForwardNode
    {
        public IWorldState WorldState { get; private set; }
        public ForwardNode ParentNode { get; private set; }
        public int Cost { get; private set; }
        public GoapAction Action { get; private set; }
        public int CurrentPlanLength { get; private set; }

        public ForwardNode(IWorldState worldState, ForwardNode parentNode, int cost, GoapAction action)
        {
            this.WorldState = worldState;
            this.ParentNode = parentNode;
            this.Cost = cost;
            this.Action = action;
            this.CurrentPlanLength = parentNode == null ? 0 : parentNode.CurrentPlanLength + 1;
        }
    }
}