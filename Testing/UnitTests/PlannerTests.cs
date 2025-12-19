using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.Internal;
using GOAP.Runtime.Util;
using GOAP.Testing.ActionStrategies;
using NUnit.Framework;

namespace GOAP.Testing.UnitTests
{
    [TestFixture]
    public class PlannerTests
    {
        [Test]
        public void TestGeneratePlanNullInputs()
        {
            IGoapPlanner planner = new GoapPlanner();

            ActionPlan plan = planner.GeneratePlan(null, null, null);
            Assert.Null(plan);
            
            plan = planner.GeneratePlan(new HashSet<GoapAction>(), null, null);
            Assert.Null(plan);
            
            plan = planner.GeneratePlan(null, new GoapGoal("",null,null), null);
            Assert.Null(plan);
            
            plan = planner.GeneratePlan(null, null, new PlannerState(null));
            Assert.Null(plan);
        }

        [Test]
        public void TestSimplePlan()
        {
            IGoapPlanner planner = new GoapPlanner();
            HashSet<GoapAction> actions = new HashSet<GoapAction>();
            IGoapActionBuilder builder = new GoapActionBuilder();
            GoapGoal goal = new GoapGoal.Builder("BuildWallsGoal")
                .WithCondition("WallsBuilt", ConditionDirection.GreaterThanEq, 4)
                .Build();

            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("WallsBuilt", 0);
            worldData.Add("WoodCount",20);

            IWorldState worldState = new PlannerState(worldData);

            actions.Add(builder.CreateAction("BuildWall")
                .WithStrategy(new NoOpStrategy())
                .WithCost(x => 1)
                .WithCondition(new GoapCondition
                {
                    Key = "WoodCount",
                    ConditionDirection = ConditionDirection.GreaterThanEq,
                    Value = 20,
                    GoapDataType = GoapDataType.Int
                })
                .WithEffect(new GoapEffect
                {
                    Key = "WallsBuilt",
                    EffectDirection = EffectDirection.Increase,
                    Value = 1,
                    GoapDataType = GoapDataType.Int
                })
                .Build());

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            Assert.AreEqual(7, plan.TotalCost);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual("BuildWallsGoal", plan.Goal.GoalName);
            foreach (GoapAction action in plan.Actions)
            {
                Assert.AreEqual("BuildWall", action.ActionName);
            }
        }

        [Test]
        public void TestComplexPlan()
        {
            IGoapPlanner planner = new GoapPlanner();
            HashSet<GoapAction> actions = new HashSet<GoapAction>();
            IGoapActionBuilder builder = new GoapActionBuilder();
            GoapGoal goal = new GoapGoal.Builder("BuildWallsGoal")
                .WithCondition("WallsBuilt", ConditionDirection.GreaterThanEq, 4)
                .Build();

            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("WallsBuilt", 0);
            worldData.Add("WoodCount",0);
            worldData.Add("WoodInWorld", 0);
            worldData.Add("HandsFree", true);
            worldData.Add("HasAxe", false);

            IWorldState worldState = new PlannerState(worldData);

            actions.Add(builder.CreateAction("BuildWall")
                .WithStrategy(new NoOpStrategy())
                .WithCost(x => 1)
                .WithCondition(new GoapCondition
                {
                    Key = "WoodCount",
                    ConditionDirection = ConditionDirection.GreaterThanEq,
                    Value = 20,
                    GoapDataType = GoapDataType.Int
                })
                .WithCondition("HandsFree", ConditionDirection.Equals, true)
                .WithEffect(new GoapEffect
                {
                    Key = "WallsBuilt",
                    EffectDirection = EffectDirection.Increase,
                    Value = 1,
                    GoapDataType = GoapDataType.Int
                })
                .Build());

            actions.Add(builder.CreateAction("GatherWood")
                .WithStrategy(new NoOpStrategy())
                .WithCost(x => 1)
                .WithCondition("WoodInWorld", ConditionDirection.GreaterThanEq, 1)
                .WithEffect("WoodCount", EffectDirection.Increase, 5)
                .WithEffect("WoodInWorld", EffectDirection.Decrease, 5)
                .Build());

            actions.Add(builder.CreateAction("ChopTree")
                .WithStrategy(new NoOpStrategy())
                .WithCost(x => 1)
                .WithCondition("HasAxe", true)
                .WithEffect("WoodInWorld", EffectDirection.Increase, 5)
                .Build());

            actions.Add(builder.CreateAction("PickupAxe")
                .WithStrategy(new NoOpStrategy())
                .WithCost(x => 2)
                .WithCondition("HandsFree", true)
                .WithEffect("HasAxe", true)
                .WithEffect("HandsFree", false)
                .Build());

            actions.Add(builder.CreateAction("DropItem")
                .WithStrategy(new NoOpStrategy())
                .WithCost(x => 1)
                .WithCondition("HandsFree",false)
                .WithEffect("HandsFree", true)
                .WithEffect("HasAxe",false)
                .Build());

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            Assert.AreEqual(7, plan.TotalCost);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual("BuildWallsGoal", plan.Goal.GoalName);
            foreach (GoapAction action in plan.Actions)
            {
                Assert.AreEqual("BuildWall", action.ActionName);
            }
        }
        
        // Test case for actions where multiple conditions should merge (e.g Health > 30 + Health > 50 = Health > 50)
    }
}