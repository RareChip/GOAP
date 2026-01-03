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
            return this.GoapDataType == other.GoapDataType && this.Key == other.Key && Equals(this.Value, other.Value) && this.EffectDirection == other.EffectDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)this.GoapDataType, this.Key, this.Value, (int)this.EffectDirection);
        }
    }

    public enum EffectDirection
    {
        Set,
        Increase,
        Decrease
    }
}