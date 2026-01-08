using System.Collections.Generic;
using GOAP.Runtime.API;
using GOAP.Runtime.Core;
using UnityEngine;

namespace GOAP.Runtime.Unity
{
    public class GoapAgent : MonoBehaviour
    {
        [SerializeField] private GoapAgentConfiguration goapConfiguration;
        [SerializeField] private float planningInterval = 0.5f;
        [Header("Debugging"), SerializeField] private bool printPlan;

        private ISet<GoapAction> actions;
        private ISet<GoapGoal> goals;
        private WorldState worldState;
        private MultithreadedPlanner planner;
        private GoapAction currentAction;
        private IActionPlan currentPlan;
        private float currentTime;

        private void Awake()
        {
            worldState = goapConfiguration.CreateWorldState();
            actions = goapConfiguration.CreateActions(new GoapActionBuilder());
            goals = goapConfiguration.CreateGoals(new GoapGoalBuilder());
            planner = new MultithreadedPlanner(goapConfiguration.CreatePlanner(), actions, goals);
            currentTime = 0;
        }

        private void Update()
        {
            if (currentTime <= 0)
            {
                RequestNewPlan();
                currentTime = planningInterval;
            }
            else
            {
                currentTime -= Time.deltaTime;
            }

            if (currentPlan != null)
            {
                if (currentAction == null || currentAction.IsComplete)
                {
                    currentAction?.StopAction();
                    currentAction = currentPlan.PopAction();
                    currentAction?.StartAction();
                }

                currentAction?.ExecuteAction();
            }

            if (!planner.GetNewPlanIfReady(out IActionPlan newPlan))
                return;

            bool shouldUseNewPlan =
                currentPlan == null
                || !currentPlan.Goal.Equals(newPlan.Goal)
                || (currentPlan.IsComplete && currentAction == null)
                || newPlan.TotalCost < currentPlan.TotalCost;

            if (!shouldUseNewPlan)
                return;
            currentPlan = newPlan;

            if (printPlan)
                PrintActionPlan();
        }

        private void RequestNewPlan()
        {
            IWorldState worldStateSnapshot = worldState.CreateSnapshot();
            GoapGoal bestGoal = planner.GetBestGoalMainThread(worldStateSnapshot);

            planner.SchedulePlanRequest(bestGoal, worldStateSnapshot);
        }

        public void ForceReplan()
        {
            currentPlan = null;
            currentTime = planningInterval;
            RequestNewPlan();
        }

        private void PrintActionPlan()
        {
            string printString = "";

            if (currentPlan == null)
            {
                print($"No plan could be found!");
                return;
            }
            
            printString += $"Goal: [{currentPlan.Goal.GoalName}]\n";
            printString += "Cost: " + currentPlan.TotalCost + "\n";
            printString += "Actions: ";
            bool first = true;
            foreach (GoapAction action in currentPlan.Actions)
            {
                if (!first)
                    printString += " -> ";
                first = false;
                printString += $"[{action.ActionName}]";
            }

            print(printString);
        }
    }
}