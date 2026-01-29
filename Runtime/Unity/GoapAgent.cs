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
        [SerializeField] private bool startAutomatically = true;
        
        [Header("Debugging"), SerializeField] private bool printPlan;

        private ISet<GoapAction> actions;
        private ISet<GoapGoal> goals;
        private WorldState worldState;
        private MultithreadedPlanner actionPlanner;
        private IGoalPlanner goalPlanner;
        private GoapGoal bestGoal;
        private GoapAction currentAction;
        private IActionPlan currentPlan;
        private float currentTime;
        private bool canPlan;

        private void Awake()
        {
            worldState = goapConfiguration.CreateWorldState(this);
            actions = goapConfiguration.CreateActions(this, new GoapActionBuilder());
            goals = goapConfiguration.CreateGoals(this, new GoapGoalBuilder());
            actionPlanner = new MultithreadedPlanner(goapConfiguration.CreateActionPlanner(), actions, goals);
            goalPlanner = goapConfiguration.CreateGoalPlanner();
            currentTime = 0;
            canPlan = startAutomatically;
        }

        private void Update()
        {
            if (!canPlan)
                return;
            
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

            if (!actionPlanner.GetNewPlanIfReady(out IActionPlan newPlan))
                return;

            bool shouldUseNewPlan =
                currentPlan == null
                || !currentPlan.Goal.Equals(newPlan.Goal)
                || (currentPlan.IsComplete && currentAction == null)
                || newPlan.TotalCost < currentPlan.TotalCost;

            if (!shouldUseNewPlan)
                return;
            currentPlan = newPlan;
            currentAction?.StopAction();
            currentAction = null;
            
            if (printPlan)
                PrintActionPlan();
        }

        private void RequestNewPlan()
        {
            IWorldState worldStateSnapshot = worldState.CreateSnapshot();
            bestGoal = goalPlanner.GenerateBestGoal(goals, worldStateSnapshot);

            actionPlanner.SchedulePlanRequest(bestGoal, worldStateSnapshot);
        }

        public void ForceReplan()
        {
            currentPlan = null;
            currentTime = planningInterval;
            RequestNewPlan();
        }

        public void EnablePlanning(bool shouldPlan)
        {
            canPlan = shouldPlan;
        }

        private void PrintActionPlan()
        {
            string printString = "";

            if (currentPlan == null)
            {
                print($"No plan could be found for goal [{bestGoal.GoalName}]!");
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