using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;
using NUnit.Framework;

namespace GOAP.Tests
{
    public static class TestUtils
    {
        private const int MAX_PLAN_LENGTH = 20;
        public static IGoapPlanner GetPlanner()
        {
            return new GoapForwardPlanner(MAX_PLAN_LENGTH);
        }
        public static void AssertPlanMakesSense(IActionPlan plan, PlannerState worldState, GoapGoal goal)
        {
            Queue<GoapAction> planCopy = new Queue<GoapAction>(plan.Actions);
            while (planCopy.Count > 0)
            {
                GoapAction current = planCopy.Dequeue();
                foreach (GoapCondition currentCondition in current.Conditions)
                {
                    Assert.IsTrue(worldState.ConditionIsSatisfied(currentCondition));
                }

                foreach (GoapEffect effect in current.Effects)
                {
                    worldState.ApplyEffect(effect);
                }
            }

            foreach (GoapCondition currentCondition in goal.Conditions)
            {
                Assert.IsTrue(worldState.ConditionIsSatisfied(currentCondition));
            }
        }
    }
}