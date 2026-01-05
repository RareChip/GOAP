using GOAP.Runtime.Util;

namespace GOAP.Runtime.Core
{
    public class GoapEffect
    {
        public GoapDataType GoapDataType;
        public string Key;
        public object Value;
        public EffectDirection EffectDirection;
    }
    public enum EffectDirection
    {
        Set,
        Increase,
        Decrease
    }
}