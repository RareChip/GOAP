using System;
using System.Collections.Generic;

namespace GOAP.Core.Planning
{
    public class PlannerNode
    {
        public HashSet<GoapCondition> Conditions { get; private set; }
        public PlannerEdge Edge { get; private set; }

        public PlannerNode(HashSet<GoapCondition> conditions, PlannerEdge edge)
        {
            Conditions = conditions;
            Edge = edge;
        }
    }
}