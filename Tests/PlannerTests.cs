using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Internal;
using GOAP.Runtime.Util;
using NUnit.Framework;

namespace GOAP.Tests
{
    [TestFixture]
    public class PlannerTests
    {
        [Test]
        public void TestGeneratePlanNullInputs()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();

            ActionPlan plan = planner.GeneratePlan(null, null, null);
            Assert.Null(plan);

            plan = planner.GeneratePlan(new HashSet<GoapAction>(), null, null);
            Assert.Null(plan);

            plan = planner.GeneratePlan(null, new GoapGoal("", null, null), null);
            Assert.Null(plan);

            plan = planner.GeneratePlan(null, null, new PlannerState((Dictionary<string, object>)null));
            Assert.Null(plan);
        }

        [Test]
        public void TestSimplePlan()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            HashSet<GoapAction> actions = new HashSet<GoapAction>();
            IGoapActionBuilder builder = new GoapActionBuilder();
            GoapGoal goal = new GoapGoal.Builder("BuildWallsGoal")
                .WithCondition("WallsBuilt", ConditionDirection.GreaterThanEq, 3)
                .Build();

            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("WallsBuilt", 0);

            IWorldState worldState = new PlannerState(worldData);

            actions.Add(builder.CreateAction("BuildWall")
                .WithStrategy(null)
                .WithCost(_ => 1)
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
            Assert.AreEqual(3, plan.TotalCost);
            Assert.AreEqual(3, plan.Actions.Count);
            Assert.AreEqual("BuildWallsGoal", plan.Goal.GoalName);
            foreach (GoapAction action in plan.Actions)
            {
                Assert.AreEqual("BuildWall", action.ActionName);
            }
        }
        
        [Test]
        public void TestSimplePlanTwoConditions()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            HashSet<GoapAction> actions = new HashSet<GoapAction>();
            IGoapActionBuilder builder = new GoapActionBuilder();
            GoapGoal goal = new GoapGoal.Builder("BuildWallsGoal")
                .WithCondition("WallsBuilt", ConditionDirection.GreaterThanEq, 3)
                .WithCondition("Prepared", true)
                .Build();

            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("WallsBuilt", 0);
            worldData.Add("Prepared", false);

            PlannerState worldState = new PlannerState(worldData);
        
            actions.Add(builder.CreateAction("BuildWall")
                .WithStrategy(null)
                .WithCost(_ => 1)
                .WithEffect("WallsBuilt",EffectDirection.Increase,1)
                .Build());
            
