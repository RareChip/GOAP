using System;

namespace GOAP.goap
{
    public struct GoapCondition
    {
        public string Key;
        public Func<object,object,bool> Condition;
    }
}