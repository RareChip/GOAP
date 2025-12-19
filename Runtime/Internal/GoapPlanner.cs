using System.Collections.Generic;
using GOAP.Runtime.Util;

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

                if (!visitedNodes.Add(current))
                {
                    continue;
                }
                
                if (current.Conditions.Count == 0)
                {
                    // We found a plan!
                    Stack<GoapAction> path = new Stack<GoapAction>();
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
                    if (!GoapResolver.EffectsSatisfyConditions(action.Effects, current.Conditions))
                        continue;
                    
                    HashSet<GoapCondition> newConditions = GoapResolver.ApplyEffects(action, current.Conditions);
                    
                    newConditions = GoapResolver.CombineConditionSets(newConditions, action.Conditions);

                    // Conditions of this action contradicted pre-existing conditions
                    if (newConditions == null)
                    {
                        continue;
                    }

                    newConditions.RemoveWhere(x => GoapResolver.ConditionIsSatisfied(x, worldState));
                    
                    int heuristic = newConditions.Count;
                    int newCost = action.CalculateCost(worldState) + current.Cost + heuristic;
                    
                    PlannerNode newNode = new PlannerNode(newConditions, current, newCost, action);
                    priorityQueue.Enqueue(newNode, newCost);
                }
            }
            
            return null;
        }
        
        
    }
}