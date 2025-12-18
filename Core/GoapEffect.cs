using System;
using GOAP.Core.Planning;
using GOAP.Util;

namespace GOAP.Core
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