using System;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Core
{
    public class GoapEffect
    {
        public GoapDataType GoapDataType { get; }
        public string Key { get; }
        public object Value { get; }
        public EffectDirection EffectDirection { get; }

        public GoapEffect(GoapDataType goapDataType, string key, object value, EffectDirection effectDirection)
        {
            GoapDataType = goapDataType;
            Key = key;
            Value = value;
            EffectDirection = effectDirection;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is not GoapEffect effect)
                return false;

            return GoapDataType == effect.GoapDataType
                   && Key == effect.Key && Value.Equals(effect.Value) && EffectDirection == effect.EffectDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GoapDataType, Key, Value, EffectDirection);
        }
    }
    public enum EffectDirection
    {
        Set,
        Increase,
        Decrease
    }
}