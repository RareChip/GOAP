using System;

namespace GOAP.goap
{
    public struct GoapEffect
    {
        public string Key;
        public Func<object, object> Effect;
    }
}