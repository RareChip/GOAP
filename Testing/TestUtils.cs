using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.Internal;
using GOAP.Runtime.Util;
using GOAP.Testing.ActionStrategies;
using NUnit.Framework;

namespace GOAP.Testing
{
    public class TestUtils
    {
        public static HashSet<GoapAction> AddAllActions(params GoapAction[] actions)
        {
            HashSet<GoapAction> actionsSet = new HashSet<GoapAction>();
            foreach (GoapAction goapAction in actions)
            {
                actionsSet.Add(goapAction);
            }

            return actionsSet;
        }
        public static void AssertPlanMakesSense(ActionPlan plan, PlannerState worldState, GoapGoal goal)
        {
            while (plan.Actions.Count > 0)
            {
                GoapAction current = plan.Actions.Dequeue();
                foreach (GoapCondition currentCondition in current.Conditions)
                {
                    Assert.IsTrue(GoapResolver.ConditionIsSatisfied(currentCondition, worldState));
                }
                GoapUtils.ApplyEffectsToPlannerState(current.Effects, worldState);
            }

            foreach (GoapCondition currentCondition in goal.Conditions)
            {
                Assert.IsTrue(GoapResolver.ConditionIsSatisfied(currentCondition, worldState));
            }
        }
    }
}