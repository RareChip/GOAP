namespace GOAP.Core.Planning
{
    public class PlannerEdge
    {
        public PlannerNode ParentNode { get; private set; }
        public int Cost { get; private set; }
        public GoapAction Action { get; private set; }

        public PlannerEdge(PlannerNode parent, int cost, GoapAction action)
        {
            this.ParentNode = parent;
            this.Cost = cost;
            this.Action = action;
        }
    }
}