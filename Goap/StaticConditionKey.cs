using UnityEngine;

namespace RareChip.Goap
{
    [CreateAssetMenu(menuName = "Scriptable Objects/GOAP/Static Condition Key", fileName = "StaticConditionKey")]
    public class StaticConditionKey : ScriptableObject
    {
        public string Key => name;
    }
}