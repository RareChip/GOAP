using System;
using GOAP.Core.Planning;
using GOAP.Util;
using UnityEngine;

namespace GOAP.Core
{
    public class GoapCondition
    {
        public GoapDataType GoapDataType { get; private set; }
        public string Key { get; private set; }
        public object Value { get; private set; }
        public ConditionState ConditionState { get; private set; }
    }
    
    public enum ConditionState
    {
        Equals,
        NotEquals,
        LessThan,
        GreaterThan,
    }
}