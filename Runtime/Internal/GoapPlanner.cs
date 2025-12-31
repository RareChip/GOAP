using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Util;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public class GoapPlanner : IGoapPlanner
    {
        // This is interesting. We can implement an interface here for a planning strategy for goals. The default
        // implementation will use Utility AI
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals)
        {

            return null;
        }

        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState)
        {
            if (actions is null || goal is null || worldState is null)
            {
                return null;
            }

            PriorityQueue<PlannerNode, int> priorityQueue = new PriorityQueue<PlannerNode, int>();
            HashSet<PlannerNode> visitedNodes = new HashSet<PlannerNode>();
            
            PlannerNode startingNode = new PlannerNode(
                new HashSet<GoapCondition>(goal.Conditions),
                null,
                0,
                null);
            priorityQueue.Enqueue(startingNode, 0);

            while (priorityQueue.Count > 0)
            {
                PlannerNode current = priorityQueue.Dequeue();


                if (visitedNodes.Contains(current) ||
                    (visitedNodes.Any(x => x.IsJustAsGood(current, worldState) && x.Cost <= current.Cost)))
                {
                    continue;
                }

                visitedNodes.Add(current);


                bool allConditionsSatisfied =
                    current.Conditions.All(x => GoapResolver.ConditionIsSatisfied(x, worldState));
                if (allConditionsSatisfied)
                {
                    // We found a plan!
                    Queue<GoapAction> path = new Queue<GoapAction>();
                    int totalCost = current.Cost;
                    while (current.ParentNode != null)
                    {
                        path.Enqueue(current.Action);
                        current = current.ParentNode;
                    }

                    return new ActionPlan(goal, path, totalCost);
                }

                foreach (GoapAction action in actions)
                {
                    if (!GoapResolver.EffectsSatisfyConditions(action.Effects, current.ConditionMap))
                        continue;


                    HashSet<GoapCondition> newConditions =
                        GoapResolver.ApplyEffects(action.EffectMap, current, worldState);

                    newConditions = GoapResolver.CombineConditionSets(newConditions, action.Conditions);

                    // Conditions of this action contradicted pre-existing conditions
                    if (newConditions == null)
                    {
                        continue;
                    }

                    int heuristic = newConditions.Sum(x =>
                        GoapResolver.ConditionIsSatisfied(x, worldState) ? 0 : CalculateHeuristic(worldState, x)
                    );

                    int nodeCost = action.CalculateCost(worldState) + current.Cost;
                    int priority = nodeCost + heuristic;


                    PlannerNode newNode = new PlannerNode(newConditions, current, nodeCost, action);
                    priorityQueue.Enqueue(newNode, priority);
                }
            }

            return null;
        }

        private static int CalculateHeuristic(IWorldState worldState, GoapCondition condition)
        {
            string key = condition.Key;
            switch (condition.GoapDataType)
            {
                case GoapDataType.Int:
                    int currentInt = worldState.Get<int>(key);
                    int condInt = (int)condition.Value;
                    return Mathf.Max(0,(currentInt - condInt));
                    return condition.ConditionDirection switch
                    {
                        ConditionDirection.GreaterThanEq => Mathf.Max(0, condInt - currentInt),
                        ConditionDirection.LessThanEq => Mathf.Max(0, currentInt - condInt),
                        ConditionDirection.Equals => condInt == currentInt ? 0 : 1,
                        ConditionDirection.NotEquals => condInt != currentInt ? 0 : 1,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                case GoapDataType.Float:
                    float currentF = worldState.Get<float>(key);
                    float condFloat = (float)condition.Value;
                    return Math.Max(0,Mathf.RoundToInt(currentF - condFloat));
                    return condition.ConditionDirection switch
                    {
                        ConditionDirection.GreaterThanEq => Mathf.RoundToInt(Mathf.Max(0, condFloat - currentF)),
                        ConditionDirection.LessThanEq => Mathf.RoundToInt(Mathf.Max(0, currentF - condFloat)),
                        ConditionDirection.Equals => Mathf.Approximately(condFloat, currentF) ? 0 : 1,
                        ConditionDirection.NotEquals => !Mathf.Approximately(condFloat,currentF) ? 0 : 1,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                case GoapDataType.Bool:
                case GoapDataType.Enum:
                    return condition.Value.Equals(worldState.Get<object>(key)) ? 0 : 1;
            }

            return 0;
        }

        public Dictionary<string, int> CalculateCostMap(HashSet<GoapAction> actions, IWorldState state)
        {
            Dictionary<string, int> costMap = new Dictionary<string, int>();

            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (GoapAction action in actions)
                {
                    int totalCost = action.CalculateCost(state);
                
                    foreach (GoapEffect effect in action.Effects)
                    {
                        if (costMap.TryGetValue(effect.Key, out int val))
                        {
                            if (val < totalCost)
                                continue;
                            costMap[effect.Key] = totalCost;
                            changed = true;
                        }
                        else
                        {
                            costMap.Add(effect.Key, totalCost);
                            changed = true;
                        }
                    }
                }
            }
            

            return costMap;
        }
        
    }
}