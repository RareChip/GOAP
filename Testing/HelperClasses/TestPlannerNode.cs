using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.Internal;

namespace GOAP.Testing.HelperClasses
{
    public class TestPlannerNode : PlannerNode
    {
        public TestPlannerNode(HashSet<GoapCondition> conditions) : base(conditions, null, 1, null)
        {
        }
    }
}