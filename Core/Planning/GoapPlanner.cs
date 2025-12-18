using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Core.Agent;
using GOAP.Util;
using Utils;

namespace GOAP.Core.Planning
{
    public class GoapPlanner
    {
        public GoapPlanner()
        {
            
        }
        
        // This is interesting. We can implement an interface here for a planning strategy for goals. The default
        // implementation will use Utility AI
        public GoapGoal GenerateBestGoal(HashSet<GoapGoal> goals)
        {
            
            return null;
        }

        public ActionPlan GeneratePlan(HashSet<GoapAction> actions, GoapGoal goal, PlannerState plannerState)
        {
            PriorityQueue<PlannerNode, int> priorityQueue = new PriorityQueue<PlannerNode, int>();
            HashSet<PlannerNode> visitedNodes = new HashSet<PlannerNode>();
            
            PlannerNode startingNode = new PlannerNode(new HashSet<GoapCondition>(goal.Conditions), null);
            priorityQueue.Enqueue(startingNode, 0);

            while (priorityQueue.Count > 0)
            {
                PlannerNode current = priorityQueue.Dequeue();

                if (!visitedNodes.Add(current))
                {
                    continue;
                }

                HashSet<GoapCondition> unsatisfiedConditions = new HashSet<GoapCondition>();

                foreach (GoapCondition condition in current.Conditions)
                {
                    if (GoapResolver.ConditionIsSatisfied(condition, plannerState))
                        continue;

                    unsatisfiedConditions.Add(condition);
                }

                if (unsatisfiedConditions.Count == 0)
                {
                    // We found a plan!
                    return new ActionPlan();
                }
                
                foreach (GoapAction action in actions)
                {
                    HashSet<GoapCondition> newConditions = GoapResolver.ApplyEffects(action, unsatisfiedConditions);
                    
                    if(newConditions.SetEquals(unsatisfiedConditions)
                       || !GoapResolver.EffectsResolveConditions(unsatisfiedConditions, newConditions)
                       )
                        continue;
                    
                    newConditions = GoapResolver.CombineConditionSets(newConditions, action.Conditions);
                    
                    // Might need a better heuristic for float/int conditions. 
                    int heuristic = newConditions.Count;
                    int newCost = action.CalculateCost(plannerState) + current.Edge.Cost + heuristic;
                    
                    PlannerEdge edge = new PlannerEdge(current, newCost, action);
                    PlannerNode newNode = new PlannerNode(newConditions, edge);
                    priorityQueue.Enqueue(newNode, newCost);
                }
            }
            
            return null;
        }
        
        
    }
}