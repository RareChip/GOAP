using System.Collections.Generic;

namespace GOAP.Runtime.Internal
{
    public class ForwardNode
    {
        public IWorldState WorldState { get; }
        public ForwardNode ParentNode { get; }
        public int Cost { get; }
        public GoapAction Action { get; }

        public ForwardNode(IWorldState worldState, ForwardNode parentNode, int cost, GoapAction action)
        {
            this.WorldState = worldState;
            this.ParentNode = parentNode;
            this.Cost = cost;
            this.Action = action;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ForwardNode other)
                return false;

            return this.WorldState.Equals(other.WorldState);
        }

        public override int GetHashCode()
        {
            return this.WorldState.GetHashCode();
        }
    }
}