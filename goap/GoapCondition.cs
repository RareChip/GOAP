using System;

namespace GOAP.goap
{
    public struct GoapCondition
    {
        public string Key;
        public Predicate<object> Condition;
    }
}