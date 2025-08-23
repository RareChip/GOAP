using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RareChip.Goap
{
    public class GoapPlanner
    {
        private readonly Dictionary<string, List<GoapAction>> adjacencyList;
        private readonly GoapAgent agent;
        private readonly float nonExactActionCostNerf;

        public GoapPlanner(List<GoapAction> totalActions, GoapAgent agent, float nonExactActionCostNerf)
        {
            adjacencyList = new Dictionary<string, List<GoapAction>>();
            this.agent = agent;
            this.nonExactActionCostNerf = nonExactActionCostNerf;

            //
            // Fill adjacency list mapping conditions to actions that COULD satisfy them
            //
            foreach (GoapAction action in totalActions)
            {
                foreach (GoapCondition condition in action.Effects)
                {
                    if (!adjacencyList.ContainsKey(condition.ConditionKey))
                    {
                        adjacencyList.Add(condition.ConditionKey, new List<GoapAction>());
                    }

                    if (!adjacencyList[condition.ConditionKey].Contains(action))
                    {
                        adjacencyList[condition.ConditionKey].Add(action);
                    }
                }
            }
        }

        public List<(GoapAction config, bool perfectlySatisfies)> GetActionsThatSatisfyCondition(
            GoapCondition condition)
        {
            List<(GoapAction config, bool perfectlySatisfies)> actionsThatSatisfy =
                new List<(GoapAction config, bool perfectlySatisfies)>();

            if (adjacencyList.ContainsKey(condition.ConditionKey))
            {
                foreach (GoapAction action in adjacencyList[condition.ConditionKey])
                {
                    foreach (GoapCondition actionEffect in action.Effects)
                    {
                        bool add = false;
                        bool perfect = true;
                        switch (condition.conditionType)
                        {
                            case ConditionType.True:
                            case ConditionType.False:
                                add = condition.conditionType == actionEffect.conditionType;
                                break;
                            case ConditionType.Equals:

                                switch (actionEffect.conditionType)
                                {
                                    case ConditionType.Equals:
                                        add = condition.value == actionEffect.value;
                                        break;
                                    case ConditionType.LessThan:
                                        // If the action's effect lowers its value, it will eventually reach the desired value
                                        add = condition.value < actionEffect.value;
                                        perfect = false;
                                        break;
                                    case ConditionType.GreaterThan:
                                        add = condition.value > actionEffect.value;
                                        perfect = false;
                                        break;
                                }

                                break;
                            case ConditionType.NotEquals:

                                switch (actionEffect.conditionType)
                                {
                                    case ConditionType.Equals:
                                        add = condition.value != actionEffect.value;
                                        break;
                                    case ConditionType.NotEquals:
                                        add = condition.value == actionEffect.value;
                                        perfect = false;
                                        break;
                                    case ConditionType.LessThan:
                                    case ConditionType.GreaterThan:
                                        // If the action's effect changes its value, it will eventually leave the undesired value
                                        add = condition.value == actionEffect.value;
                                        break;
                                }

                                break;
                            case ConditionType.GreaterThan:

                                switch (actionEffect.conditionType)
                                {
                                    case ConditionType.Equals:
                                        // Makes valid if the action's effect makes the value higher than what we want it
                                        add = actionEffect.value > condition.value;
                                        break;
                                    case ConditionType.GreaterThan:
                                        add = condition.value > actionEffect.value;
                                        perfect = false;
                                        break;
                                }

                                break;
                            case ConditionType.LessThan:

                                switch (actionEffect.conditionType)
                                {
                                    case ConditionType.Equals:
                                        // Makes valid if the action's effect makes the value less than what we want it
                                        add = actionEffect.value < condition.value;
                                        break;
                                    case ConditionType.LessThan:
                                        add = condition.value < actionEffect.value;
                                        perfect = false;
                                        break;
                                }

                                break;
                        }

                        if (add)
                        {
                            actionsThatSatisfy.Add((action, perfect));
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"No actions satisfy the condition: {condition.ConditionKey}!");
            }


            return actionsThatSatisfy;
        }

        public Plan GetPlan(List<IGoal> goals, List<ActionConfig> actionsToInclude, GoapAgent agent)
        {
            //sort goals by insistence
            goals.Sort((goal1, goal2) => goal2.Insistance.CompareTo(goal1.Insistance));
            IGoal highestPriorityGoal = goals[0];
            GoapCondition desiredConditionState = highestPriorityGoal.Config.Precondition;
            PriorityQueue<PlannerNode> priorityQueue =
                new PriorityQueue<PlannerNode>((plannerNodeA, plannerNodeB) => plannerNodeA.Cost < plannerNodeB.Cost);
            List<GoapAction> visitedActions = new List<GoapAction>();

            foreach ((GoapAction action, bool perfectMatch) action in GetActionsThatSatisfyCondition(
                         desiredConditionState))
            {
                float penalty = action.perfectMatch ? 0 : nonExactActionCostNerf;
                priorityQueue.Enqueue(new PlannerNode(null, action.action, agent, penalty));
            }

            PlannerNode currentNode = null;

            while (priorityQueue.Count > 0)
            {
                currentNode = priorityQueue.Dequeue();
                GoapAction currentAction = currentNode.Action;

                //In Djikstras, when we find an action with no conditions, that is the shortest and most cost efficient path,
                //so we know that we can take it right away
                if (currentAction.Preconditions.Count == 0)
                    break;

                visitedActions.Add(currentAction);

                int conditionsSatisfied = 0;

                foreach (GoapCondition conditionScriptable in currentAction.Preconditions)
                {
                    //If the condition is satisfied, we don't need to add an action to the queue to satisfy it
                    if (agent.EvaluateCondition(conditionScriptable))
                    {
                        conditionsSatisfied++;
                        continue;
                    }

                    foreach ((GoapAction config, bool perfectMatch) action in GetActionsThatSatisfyCondition(
                                 conditionScriptable))
                    {
                        if (visitedActions.Contains(action.config))
                            continue;

                        float penalty = action.perfectMatch ? 0 : nonExactActionCostNerf;
                        priorityQueue.Enqueue(new PlannerNode(currentNode, action.config, agent,
                            currentNode.Cost + penalty));
                    }
                }

                //If after checking all of the action's conditions, they are all satisfied, this action is the best path
                if (conditionsSatisfied == currentAction.Preconditions.Count)
                    break;

                if (priorityQueue.Count == 0)
                {
                    Debug.LogError("Couldn't find any path to satisfy this goal. Goal: " +
                                   highestPriorityGoal.Config.Name);
                    return null;
                }
            }

            Queue<GoapAction> bestPath = new Queue<GoapAction>();
            while (currentNode != null)
            {
                bestPath.Enqueue(currentNode.Action);
                currentNode = currentNode.Parent;
            }


            return new Plan(bestPath.ToList());
            //return null;
        }
    }

    public class Plan
    {
        public int planID { get; private set; }

        public List<GoapAction> plan { get; private set; }

        private string planString = "";

        public Plan(List<GoapAction> actions)
        {
            foreach (var action in actions)
            {
                planID += action.GetHashCode();

                if (planString != "")
                    planString += " -> ";

                planString += "[" + action.Name + "]";
            }

            plan = actions;
        }

        public override string ToString()
        {
            return planString;
        }
    }

    public class PlannerNode
    {
        public PlannerNode Parent { get; }

        public float Cost
        {
            get { return costSoFar + Action.Cost; }
        }

        public GoapAction Action { get; private set; }

        private float costSoFar;

        public PlannerNode(PlannerNode parent, GoapAction action, GoapAgent agent, float costSoFar)
        {
            Parent = parent;
            this.costSoFar = costSoFar;
            Action = action;
        }
    }

    public class PriorityQueue<T>
    {
        protected List<T> heap;

        private Func<T, T, bool> lessThan;

        public int Count
        {
            get { return heap.Count; }
        }

        public PriorityQueue(Func<T, T, bool> lessThanComparison)
        {
            heap = new List<T>();
            lessThan = lessThanComparison;
        }

        public T Enqueue(T element)
        {
            heap.Add(element);
            UpHeap(heap.Count - 1);
            return element;
        }

        public T Dequeue()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            T result = heap[0];
            T last = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                heap[0] = last;
                DownHeap(0);
            }

            return result;
        }

        public T Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            return heap[0];
        }

        public void Clear()
        {
            heap.Clear();
        }

        private void UpHeap(int index)
        {
            T node = heap[index];
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                T parent = heap[parentIndex];

                if (!lessThan(node, parent))
                {
                    break;
                }

                heap[index] = parent;
                index = parentIndex;
            }

            heap[index] = node;
        }

        private void DownHeap(int index)
        {
            int size = heap.Count;
            T node = heap[index];

            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild < size && lessThan(heap[leftChild], heap[smallest]))
                {
                    smallest = leftChild;
                }

                if (rightChild < size && lessThan(heap[rightChild], heap[smallest]))
                {
                    smallest = rightChild;
                }

                if (smallest == index)
                {
                    break;
                }

                heap[index] = heap[smallest];
                index = smallest;
            }

            heap[index] = node;
        }
    }
}