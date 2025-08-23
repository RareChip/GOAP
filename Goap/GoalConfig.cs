using MyBox;
using RareChip.Serialization;
using UnityEngine;

namespace RareChip.Goap
{
    [CreateAssetMenu(fileName = "Goal Config", menuName = "Scriptable Objects/GOAP/Goal Config")]
    public class GoalConfig : ScriptableObject
    {
        public string Name;
        public GoapCondition Precondition;
        public bool StaticInsistance = true;

        [SerializeField, ConditionalField("StaticInsistance")]
        public float Insistance = 0f;

        [SerializeField, ConditionalField("StaticInsistance", true)] [TypeFilter(typeof(IGoal))]
        public SerializableType goalStrategy;

        public IGoal GetGoal(GoapAgent agent)
        {
            IGoal goal = goalStrategy.CreateInstance<IGoal>();

            if (goal == null)
                return null;

            goal.Initialize(this, agent);
            return goal;
        }
    }
}