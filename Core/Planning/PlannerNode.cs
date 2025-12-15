using System;

namespace GOAP.Core.Planning
{
    public class PlannerNode
    {
        public GoapCondition[] Conditions { get; private set; }
        public PlannerEdge Edge { get; private set; }

        public PlannerNode(GoapCondition[] conditions, PlannerEdge edge)
        {
            Conditions = conditions;
            Edge = edge;
        }
    }
}