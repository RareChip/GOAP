using System;
using System.Collections.Generic;

namespace GOAP.goap
{
    public sealed class GoapGoal
    {
        private string goalName;
        private HashSet<GoapCondition> preconditions;
        private Func<float> calculateInsistence;
    }
}