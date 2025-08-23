using RareChip.AITools;
using UnityEditor;

namespace RareChip.Goap
{
    public class DoNothingGoal : BaseGoal
    {
    }

    public class DoNothingAction :  IActionStrategy
    {
        public bool IsComplete => false;
        public bool CanPerform => true;

        public void EnterAction(IContext context)
        {
            context.AnimationHandler.PlayLoopingAnimation("Idle");
        }
    }
}