            actions.Add(builder.CreateAction("Prepare")
                .WithStrategy(null)
                .WithCost(_ => 1)
                .WithEffect("Prepared",true)
                .Build());

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            Assert.AreEqual(4, plan.TotalCost);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual("BuildWallsGoal", plan.Goal.GoalName);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
        }

        [Test]
        public void TestComplexPlan()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            HashSet<GoapAction> actions = new HashSet<GoapAction>();
            IGoapActionBuilder builder = new GoapActionBuilder();
            GoapGoal goal = new GoapGoal.Builder("BuildWallsGoal")
                .WithCondition("WallsBuilt", ConditionDirection.GreaterThanEq, 4)
                .Build();

            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("WallsBuilt", 0);
            worldData.Add("WoodCount", 0);
            worldData.Add("WoodInWorld", 0);
            worldData.Add("HandsFree", true);
            worldData.Add("HasAxe", false);

            PlannerState worldState = new PlannerState(worldData);

            actions.Add(builder.CreateAction("BuildWall")
                .WithStrategy(null)
                .WithCost(_ => 1)
                .WithCondition("WoodCount", ConditionDirection.GreaterThanEq, 5)
                .WithCondition("HandsFree", ConditionDirection.Equals, true)
                .WithEffect("WallsBuilt", EffectDirection.Increase, 1)
                .WithEffect("WoodCount", EffectDirection.Decrease, 5)
                .Build());

            actions.Add(builder.CreateAction("GatherWood")
                .WithStrategy(null)
                .WithCost(_ => 1)
                .WithCondition("WoodInWorld", ConditionDirection.GreaterThanEq, 5)
                .WithEffect("WoodCount", EffectDirection.Increase, 5)
                .WithEffect("WoodInWorld", EffectDirection.Decrease, 5)
                .Build());

            actions.Add(builder.CreateAction("ChopTree")
                .WithStrategy(null)
                .WithCost(_ => 1)
                .WithCondition("HasAxe", true)
                .WithEffect("WoodInWorld", EffectDirection.Increase, 5)
                .Build());

            actions.Add(builder.CreateAction("PickupAxe")
                .WithStrategy(null)
                .WithCost(_ => 2)
                .WithCondition("HandsFree", true)
                .WithEffect("HasAxe", true)
                .WithEffect("HandsFree", false)
                .Build());

            actions.Add(builder.CreateAction("DropItem")
                .WithStrategy(null)
                .WithCost(_ => 1)
                .WithCondition("HandsFree", false)
                .WithEffect("HandsFree", true)
                .WithEffect("HasAxe", false)
                .Build());

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(14, plan.Actions.Count);
            Assert.AreEqual(15, plan.TotalCost);
            Assert.AreEqual("BuildWallsGoal", plan.Goal.GoalName);

        }

        [Test]
        public void TestPlanLongRegression()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("PickaxeCrafted", false);
            worldData.Add("AtFurnace", true);
            worldData.Add("AtCraftingTable", false);
            worldData.Add("AtTree", false);
            worldData.Add("AtMine", false);
            worldData.Add("IngotCount", 0);
            worldData.Add("OreCount", 0);
            worldData.Add("StickCount", 0);
            worldData.Add("WoodCount", 0);
            worldData.Add("HasAxe", false);
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("CraftPickaxeGoal")
                .WithCondition("PickaxeCrafted", true)
                .Build();
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("CraftPickaxe")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 2)
                    .WithCondition("IngotCount", ConditionDirection.GreaterThanEq, 3)
                    .WithCondition("AtCraftingTable", true)
                    .WithEffect("PickaxeCrafted", true)
                    .Build(),
                builder.CreateAction("SmeltOre")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("IngotCount", ConditionDirection.LessThanEq, 3)
                    .WithCondition("AtFurnace", true)
                    .WithCondition("OreCount", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("IngotCount", EffectDirection.Increase, 1)
                    .WithEffect("OreCount", EffectDirection.Decrease, 1)
                    .Build(),
                builder.CreateAction("ChopWood")
                    .WithStrategy(null)
                    .WithCost(_ => 3)
                    .WithCondition("WoodCount", ConditionDirection.LessThanEq, 3)
                    .WithCondition("AtTree", true)
                    .WithCondition("HasAxe", true)
                    .WithEffect("WoodCount", EffectDirection.Increase, 3)
                    .WithEffect("AtTree", false)
                    .Build(),
                builder.CreateAction("EquipAxe")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasAxe", false)
                    .WithEffect("HasAxe", true)
                    .Build(),
                builder.CreateAction("GoToTree")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtTree", false)
                    .WithEffect("AtTree", true)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToFurnace")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtFurnace", false)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", true)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToCraftingTable")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtCraftingTable", false)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",true)
                    .Build(),
                builder.CreateAction("GoToMine")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtMine", false)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtCraftingTable",false)
                    .WithEffect("AtMine", true)
                    .Build(),
                builder.CreateAction("CraftSticks")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("StickCount", ConditionDirection.LessThanEq, 2)
                    .WithCondition("AtCraftingTable",true)
                    .WithCondition("WoodCount", ConditionDirection.GreaterThanEq, 2)
                    .WithEffect("StickCount", EffectDirection.Increase, 4)
                    .Build(),
                builder.CreateAction("MineOre")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("OreCount", ConditionDirection.LessThanEq, 3)
                    .WithCondition("AtMine", true)
                    .WithEffect("OreCount", EffectDirection.Increase, 1)
                    .Build()
                );
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(14, plan.Actions.Count);
            Assert.AreEqual(35, plan.TotalCost);
            Assert.AreEqual("CraftPickaxeGoal", plan.Goal.GoalName);

        }

        [Test]
        public void TestExceedingLengthPlanFails()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new()
            {
                { "PickaxeCrafted", false },
                { "AtFurnace", false },
                { "AtCraftingTable", true },
                { "AtTree", false },
                { "AtMine", false },
                { "IngotCount", 0 },
                { "OreCount", 0 },
                { "StickCount", 0 },
                { "WoodCount", 0 },
                { "HasAxe", false },
                { "InEnd", false },
                { "EnderPearls", 0 },
                { "HasSword", false },
                { "DragonHealth", 100 },
                { "Stress", 0f }
            };

            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("WinGameGoal")
                .WithCondition("DragonHealth", ConditionDirection.LessThanEq, 0)
                .WithCondition("InEnd", true)
                .Build();
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("CraftPickaxe")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 2)
                    .WithCondition("IngotCount", ConditionDirection.GreaterThanEq, 3)
                    .WithCondition("AtCraftingTable", true)
                    .WithEffect("PickaxeCrafted", true)
                    .Build(),
                builder.CreateAction("SmeltOre")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("IngotCount", ConditionDirection.LessThanEq, 3)
                    .WithCondition("AtFurnace", true)
                    .WithCondition("OreCount", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("IngotCount", EffectDirection.Increase, 1)
                    .WithEffect("OreCount", EffectDirection.Decrease, 1)
                    .Build(),
                builder.CreateAction("ChopWood")
                    .WithStrategy(null)
                    .WithCost(_ => 3)
                    .WithCondition("WoodCount", ConditionDirection.LessThanEq, 3)
                    .WithCondition("AtTree", true)
                    .WithCondition("HasAxe", true)
                    .WithEffect("WoodCount", EffectDirection.Increase, 3)
                    .WithEffect("AtTree", false)
                    .Build(),
                builder.CreateAction("EquipAxe")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasAxe", false)
                    .WithEffect("HasAxe", true)
                    .Build(),
                builder.CreateAction("GoToTree")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtTree", false)
                    .WithEffect("AtTree", true)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToFurnace")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtFurnace", false)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", true)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToCraftingTable")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtCraftingTable", false)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",true)
                    .Build(),
                builder.CreateAction("GoToMine")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtMine", false)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtCraftingTable",false)
                    .WithEffect("AtMine", true)
                    .Build(),
                builder.CreateAction("CraftSticks")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("StickCount", ConditionDirection.LessThanEq, 2)
                    .WithCondition("AtCraftingTable",true)
                    .WithCondition("WoodCount", ConditionDirection.GreaterThanEq, 2)
                    .WithEffect("StickCount", EffectDirection.Increase, 4)
                    .Build(),
                builder.CreateAction("MineOre")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("OreCount", ConditionDirection.LessThanEq, 3)
                    .WithCondition("AtMine", true)
                    .WithEffect("OreCount", EffectDirection.Increase, 1)
                    .Build(),
                builder.CreateAction("KillEnderman")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("EnderPearls", ConditionDirection.LessThanEq, 12)
                    .WithCondition("HasSword", true)
                    .WithEffect("EnderPearls",EffectDirection.Increase, 2)
                    .Build(),
                builder.CreateAction("MakeEndPortal")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("EnderPearls", ConditionDirection.GreaterThanEq, 12)
                    .WithCondition("InEnd", false)
                    .WithEffect("EnderPearls",EffectDirection.Decrease, 12)
                    .WithEffect("InEnd", true)
                    .Build(),
                builder.CreateAction("CraftSword")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 1)
                    .WithCondition("IngotCount", ConditionDirection.GreaterThanEq, 2)
                    .WithCondition("AtCraftingTable", true)
                    .WithCondition("HasSword", false)
                    .WithEffect("StickCount", EffectDirection.Decrease, 1)
                    .WithEffect("IngotCount", EffectDirection.Decrease, 2)
                    .WithEffect("HasSword", true)
                    .Build(),
                builder.CreateAction("FightDragon")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("InEnd", true)
                    .WithCondition("Stress", ConditionDirection.LessThanEq, 0.8f)
                    .WithEffect("DragonHealth", EffectDirection.Decrease, 15)
                    .WithEffect("Stress", EffectDirection.Increase, 0.34f)
                    .Build(),
                builder.CreateAction("Destress")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("Stress", ConditionDirection.GreaterThanEq, 0f)
                    .WithEffect("Stress", EffectDirection.Decrease, 0.4f)
                    .Build()
                );

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.Null(plan);
        }
        
        [Test]
        public void TestPlanManyActionsSimplePlan()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new()
            {
                { "PickaxeCrafted", false },
                { "AtFurnace", true },
                { "AtCraftingTable", false },
                { "AtTree", false },
                { "AtMine", false },
                { "IngotCount", 0 },
                { "OreCount", 0 },
                { "StickCount", 0 },
                { "WoodCount", 0 },
                { "HasAxe", false },
                { "InEnd", false },
                { "EnderPearls", 0 },
                { "HasSword", false },
                { "DragonHealth", 100 },
                { "Stress", 0f }
            };

            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("WinGameGoal")
                .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 10)
                .Build();
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("CraftPickaxe")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 2)
                    .WithCondition("IngotCount", ConditionDirection.GreaterThanEq, 3)
                    .WithCondition("AtCraftingTable", true)
                    .WithEffect("PickaxeCrafted", true)
                    .Build(),
                builder.CreateAction("SmeltOre")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("AtFurnace", true)
                    .WithCondition("OreCount", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("IngotCount", EffectDirection.Increase, 1)
                    .WithEffect("OreCount", EffectDirection.Decrease, 1)
                    .Build(),
                builder.CreateAction("ChopWood")
                    .WithStrategy(null)
                    .WithCost(_ => 3)
                    .WithCondition("AtTree", true)
                    .WithCondition("HasAxe", true)
                    .WithEffect("WoodCount", EffectDirection.Increase, 3)
                    .WithEffect("AtTree", false)
                    .Build(),
                builder.CreateAction("EquipAxe")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("HasAxe", true)
                    .Build(),
                builder.CreateAction("GoToTree")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", true)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToFurnace")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", true)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToCraftingTable")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",true)
                    .Build(),
                builder.CreateAction("GoToMine")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtCraftingTable",false)
                    .WithEffect("AtMine", true)
                    .Build(),
                builder.CreateAction("CraftSticks")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("AtCraftingTable",true)
                    .WithCondition("WoodCount", ConditionDirection.GreaterThanEq, 2)
                    .WithEffect("StickCount", EffectDirection.Increase, 4)
                    .Build(),
                builder.CreateAction("LootChestForOre")
                    .WithStrategy(null)
                    .WithCost(_ => 40)
                    .WithEffect("OreCount", EffectDirection.Increase, 1)
                    .Build(),
                builder.CreateAction("MineOre")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("PickaxeCrafted", true)
                    .WithCondition("AtMine", true)
                    .WithEffect("OreCount", EffectDirection.Increase, 1)
                    .Build(),
                builder.CreateAction("KillEnderman")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("HasSword", true)
                    .WithEffect("EnderPearls",EffectDirection.Increase, 2)
                    .Build(),
                builder.CreateAction("MakeEndPortal")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("EnderPearls", ConditionDirection.GreaterThanEq, 12)
                    .WithEffect("EnderPearls",EffectDirection.Decrease, 12)
                    .WithEffect("InEnd", true)
                    .Build(),
                builder.CreateAction("CraftSword")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 1)
                    .WithCondition("IngotCount", ConditionDirection.GreaterThanEq, 2)
                    .WithEffect("StickCount", EffectDirection.Decrease, 1)
                    .WithEffect("IngotCount", EffectDirection.Decrease, 2)
                    .WithEffect("HasSword", true)
                    .Build(),
                builder.CreateAction("FightDragon")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("InEnd", true)
                    .WithCondition("Stress", ConditionDirection.LessThanEq, 0.8f)
                    .WithEffect("DragonHealth", EffectDirection.Decrease, 15)
                    .WithEffect("Stress", EffectDirection.Increase, 0.34f)
                    .Build(),
                builder.CreateAction("Destress")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("Stress", ConditionDirection.GreaterThanEq, 0f)
                    .WithEffect("Stress", EffectDirection.Decrease, 0.4f)
                    .Build()
                );

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
        }

        [Test]
        public void TestPlanFindsEfficientPath()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new() { { "Health", 0 } };

            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("HealGoal")
                .WithCondition("Health", ConditionDirection.GreaterThanEq, 100)
                .Build();
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("TinyHeal")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("Health", EffectDirection.Increase, 1)
                    .Build(),
                builder.CreateAction("BigHeal")
                    .WithStrategy(null)
                    .WithCost(_ => 30)
                    .WithEffect("Health", EffectDirection.Increase, 33)
                    .Build());
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(91, plan.TotalCost);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual("HealGoal", plan.Goal.GoalName);

            Assert.AreEqual(100,worldState.GetInt("Health"));
        }
        
        [Test]
        public void TestPlanFindsBestPath()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("EnemyHealth", 100);

            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("KillEnemy")
                .WithCondition("EnemyHealth", ConditionDirection.LessThanEq, 0)
                .Build();
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("UltimateAttack")
                    .WithStrategy(null)
                    .WithCost(_ => 100)
                    .WithEffect("EnemyHealth", EffectDirection.Decrease, 100)
                    .Build(),
                builder.CreateAction("NormalAttack")
                    .WithStrategy(null)
                    .WithCost(_ => 10)
                    .WithEffect("EnemyHealth", EffectDirection.Decrease, 25)
                    .Build());
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(40, plan.TotalCost);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual("KillEnemy", plan.Goal.GoalName);

            Assert.AreEqual(0,worldState.GetInt("EnemyHealth"));
        }

        [Test]
        public void TestPlanContradictions()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Money", 25);
            worldData.Add("HasBed", false);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("HaveBothGoal")
                .WithCondition("Money", ConditionDirection.GreaterThanEq, 20)
                .WithCondition("HasBed", true)
                .Build();
            
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("BuyBed")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("Money", ConditionDirection.GreaterThanEq, 20)
                    .WithEffect("HasBed", true)
                    .WithEffect("Money", EffectDirection.Decrease, 20)
                    .Build(),
                builder.CreateAction("SellCrops")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("Money", EffectDirection.Increase, 5)
                    .Build());
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual(4, plan.TotalCost);
        }

        [Test]
        public void TestPlanMergingConditions()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Health", 10f);
            worldData.Add("BossHealth", 50);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("KillBoss")
                .WithCondition("BossHealth", ConditionDirection.LessThanEq, 0)
                .WithCondition("Health", ConditionDirection.GreaterThanEq, 30f)
                .Build();
            
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("TinyHeal")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithEffect("Health", EffectDirection.Increase, 5f)
                    .Build(),
                builder.CreateAction("BigHeal")
                    .WithStrategy(null)
                    .WithCost(_ => 15)
                    .WithEffect("Health", EffectDirection.Increase, 40f)
                    .Build(),
                builder.CreateAction("UltraAttack")
                    .WithStrategy(null)
                    .WithCost(_ => 6)
                    .WithCondition("Health", ConditionDirection.GreaterThanEq, 50f)
                    .WithEffect("BossHealth", EffectDirection.Decrease, 22)
                    .Build(),
                builder.CreateAction("MehAttack")
                    .WithStrategy(null)
                    .WithCost(_ => 4)
                    .WithEffect("BossHealth", EffectDirection.Decrease, 6)
                    .Build()
                );
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual(31, plan.TotalCost);
        }

        [Test]
        public void TestPlanImpossible()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("BeastHealth", 10);
            worldData.Add("IsWorthy", false);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("KillTheBeast")
                .WithCondition("BeastHealth", ConditionDirection.LessThanEq, 0)
                .Build();

            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("SlayBeast")
                    .WithStrategy(null)
                    .WithCost(_ => 20)
                    .WithCondition("IsWorthy", true)
                    .WithEffect("BeastHealth", EffectDirection.Decrease, 100)
                    .Build()
            );
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.Null(plan);
        }
        
        [Test]
        public void TestInfinitePlanImpossible()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Money", 10);
            worldData.Add("HasTicket", false);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("WinBig")
                .WithCondition("Money", ConditionDirection.GreaterThanEq, 100)
                .Build();

            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("Gamble")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasTicket", true)
                    .WithEffect("Money", EffectDirection.Increase, 10)
                    .WithEffect("HasTicket", false)
                    .Build(),
                builder.CreateAction("BuyTicket")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("Money", ConditionDirection.GreaterThanEq, 10)
                    .WithEffect("Money", EffectDirection.Decrease, 10)
                    .WithEffect("HasTicket", true)
                    .Build()
            );
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.Null(plan);
        }

        [Test]
        public void TestPlanNegativeCondition()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Temp", -20);
            worldData.Add("IceCreamForSale", false);
            worldData.Add("SnowmanCount", 3);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("SummerTime")
                .WithCondition("Temp", ConditionDirection.GreaterThanEq, 80)
                .WithCondition("IceCreamForSale", true)
                .WithCondition("SnowmanCount", ConditionDirection.LessThanEq, 0)
                .Build();

            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("ShineSun")
                    .WithStrategy(null)
                    .WithCost(_ => 10)
                    .WithEffect("Temp", EffectDirection.Increase, 25)
                    .Build(),
                builder.CreateAction("MeltSnowman")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("Temp", ConditionDirection.GreaterThanEq, 40)
                    .WithEffect("SnowmanCount", EffectDirection.Decrease, 1)
                    .Build(),
                builder.CreateAction("OpenIceCreamShop")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("Temp", ConditionDirection.GreaterThanEq, 70)
                    .WithCondition("SnowmanCount", ConditionDirection.LessThanEq, 0)
                    .WithEffect("IceCreamForSale", true)
                    .Build()
            );
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(48,plan.TotalCost);
            Assert.AreEqual(8,plan.Actions.Count);
        }

        [Test]
        public void TestFindsCheapestPathWithContradictions()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("AtBossLocation", true);
            worldData.Add("BandAids", 100);
            worldData.Add("MedKits", 0);
            worldData.Add("PickedUpMedkits", false);
            worldData.Add("Health", 10);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("BeginBossFight")
                .WithCondition("AtBossLocation", true)
                .WithCondition("Health", ConditionDirection.GreaterThanEq, 100)
                .Build();

            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("UseBandAid")
                    .WithStrategy(null)
                    .WithCost(_ => 3)
                    .WithCondition("BandAids", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("BandAids", EffectDirection.Decrease, 1)
                    .WithEffect("Health", EffectDirection.Increase, 3)
                    .Build(),
                builder.CreateAction("UseMedKit")
                    .WithStrategy(null)
                    .WithCost(_ => 10)
                    .WithCondition("MedKits", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("MedKits", EffectDirection.Decrease, 1)
                    .WithEffect("Health", EffectDirection.Increase, 45)
                    .Build(),
                builder.CreateAction("GoToBossLocation")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithEffect("AtBossLocation", true)
                    .Build(),
                builder.CreateAction("WalkThroughSpikesForMedkits")
                    .WithStrategy(null)
                    .WithCost(_ => 20)
                    .WithCondition("PickedUpMedkits", false)
                    .WithEffect("Health", EffectDirection.Decrease, 5)
                    .WithEffect("AtBossLocation", false)
                    .WithEffect("MedKits", EffectDirection.Increase, 5)
                    .WithEffect("PickedUpMedkits", true)
                    .Build()
            );
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(47,plan.TotalCost);
            Assert.AreEqual(6,plan.Actions.Count);
        }

        [Test]
        public void TestPlanLongBooleansOnly()
        {
            IGoapPlanner planner = TestUtils.GetPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("MonsterKilled", false);
            worldData.Add("HasWeapon", false);
            worldData.Add("IsHealthy", false);
            worldData.Add("IsHungry", true);
            worldData.Add("HasMaterials", false);
            worldData.Add("HasFood", false);
            worldData.Add("AtFarm", false);
            worldData.Add("HasFriends", false);
            worldData.Add("InGoodMood", false);
            worldData.Add("HasCat", false);
            worldData.Add("HasMedicalDiagnosis", false);
            worldData.Add("HasMedicine", false);
            worldData.Add("HasWater", false);
            worldData.Add("HasBucket", false);
            worldData.Add("InLair", false);
            worldData.Add("HasEnoughExp", false);
            worldData.Add("TravelReady", false);
            worldData.Add("SaidGoodbyes", false);
            worldData.Add("ByeMom", false);
            worldData.Add("ByeDad", false);
            worldData.Add("ByeSis", false);
            worldData.Add("ByeBro", false);

            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("WinGameGoal")
                .WithCondition("MonsterKilled", true)
                //.WithCondition("InLair", true)
                //.WithCondition("IsHungry", false)
                .Build();
            HashSet<GoapAction> actions = GoapUtils.AddAllActions(
                builder.CreateAction("KillMonster")
                    .WithStrategy(null)
                    .WithCost(_ =>1)
                    .WithCondition("HasWeapon", true)
                    .WithCondition("IsHealthy", true)
                    .WithCondition("HasFriends", true)
                    .WithCondition("IsHungry", false)
                    .WithCondition("InLair", true)
                    .WithEffect("MonsterKilled", true)
                    .Build(),
                builder.CreateAction("MakeWeapon")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("HasWeapon", false)
                    .WithCondition("HasMaterials", true)
                    .WithCondition("IsHungry", false)
                    .WithEffect("HasWeapon", true)
                    .Build(),
                builder.CreateAction("GatherMaterials")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasMaterials", false)
                    .WithEffect("HasMaterials", true)
                    .Build(),
                builder.CreateAction("Eat")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("IsHungry", true)
                    .WithCondition("HasFood", true)
                    .WithEffect("IsHungry", false)
                    .Build(),
                builder.CreateAction("FarmBread")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasFood", false)
                    .WithCondition("AtFarm", true)
                    .WithEffect("HasFood", true)
                    .Build(),
                builder.CreateAction("RaidHouse")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("HasFriends", true)
                    .WithCondition("HasMaterials", false)
                    .WithEffect("HasFood", true)
                    .WithEffect("HasMaterials", true)
                    .Build(),
                builder.CreateAction("MakeFriends")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("InGoodMood", true)
                    .WithCondition("HasFriends", false)
                    .WithEffect("HasFriends", true)
                    .Build(),
                builder.CreateAction("BoostMood")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("InGoodMood", false)
                    .WithCondition("HasCat", true)
                    .WithEffect("InGoodMood", true)
                    .Build(),
                builder.CreateAction("VisitFarm")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasCat", false)
                    .WithEffect("HasCat", true)
                    .Build(),
                builder.CreateAction("VisitDoctor")
                    .WithStrategy(null)
                    .WithCost(_ => 5)
                    .WithCondition("HasMedicalDiagnosis", false)
                    .WithEffect("HasMedicalDiagnosis", true)
                    .Build(),
                builder.CreateAction("ObtainMedicine")
                    .WithStrategy(null)
                    .WithCost(_ => 20)
                    .WithCondition("HasMedicine", false)
                    .WithCondition("HasMedicalDiagnosis", true)
                    .WithEffect("HasMedicine", true)
                    .Build(),
                builder.CreateAction("TakeMedicine")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("IsHealthy", false)
                    .WithCondition("HasWater", true)
                    .WithEffect("IsHealthy", true)
                    .Build(),
                builder.CreateAction("GetWater")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("HasWater", false)
                    .WithCondition("HasBucket", true)
                    .WithEffect("HasWater", true)
                    .Build(),
                builder.CreateAction("GetBucket")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("HasMaterials", true)
                    .WithCondition("HasBucket", false)
                    .WithEffect("HasBucket", true)
                    .WithEffect("HasMaterials", false)
                    .Build(),
                builder.CreateAction("TravelToLair")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("InLair", false)
                    .WithCondition("HasEnoughExp", true)
                    .WithEffect("InLair", true)
                    .Build(),
                builder.CreateAction("GrindLevels")
                    .WithStrategy(null)
                    .WithCost(_ => 7)
                    .WithCondition("TravelReady", true)
                    .WithCondition("HasEnoughExp", false)
                    .WithCondition("HasWeapon", true)
                    .WithEffect("HasEnoughExp", true)
                    .Build(),
                builder.CreateAction("Prepare")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("TravelReady", false)
                    .WithCondition("SaidGoodbyes", true)
                    .WithEffect("TravelReady", true)
                    .Build(),
                builder.CreateAction("SayGoodbyes")
                    .WithStrategy(null)
                    .WithCost(_ => 2)
                    .WithCondition("ByeMom", true)
                    .WithCondition("ByeDad", true)
                    .WithCondition("ByeSis", true)
                    .WithCondition("SaidGoodbyes", false)
                    .WithCondition("ByeBro", true)
                    .WithEffect("SaidGoodbyes", true)
                    .Build(),
                builder.CreateAction("ByeMom")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("ByeMom", false)
                    .WithEffect("ByeMom", true)
                    .Build(),
                builder.CreateAction("ByeSis")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("ByeSis", false)
                    .WithEffect("ByeSis", true)
                    .Build(),
                builder.CreateAction("ByeBro")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("ByeBro", false)
                    .WithEffect("ByeBro", true)
                    .Build(),
                builder.CreateAction("ByeDad")
                    .WithStrategy(null)
                    .WithCost(_ => 1)
                    .WithCondition("ByeDad", false)
                    .WithEffect("ByeDad", true)
                    .Build()
                );

            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
        }
    }
}