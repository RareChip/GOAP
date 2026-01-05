using GOAP.Runtime.Util;

namespace GOAP.Runtime.Core
{
    public class GoapCondition
    {
        public GoapDataType GoapDataType { get; private set; }
        public string Key { get; private set; }
        public object Value { get; private set; }
        public ConditionDirection ConditionDirection { get; private set; }

        public GoapCondition(GoapDataType goapDataType, string key, object value, ConditionDirection conditionDirection)
        {
            this.GoapDataType = goapDataType;
            this.Key = key;
            this.Value = value;
            this.ConditionDirection = conditionDirection;
        }
    }
    
    public enum ConditionDirection
    {
        Equals,
        NotEquals,
        LessThanEq,
        GreaterThanEq,
    }
}