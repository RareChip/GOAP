using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GOAP.Runtime.Util;

namespace GOAP.Runtime.Internal
{
    public class GoapForwardPlanner : IGoapPlanner
    {
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals)
        {
            return null;
        }

        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState)
        {
            if (actions is null || goal is null || worldState is null)
                return null;
            
            HashSet<ForwardNode> visitedNodes = new HashSet<ForwardNode>();
            PriorityQueue<ForwardNode, int> priorityQueue = new PriorityQueue<ForwardNode, int>();
            Dictionary<string, List<(GoapCondition, int)>> costMap = GenerateCostMap(actions, goal, worldState);
            
            ForwardNode startingNode = new ForwardNode(worldState.Clone(), null, 0, null);
            priorityQueue.Enqueue(startingNode, 0);

            while (priorityQueue.Count > 0)
            {
                ForwardNode current = priorityQueue.Dequeue();
              
                if (!visitedNodes.Add(current))
                {
                    continue;
                }
                if (goal.Conditions.All(x => GoapResolver.ConditionIsSatisfied(x, current.WorldState)))
                {
                    // We found a plan!
                    Queue<GoapAction> path = new Queue<GoapAction>();
                    int totalCost = current.Cost;
                    while (current.ParentNode != null)
                    {
                        path.Enqueue(current.Action);
                        current = current.ParentNode;
                    }

                    return new ActionPlan(goal, new Queue<GoapAction>(path.Reverse()), totalCost);
                }

                foreach (GoapAction action in actions)
                {
                    if(!action.Conditions.All(x => GoapResolver.ConditionIsSatisfied(x, current.WorldState)))
                        continue;
                    
                    IWorldState newState = current.WorldState.Clone();
                    
                    foreach (GoapEffect effect in action.Effects)
                    {
                        switch (effect.EffectDirection)
                        {
                            case EffectDirection.Set:
                                newState.Update(effect.Key, effect.Value);
                                break;
                            case EffectDirection.Increase:
                                switch (effect.GoapDataType)
                                {
                                    case GoapDataType.Int:
                                        int oldInt = newState.Get<int>(effect.Key);
                                        newState.Update(effect.Key, oldInt + (int)effect.Value);
                                        break;
                                    case GoapDataType.Float:
                                        float oldFloat = newState.Get<float>(effect.Key);
                                        newState.Update(effect.Key, oldFloat + (float)effect.Value);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                            case EffectDirection.Decrease:
                                switch (effect.GoapDataType)
                                {
                                    case GoapDataType.Int:
                                        int oldInt = newState.Get<int>(effect.Key);
                                        newState.Update(effect.Key, oldInt - (int)effect.Value);
                                        break;
                                    case GoapDataType.Float:
                                        float oldFloat = newState.Get<float>(effect.Key);
                                        newState.Update(effect.Key, oldFloat - (float)effect.Value);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                break;
                        }
                    }

                    int cost = current.Cost + action.CalculateCost(worldState);
                    int heuristic = goal.Conditions.Count(x => !GoapResolver.ConditionIsSatisfied(x, newState));
                    ForwardNode newNode = new ForwardNode(newState, current, cost, action);
                    priorityQueue.Enqueue(newNode, cost + heuristic);
                }
            }
            
            return null;
        }

        private Dictionary<string, List<(GoapCondition, int)>> GenerateCostMap(HashSet<GoapAction> actions, GoapGoal goal, IWorldState worldState)
        {
            Dictionary<string, List<(GoapCondition, int)>> costMap =
                new Dictionary<string, List<(GoapCondition, int)>>();

            int currentLevel = 0;

            foreach (GoapAction action in actions)
            {
                
            }
            
            return costMap;
        }
    }
}