using System.Collections.Generic;
using GOAP.Runtime;
using GOAP.Runtime.Internal;
using GOAP.Runtime.Util;
using NUnit.Framework;

namespace GOAP.Testing.UnitTests
{
    [TestFixture]
    public class ResolverTests
    {
        [Test]
        public void TestCombineConditionSetsSimple()
        {
            HashSet<GoapCondition> set1 = new HashSet<GoapCondition>();
            HashSet<GoapCondition> set2 = new HashSet<GoapCondition>();

            GoapCondition healthCond = new GoapCondition
            {
                Key = "Health",
                ConditionDirection = ConditionDirection.GreaterThanEq,
                Value = 30,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition moneyCond = new GoapCondition
            {
                Key = "Money",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 1000,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition distanceCond = new GoapCondition
            {
                Key = "DistanceToPlayer",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 10f,
                GoapDataType = GoapDataType.Float
            };
            GoapCondition scaredCond = new GoapCondition
            {
                Key = "IsScared",
                ConditionDirection = ConditionDirection.Equals,
                Value = false,
                GoapDataType = GoapDataType.Bool
            };
            
            set1.Add(healthCond);
            set1.Add(moneyCond);
            set2.Add(distanceCond);
            set2.Add(scaredCond);


            HashSet<GoapCondition> combinedSet = GoapResolver.CombineConditionSets(set1, set2);
            
            Assert.AreEqual(4, combinedSet.Count);
            Assert.IsTrue(combinedSet.Contains(healthCond));
            Assert.IsTrue(combinedSet.Contains(moneyCond));
            Assert.IsTrue(combinedSet.Contains(distanceCond));
            Assert.IsTrue(combinedSet.Contains(scaredCond));
        }
        
        [Test]
        public void TestCombineConditionSetsComplex()
        {
            HashSet<GoapCondition> set1 = new HashSet<GoapCondition>();
            HashSet<GoapCondition> set2 = new HashSet<GoapCondition>();

            GoapCondition healthCond = new GoapCondition
            {
                Key = "Health",
                ConditionDirection = ConditionDirection.GreaterThanEq,
                Value = 30,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition scaredCond = new GoapCondition
            {
                Key = "IsScared",
                ConditionDirection = ConditionDirection.Equals,
                Value = false,
                GoapDataType = GoapDataType.Bool
            };
            GoapCondition moneyCond = new GoapCondition
            {
                Key = "Money",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 1000,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition money2Cond = new GoapCondition
            {
                Key = "Money",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 500,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition health2Cond = new GoapCondition
            {
                Key = "Health",
                ConditionDirection = ConditionDirection.GreaterThanEq,
                Value = 50,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition scaredCond2 = new GoapCondition
            {
                Key = "IsScared",
                ConditionDirection = ConditionDirection.Equals,
                Value = false,
                GoapDataType = GoapDataType.Bool
            };
            
            set1.Add(healthCond);
            set1.Add(moneyCond);
            set1.Add(scaredCond);
            set2.Add(money2Cond);
            set2.Add(health2Cond);
            set2.Add(health2Cond);
            set2.Add(scaredCond2);


            HashSet<GoapCondition> combinedSet = GoapResolver.CombineConditionSets(set1, set2);
            
            Assert.AreEqual(3, combinedSet.Count);
            Assert.IsTrue(combinedSet.Contains(health2Cond));
            Assert.IsTrue(combinedSet.Contains(money2Cond));
            Assert.IsTrue(combinedSet.Contains(scaredCond2));
            Assert.IsTrue(combinedSet.Contains(scaredCond));
            
            Assert.IsFalse(combinedSet.Contains(healthCond));
            Assert.IsFalse(combinedSet.Contains(moneyCond));
        }
        
        [Test]
        public void TestCombineConditionSetsComplex1()
        {
            HashSet<GoapCondition> set1 = new HashSet<GoapCondition>();
            HashSet<GoapCondition> set2 = new HashSet<GoapCondition>();

            GoapCondition moneyCond = new GoapCondition
            {
                Key = "Money",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 1000,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition money2Cond = new GoapCondition
            {
                Key = "Money",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 5,
                GoapDataType = GoapDataType.Int
            };
            
            set1.Add(moneyCond);
            set2.Add(money2Cond);
            
            HashSet<GoapCondition> combinedSet = GoapResolver.CombineConditionSets(set1, set2);
            
            Assert.AreEqual(1, combinedSet.Count);
            Assert.IsTrue(combinedSet.Contains(money2Cond));
            
            Assert.IsFalse(combinedSet.Contains(moneyCond));
        }
        
        [Test]
        public void TestCombineConditionSetsIntContradiction()
        {
            // Yes, this test case is supposed to catch a fail.
            // The current implementation does not support having conditions that differ in direction.
            // (This means having range conditions are not currently possible.)
            // Example: 30 < Health < 50 is not possible.
            
            HashSet<GoapCondition> set1 = new HashSet<GoapCondition>();
            HashSet<GoapCondition> set2 = new HashSet<GoapCondition>();

            GoapCondition healthCond = new GoapCondition
            {
                Key = "Health",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 50,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition healthCond2 = new GoapCondition
            {
                Key = "Health",
                ConditionDirection = ConditionDirection.GreaterThanEq,
                Value = 30,
                GoapDataType = GoapDataType.Int
            };
            
            set1.Add(healthCond);
            set2.Add(healthCond2);
            
            HashSet<GoapCondition> combinedSet = GoapResolver.CombineConditionSets(set1, set2);
            
            Assert.Null(combinedSet);
        }
        
        [Test]
        public void TestCombineConditionSetsBoolContradiction()
        {
            // Yes, this test case is supposed to catch a fail.
            // The current implementation does not support having conditions that differ in direction.
            // (This means having range conditions are not currently possible.)
            // Example: 30 < Health < 50 is not possible.
            
            HashSet<GoapCondition> set1 = new HashSet<GoapCondition>();
            HashSet<GoapCondition> set2 = new HashSet<GoapCondition>();

            GoapCondition healthCond = new GoapCondition
            {
                Key = "Health",
                ConditionDirection = ConditionDirection.LessThanEq,
                Value = 50,
                GoapDataType = GoapDataType.Int
            };
            GoapCondition coolCond = new GoapCondition
            {
                Key = "IsCool",
                ConditionDirection = ConditionDirection.Equals,
                Value = false,
                GoapDataType = GoapDataType.Bool
            };
            GoapCondition coolCond2 = new GoapCondition
            {
                Key = "IsCool",
                ConditionDirection = ConditionDirection.Equals,
                Value = true,
                GoapDataType = GoapDataType.Bool
            };
            
            set1.Add(healthCond);
            set1.Add(coolCond);
            set2.Add(coolCond2);
            
            HashSet<GoapCondition> combinedSet = GoapResolver.CombineConditionSets(set1, set2);
            
            Assert.Null(combinedSet);
        }
        
        [Test]
        public void TestCombineConditionSetsBoolContradiction2()
        {
            // Yes, this test case is supposed to catch a fail.
            // The current implementation does not support having conditions that differ in direction.
            // (This means having range conditions are not currently possible.)
            // Example: 30 < Health < 50 is not possible.
            
            HashSet<GoapCondition> set1 = new HashSet<GoapCondition>();
            HashSet<GoapCondition> set2 = new HashSet<GoapCondition>();

            GoapCondition coolCond = new GoapCondition
            {
                Key = "IsCool",
                ConditionDirection = ConditionDirection.Equals,
                Value = false,
                GoapDataType = GoapDataType.Bool
            };
            GoapCondition coolCond2 = new GoapCondition
            {
                Key = "IsCool",
                ConditionDirection = ConditionDirection.NotEquals,
                Value = false,
                GoapDataType = GoapDataType.Bool
            };
            
            set1.Add(coolCond);
            set2.Add(coolCond2);
            
            HashSet<GoapCondition> combinedSet = GoapResolver.CombineConditionSets(set1, set2);
            
            Assert.Null(combinedSet);
        }
    }
}