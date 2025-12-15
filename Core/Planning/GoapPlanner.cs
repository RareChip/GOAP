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
            
            PlannerNode startingNode = new PlannerNode(goal.Conditions.ToArray(), null);
            priorityQueue.Enqueue(startingNode, 0);

            while (priorityQueue.Count > 0)
            {
                PlannerNode current = priorityQueue.Dequeue();
                int conditionsUnsatisfied = 0;

                if (!visitedNodes.Add(current))
                {
                    continue;
                }

                foreach (GoapCondition condition in current.Conditions)
                {
                    if (GoapResolver.ConditionIsSatisfied(condition, plannerState))
                        continue;

                    conditionsUnsatisfied++;
                    //Otherwise, find all actions that satisfy this condition.
                    PlannerState newState = plannerState.Clone();
                    foreach (GoapAction action in actions)
                    {
                        foreach (GoapEffect effect in action.Effects)
                        {
                            if (!string.Equals(effect.Key, condition.Key, StringComparison.CurrentCultureIgnoreCase))
                                continue;
                            
                            
                        }
                    }
                    
                }

                if (conditionsUnsatisfied == 0)
                {
                    // We found a plan!
                }
            }
            
            return null;
        }
        
        
    }
}