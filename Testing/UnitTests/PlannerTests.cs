using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.Internal;
using GOAP.Runtime.Util;
using GOAP.Testing.ActionStrategies;
using NUnit.Framework;
using UnityEditor.VersionControl;

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

            plan = planner.GeneratePlan(null, new GoapGoal("", null, null), null);
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
                .WithCondition("WallsBuilt", ConditionDirection.GreaterThanEq, 3)
                .Build();

            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("WallsBuilt", 0);

            IWorldState worldState = new PlannerState(worldData);

            actions.Add(builder.CreateAction("BuildWall")
                .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
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
                .WithStrategy(new NoOpStrategy())
                .WithCost(_ => 1)
                .WithEffect("WallsBuilt",EffectDirection.Increase,1)
                .Build());
            
            actions.Add(builder.CreateAction("Prepare")
                .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
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
                .WithStrategy(new NoOpStrategy())
                .WithCost(_ => 1)
                .WithCondition("WoodCount", ConditionDirection.GreaterThanEq, 5)
                .WithCondition("HandsFree", ConditionDirection.Equals, true)
                .WithEffect("WallsBuilt", EffectDirection.Increase, 1)
                .WithEffect("WoodCount", EffectDirection.Decrease, 5)
                .Build());

            actions.Add(builder.CreateAction("GatherWood")
                .WithStrategy(new NoOpStrategy())
                .WithCost(_ => 1)
                .WithCondition("WoodInWorld", ConditionDirection.GreaterThanEq, 5)
                .WithEffect("WoodCount", EffectDirection.Increase, 5)
                .WithEffect("WoodInWorld", EffectDirection.Decrease, 5)
                .Build());

            actions.Add(builder.CreateAction("ChopTree")
                .WithStrategy(new NoOpStrategy())
                .WithCost(_ => 1)
                .WithCondition("HasAxe", true)
                .WithEffect("WoodInWorld", EffectDirection.Increase, 5)
                .Build());

            actions.Add(builder.CreateAction("PickupAxe")
                .WithStrategy(new NoOpStrategy())
                .WithCost(_ => 2)
                .WithCondition("HandsFree", true)
                .WithEffect("HasAxe", true)
                .WithEffect("HandsFree", false)
                .Build());

            actions.Add(builder.CreateAction("DropItem")
                .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
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
            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("CraftPickaxe")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 5)
                    .WithCondition("StickCount", ConditionDirection.GreaterThanEq, 2)
                    .WithCondition("IngotCount", ConditionDirection.GreaterThanEq, 3)
                    .WithCondition("AtCraftingTable", true)
                    .WithEffect("PickaxeCrafted", true)
                    .Build(),
                builder.CreateAction("SmeltOre")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 2)
                    .WithCondition("AtFurnace", true)
                    .WithCondition("OreCount", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("IngotCount", EffectDirection.Increase, 1)
                    .WithEffect("OreCount", EffectDirection.Decrease, 1)
                    .Build(),
                builder.CreateAction("ChopWood")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 3)
                    .WithCondition("AtTree", true)
                    .WithCondition("HasAxe", true)
                    .WithEffect("WoodCount", EffectDirection.Increase, 3)
                    .WithEffect("AtTree", false)
                    .Build(),
                builder.CreateAction("EquipAxe")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("HasAxe", true)
                    .Build(),
                builder.CreateAction("GoToTree")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", true)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToFurnace")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", true)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",false)
                    .Build(),
                builder.CreateAction("GoToCraftingTable")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtMine", false)
                    .WithEffect("AtCraftingTable",true)
                    .Build(),
                builder.CreateAction("GoToMine")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("AtTree", false)
                    .WithEffect("AtFurnace", false)
                    .WithEffect("AtCraftingTable",false)
                    .WithEffect("AtMine", true)
                    .Build(),
                builder.CreateAction("CraftSticks")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithCondition("AtCraftingTable",true)
                    .WithCondition("WoodCount", ConditionDirection.GreaterThanEq, 2)
                    .WithEffect("StickCount", EffectDirection.Increase, 4)
                    .Build(),
                builder.CreateAction("MineOre")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 5)
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
        public void TestPlanFindsBestPath()
        {
            IGoapPlanner planner = new GoapPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Health", 0);

            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("HealGoal")
                .WithCondition("Health", ConditionDirection.GreaterThanEq, 100)
                .Build();
            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("TinyHeal")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("Health", EffectDirection.Increase, 1)
                    .Build(),
                builder.CreateAction("BigHeal")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 30)
                    .WithEffect("Health", EffectDirection.Increase, 33)
                    .Build());
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            Assert.NotNull(plan);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(91, plan.TotalCost);
            Assert.AreEqual(4, plan.Actions.Count);
            Assert.AreEqual("HealGoal", plan.Goal.GoalName);

            Assert.AreEqual(100,worldState.Get<int>("Health"));
        }

        [Test]
        public void TestPlanContradictions()
        {
            IGoapPlanner planner = new GoapPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Money", 25);
            worldData.Add("HasBed", false);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("HaveBothGoal")
                .WithCondition("Money", ConditionDirection.GreaterThanEq, 20)
                .WithCondition("HasBed", true)
                .Build();
            
            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("BuyBed")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithCondition("Money", ConditionDirection.GreaterThanEq, 20)
                    .WithEffect("HasBed", true)
                    .WithEffect("Money", EffectDirection.Decrease, 20)
                    .Build(),
                builder.CreateAction("SellCrops")
                    .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Health", 10f);
            worldData.Add("BossHealth", 50);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("KillBoss")
                .WithCondition("BossHealth", ConditionDirection.LessThanEq, 0)
                .WithCondition("Health", ConditionDirection.GreaterThanEq, 15f)
                .Build();
            
            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("TinyHeal")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 5)
                    .WithEffect("Health", EffectDirection.Increase, 5f)
                    .Build(),
                builder.CreateAction("BigHeal")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 15)
                    .WithEffect("Health", EffectDirection.Increase, 40f)
                    .Build(),
                builder.CreateAction("UltraAttack")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 6)
                    .WithCondition("Health", ConditionDirection.GreaterThanEq, 50f)
                    .WithEffect("BossHealth", EffectDirection.Decrease, 22)
                    .Build(),
                builder.CreateAction("MehAttack")
                    .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("BeastHealth", 10);
            worldData.Add("IsWorthy", false);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("KillTheBeast")
                .WithCondition("BeastHealth", ConditionDirection.LessThanEq, 0)
                .Build();

            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("SlayBeast")
                    .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
            IGoapActionBuilder builder = new GoapActionBuilder();
            Dictionary<string, object> worldData = new Dictionary<string, object>();
            worldData.Add("Money", 10);
            worldData.Add("HasTicket", false);
            
            PlannerState worldState = new PlannerState(worldData);
            GoapGoal goal = new GoapGoal.Builder("WinBig")
                .WithCondition("Money", ConditionDirection.GreaterThanEq, 100)
                .Build();

            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("Gamble")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithCondition("HasTicket", true)
                    .WithEffect("Money", EffectDirection.Increase, 10)
                    .WithEffect("HasTicket", false)
                    .Build(),
                builder.CreateAction("BuyTicket")
                    .WithStrategy(new NoOpStrategy())
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
            IGoapPlanner planner = new GoapPlanner();
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

            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("ShineSun")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 10)
                    .WithEffect("Temp", EffectDirection.Increase, 25)
                    .Build(),
                builder.CreateAction("MeltSnowman")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithCondition("Temp", ConditionDirection.GreaterThanEq, 40)
                    .WithEffect("SnowmanCount", EffectDirection.Decrease, 1)
                    .Build(),
                builder.CreateAction("OpenIceCreamShop")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 5)
                    .WithCondition("Temp", ConditionDirection.GreaterThanEq, 70)
                    .WithCondition("SnowmanCount", ConditionDirection.LessThanEq, 0)
                    .WithEffect("IceCreamForSale", true)
                    .Build()
            );
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(48,plan.TotalCost);
            Assert.AreEqual(8,plan.Actions.Count);
        }

        [Test]
        public void TestFindsBestPathWithContradictions()
        {
            IGoapPlanner planner = new GoapPlanner();
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

            HashSet<GoapAction> actions = TestUtils.AddAllActions(
                builder.CreateAction("UseBandAid")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 3)
                    .WithCondition("BandAids", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("BandAids", EffectDirection.Decrease, 1)
                    .WithEffect("Health", EffectDirection.Increase, 3)
                    .Build(),
                builder.CreateAction("UseMedKit")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 10)
                    .WithCondition("MedKits", ConditionDirection.GreaterThanEq, 1)
                    .WithEffect("MedKits", EffectDirection.Decrease, 1)
                    .WithEffect("Health", EffectDirection.Increase, 45)
                    .Build(),
                builder.CreateAction("GoToBossLocation")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 1)
                    .WithEffect("AtBossLocation", true)
                    .Build(),
                builder.CreateAction("WalkThroughSpikesForMedkits")
                    .WithStrategy(new NoOpStrategy())
                    .WithCost(_ => 20)
                    .WithCondition("PickedUpMedkits", false)
                    .WithEffect("Health", EffectDirection.Decrease, 5)
                    .WithEffect("AtBossLocation", false)
                    .WithEffect("MedKits", EffectDirection.Increase, 5)
                    .WithEffect("PickedUpMedkits", true)
                    .Build()
            );
            
            ActionPlan plan = planner.GeneratePlan(actions, goal, worldState);
            TestUtils.AssertPlanMakesSense(plan, worldState, goal);
            Assert.AreEqual(47,plan.TotalCost);
            Assert.AreEqual(6,plan.Actions.Count);
        }
    }
}