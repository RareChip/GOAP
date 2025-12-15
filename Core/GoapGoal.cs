using System;
using System.Collections.Generic;

namespace GOAP.Core
{
    public sealed class GoapGoal
    {
        private string goalName;
        public HashSet<GoapCondition> Conditions;
        private Func<float> calculateInsistence;
    }
}