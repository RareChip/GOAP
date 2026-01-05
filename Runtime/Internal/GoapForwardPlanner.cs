using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Internal
{
    public class GoapForwardPlanner : IGoapPlanner
    {
        private readonly int maxPlanLength;
        public GoapForwardPlanner(int maxPlanLength)
        {
            this.maxPlanLength = maxPlanLength;
        }

        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals, IWorldState worldState)
        {
            GoapGoal bestGoal = goals.OrderByDescending(x => x.CalculateInsistence).First();
            return bestGoal;
        }

        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState)
        {
            if (actions is null || goal is null || worldState is null)
                return null;

            HashSet<IWorldState> visitedStates = new();
            PriorityQueue<ForwardNode, int> priorityQueue = new();

            ForwardNode startingNode = new ForwardNode(worldState.Clone(), null, 0, null);
            priorityQueue.Enqueue(startingNode, 0);

            while (priorityQueue.Count > 0)
            {
                ForwardNode current = priorityQueue.Dequeue();
                
                if (!visitedStates.Add(current.WorldState))
                {
                    continue;
                }

                bool allSatisfied = true;
                foreach (GoapCondition condition in goal.Conditions)
                {
                    if (current.WorldState.ConditionIsSatisfied(condition)) 
                        continue;
                    allSatisfied = false;
                    break;
                }
                
                if(allSatisfied)
                {
                    Stack<GoapAction> path = new();
                    int totalCost = current.Cost;
                    while (current.ParentNode != null)
                    {
                        path.Push(current.Action);
                        current = current.ParentNode;
                    }

                    return new ActionPlan(goal, path, totalCost);
                }

                if(current.CurrentPlanLength > this.maxPlanLength)
                    continue;
                
                foreach (GoapAction action in actions)
                {
                    allSatisfied = true;
                    foreach (GoapCondition condition in action.Conditions)
                    {
                        if (current.WorldState.ConditionIsSatisfied(condition)) 
                            continue;
                        allSatisfied = false;
                        break;
                    }

                    if (!allSatisfied)
                        continue;

                    IWorldState newState = current.WorldState.Clone();

                    foreach (GoapEffect effect in action.Effects)
                    {
                        newState.ApplyEffect(effect);
                    }

                    int cost = current.Cost + action.CalculateCost(worldState);

                    int heuristic = 0;
                    
                    foreach (GoapCondition condition in goal.Conditions)
                    {
                        heuristic += CalculateHeuristic(condition, newState);
                    }

                    ForwardNode newNode = new ForwardNode(newState, current, cost, action);
                    priorityQueue.Enqueue(newNode, cost + heuristic);
                }
            }

            return null;
        }
        private static int CalculateHeuristic(GoapCondition condition, IWorldState newState)
        {
            if (newState.ConditionIsSatisfied(condition))
                return 0;
        
            return condition.GoapDataType switch
            {
                GoapDataType.Bool => 3,
                GoapDataType.Enum => 3,
                GoapDataType.Int => condition.ConditionDirection switch
                {
                    ConditionDirection.LessThanEq =>
                        Math.Max(0, newState.GetInt(condition.Key) - (int)condition.Value),
                    ConditionDirection.GreaterThanEq =>
                        Math.Max(0, (int)condition.Value - newState.GetInt(condition.Key)),
                    _ => throw new ArgumentOutOfRangeException()
                },
                GoapDataType.Float => condition.ConditionDirection switch
                {
                    ConditionDirection.LessThanEq =>
                        Math.Max(0, (int)Math.Round(newState.GetFloat(condition.Key) - (float)condition.Value)),
                    ConditionDirection.GreaterThanEq =>
                        Math.Max(0, (int)Math.Round((float)condition.Value - newState.GetFloat(condition.Key))),
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}