using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Util;
using UnityEngine;

namespace GOAP.Runtime.Internal
{
    public class GoapForwardPlanner : IGoapPlanner
    {
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals, IWorldState worldState)
        {
            return null;
        }

        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState)
        {
            if (actions is null || goal is null || worldState is null)
                return null;

            HashSet<ForwardNode> visitedNodes = new HashSet<ForwardNode>();
            PriorityQueue<ForwardNode, int> priorityQueue = new PriorityQueue<ForwardNode, int>();
            Dictionary<string, int> maxChange = CalculateMaxChangePerAction(actions);

            ForwardNode startingNode = new ForwardNode(worldState.Clone(), null, 0, null);
            priorityQueue.Enqueue(startingNode, 0);

            while (priorityQueue.Count > 0)
            {
                ForwardNode current = priorityQueue.Dequeue();

                if (!visitedNodes.Add(current))
                {
                    continue;
                }

                if (goal.Conditions.All(x => current.WorldState.ConditionIsSatisfied(x)))
                {
                    // We found a plan!
                    Stack<GoapAction> path = new();
                    int totalCost = current.Cost;
                    while (current.ParentNode != null)
                    {
                        path.Push(current.Action);
                        current = current.ParentNode;
                    }

                    return new ActionPlan(goal, path, totalCost);
                }

                foreach (GoapAction action in actions)
                {
                    if (!action.Conditions.All(x => current.WorldState.ConditionIsSatisfied(x)))
                        continue;

                    IWorldState newState = current.WorldState.Clone();

                    foreach (GoapEffect effect in action.Effects)
                    {
                        newState.ApplyEffect(effect);
                    }

                    int cost = current.Cost + action.CalculateCost(worldState);

                    int heuristic = 
                        goal.Conditions.Sum(x => this.CalculateHeuristic(x, maxChange, newState));

                    ForwardNode newNode = new ForwardNode(newState, current, cost, action);
                    priorityQueue.Enqueue(newNode, cost + heuristic);
                }
            }

            return null;
        }

        private Dictionary<string, int> CalculateMaxChangePerAction(HashSet<GoapAction> actions)
        {
            Dictionary<string, int> maxChange = new Dictionary<string, int>();

            foreach (GoapAction action in actions)
            {
                foreach (GoapEffect effect in action.Effects)
                {
                    switch (effect.GoapDataType)
                    {
                        case GoapDataType.Int:
                            if (maxChange.TryAdd(effect.Key, (int)effect.Value))
                                continue;
                            if (maxChange[effect.Key] < (int)effect.Value)
                            {
                                maxChange[effect.Key] = (int)effect.Value;
                            }

                            break;
                        case GoapDataType.Float:
                            if (maxChange.TryAdd(effect.Key, Mathf.RoundToInt((float)effect.Value)))
                                continue;
                            if (maxChange[effect.Key] < (float)effect.Value)
                            {
                                maxChange[effect.Key] = Mathf.RoundToInt((float)effect.Value);
                            }

                            break;
                    }
                }
            }

            return maxChange;
        }
        
        // private int CalculateHeuristic(GoapCondition condition, IWorldState newState)
        // {
        //     if (newState.ConditionIsSatisfied(condition))
        //         return 0;
        //
        //     return condition.GoapDataType switch
        //     {
        //         GoapDataType.Bool => 1,
        //         GoapDataType.Enum => 1,
        //         GoapDataType.Int => condition.ConditionDirection switch
        //         {
        //             ConditionDirection.LessThanEq =>
        //                 Math.Max(0, newState.Get<int>(condition.Key) - (int)condition.Value),
        //             ConditionDirection.GreaterThanEq =>
        //                 Math.Max(0, (int)condition.Value - newState.Get<int>(condition.Key)),
        //             _ => throw new ArgumentOutOfRangeException()
        //         },
        //         GoapDataType.Float => condition.ConditionDirection switch
        //         {
        //             ConditionDirection.LessThanEq =>
        //                 Math.Max(0, Mathf.RoundToInt(newState.Get<float>(condition.Key) - (float)condition.Value)),
        //             ConditionDirection.GreaterThanEq =>
        //                 Math.Max(0, Mathf.RoundToInt((float)condition.Value - newState.Get<float>(condition.Key))),
        //             _ => throw new ArgumentOutOfRangeException()
        //         },
        //         _ => throw new ArgumentOutOfRangeException()
        //     };
        // }

        private int CalculateHeuristic(GoapCondition condition, Dictionary<string, int> maxChange, IWorldState newState)
        {
            if (newState.ConditionIsSatisfied(condition))
                return 0;
        
            return condition.GoapDataType switch
            {
                GoapDataType.Bool => 1,
                GoapDataType.Enum => 1,
                GoapDataType.Int => condition.ConditionDirection switch
                {
                    ConditionDirection.LessThanEq =>
                        Mathf.CeilToInt((float)Math.Max(0, newState.Get<int>(condition.Key) - (int)condition.Value)
                                        / maxChange[condition.Key]),
                    ConditionDirection.GreaterThanEq =>
                        Mathf.CeilToInt((float)Math.Max(0, (int)condition.Value - newState.Get<int>(condition.Key))
                                        / maxChange[condition.Key]),
                    _ => throw new ArgumentOutOfRangeException()
                },
                GoapDataType.Float => condition.ConditionDirection switch
                {
                    ConditionDirection.LessThanEq =>
                        Mathf.CeilToInt(Math.Max(0, newState.Get<float>(condition.Key) - (float)condition.Value)
                                        / maxChange[condition.Key]),
                    ConditionDirection.GreaterThanEq =>
                        Mathf.CeilToInt(Math.Max(0, (float)condition.Value - newState.Get<float>(condition.Key))
                                        / maxChange[condition.Key]),
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}