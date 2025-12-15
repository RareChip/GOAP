using System;
using GOAP.Core.Planning;
using GOAP.Util;

namespace GOAP.Core
{
    public class GoapEffect
    {
        public GoapDataType GoapDataType { get; private set; }
        public string Key { get; private set; }
        public object Value { get; private set; }
        public EffectState EffectDirection { get; private set; }
    }

    public enum EffectState
    {
        Set,
        Increase,
        Decrease
    }
}