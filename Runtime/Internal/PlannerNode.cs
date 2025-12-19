using System.Collections.Generic;

namespace GOAP.Runtime.Internal
{
    public class PlannerNode
    {
        public HashSet<GoapCondition> Conditions { get; private set; }
        public PlannerNode ParentNode { get; private set; }
        public int Cost { get; private set; }
        public GoapAction Action { get; private set; }

        public PlannerNode(HashSet<GoapCondition> conditions, PlannerNode parentNode, int cost, GoapAction action)
        {
            Conditions = conditions;
            ParentNode = parentNode;
            Cost = cost;
            Action = action;
        }
    }
}