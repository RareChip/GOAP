using System;
using GOAP.Runtime.Util;

namespace GOAP.Runtime
{
    public struct GoapEffect : IEquatable<GoapEffect>
    {
        public GoapDataType GoapDataType;
        public string Key;
        public object Value;
        public EffectDirection EffectDirection;

        public bool Equals(GoapEffect other)
        {
            return GoapDataType == other.GoapDataType && Key == other.Key && Equals(Value, other.Value) && EffectDirection == other.EffectDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)GoapDataType, Key, Value, (int)EffectDirection);
        }
    }

    public enum EffectDirection
    {
        Set,
        Increase,
        Decrease
    }
}