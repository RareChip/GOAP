using GOAP.Runtime.Core;

namespace GOAP.Runtime.API
{
    public interface IActionPlan
    {
        public GoapGoal Goal { get; }
        public int TotalCost { get; }
        public bool IsComplete { get; }
        public GoapAction[] Actions { get; }
        public GoapAction PopAction();
    }
}